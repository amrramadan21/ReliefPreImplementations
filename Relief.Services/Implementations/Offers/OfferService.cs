using Relief.Domain.Contracts;
using Relief.Domain.Entities.Offers;
using Relief.Domain.Entities.Users;
using Relief.Domain.Exceptions;
using Relief.ServiceAbstraction.Interfaces.Offers;
using Relief.Services.Implementations;
using Relief.Services.Implementations.Applications.Specifications;
using Relief.Services.Implementations.Offers.Specifications;
using Shared.OffersDTOs.CreateDTO;
using Shared.OffersDTOs.OfferInfoDTO;
using Shared.OffersDTOs.UpdateDTO;
using Shared.QueryDTOs;
using Shared.QueryDTOs.JobOffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations.Offers
{

    public class OfferService : IOfferService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OfferService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // =========================================================
        // CREATE OFFER
        // =========================================================
        public async Task<Guid> CreateOfferAsync(Guid userId, CreateJobOfferDto dto)
        {
            if (dto.Shifts == null || !dto.Shifts.Any())
                throw new BadRequestException("Offer must contain at least one shift.");

            var careHomeRepo = _unitOfWork.GetRepository<CareHomeUser, Guid>();
            var individualRepo = _unitOfWork.GetRepository<IndividualCareHomeUser, Guid>();

            var careHome = await careHomeRepo.GetByIdAsync(userId);

            var offer = new JobOffer
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                Position = dto.Position,
                Address = dto.Address,
                Address2 = dto.Address2,
                City = dto.City,
                Preferences = new List<string>(dto.Preferences ?? Enumerable.Empty<string>()),
                PostalCode = dto.PostalCode,
                Province = dto.Province,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                HourlyRate = dto.HourlyRate
            };

            if (careHome != null)
            {
                offer.CareHomeId = userId;
            }
            else
            {
                var individual = await individualRepo.GetByIdAsync(userId);

                if (individual == null)
                    throw new NotFoundException("User is not CareHome or Individual.");

                offer.IndividualId = userId;
            }

            var shiftRepo = _unitOfWork.GetRepository<OfferShift, Guid>();

            foreach (var shiftDto in dto.Shifts)
            {
                if (shiftDto.StartTime == null || shiftDto.EndTime == null)
                    throw new BadRequestException($"Shift on {shiftDto.Date:yyyy-MM-dd} must have start and end time.");

                bool isOvernightShift = shiftDto.EndTime <= shiftDto.StartTime;

                var shiftStart = shiftDto.Date.ToDateTime(shiftDto.StartTime.Value);

                var shiftEnd = isOvernightShift
                    ? shiftDto.Date.AddDays(1).ToDateTime(shiftDto.EndTime.Value)
                    : shiftDto.Date.ToDateTime(shiftDto.EndTime.Value);

                var spec = new JobOfferShiftSpecification(userId, shiftDto.Date);

                var existingShifts = await shiftRepo.GetAllAsync(spec);

                foreach (var existingShift in existingShifts)
                {
                    var existingStart = existingShift.Date.Value.ToDateTime(existingShift.StartTime.Value);

                    bool existingOvernight = existingShift.EndTime <= existingShift.StartTime;

                    var existingEnd = existingOvernight
                        ? existingShift.Date.Value.AddDays(1).ToDateTime(existingShift.EndTime.Value)
                        : existingShift.Date.Value.ToDateTime(existingShift.EndTime.Value);

                    bool isOverlapping =
                        shiftStart < existingEnd &&
                        shiftEnd > existingStart;

                    if (isOverlapping)
                        throw new ConflictException($"Shift overlaps with existing shift on {shiftDto.Date:yyyy-MM-dd}.");
                }

                offer.Shifts.Add(new OfferShift
                {
                    Id = Guid.NewGuid(),
                    Date = shiftDto.Date,
                    JobOfferId = offer.Id,
                    StartTime = shiftDto.StartTime,
                    EndTime = shiftDto.EndTime
                });
            }

            var offerRepo = _unitOfWork.GetRepository<JobOffer, Guid>();

            await offerRepo.AddAsync(offer);

            await _unitOfWork.SaveChangesAsync();

            return offer.Id;
        }

        // =========================================================
        // GET OFFER BY ID
        // =========================================================
        public async Task<JobOfferDetailsDto> GetOfferByIdAsync(Guid id, Guid userId)
        {
            var offerRepo = _unitOfWork.GetRepository<JobOffer, Guid>();

            var spec = new JobOfferWithDetailsSpecification(id);

            var offer = await offerRepo.GetByIdAsync(spec);

            if (offer == null ||
               (offer.CareHomeId != userId && offer.IndividualId != userId))
                throw new NotFoundException("Offer not found.");

            return new JobOfferDetailsDto
            {
                Id = offer.Id,
                Title = offer.Title,
                Description = offer.Description,
                Position = offer.Position,
                HourlyRate = offer.HourlyRate,
                Address = offer.Address,
                Address2 = offer.Address2,
                City = offer.City,
                PostalCode = offer.PostalCode,
                Province = offer.Province,
                Preferences = new List<string>(offer.Preferences ?? Enumerable.Empty<string>()),
                Latitude = offer.Latitude,
                Longitude = offer.Longitude,
                PosterId = offer.CareHomeId ?? offer.IndividualId,
                PosterType = offer.CareHomeId.HasValue ? "CareHome" : "Individual",
                PosterName = GetPosterName(offer),
                Shifts = offer.Shifts.Select(s => new OfferShiftDetailsDto
                {
                    ShiftId = s.Id,
                    Date = s.Date,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    IsAvailable = s.IsAvailable
                }).ToList()
            };
        }

        // =========================================================
        // VIEW OFFER DETAILS FOR PSW
        // =========================================================
        public async Task<JobOfferDetailsDto?> GetOfferDetailsForPswAsync(Guid offerId)
        {
            var repo = _unitOfWork.GetRepository<JobOffer, Guid>();

            var spec = new JobOfferWithDetailsSpecification(offerId);

            var offer = await repo.GetByIdAsync(spec);

            if (offer == null)
                throw new NotFoundException("Offer not found");

            return new JobOfferDetailsDto
            {
                Id = offer.Id,
                Title = offer.Title,
                Description = offer.Description,
                Position = offer.Position,
                HourlyRate = offer.HourlyRate,
                Address = offer.Address,
                Address2 = offer.Address2,
                City = offer.City,
                PostalCode = offer.PostalCode,
                Province = offer.Province,
                Preferences = new List<string>(offer.Preferences ?? Enumerable.Empty<string>()),        
                Latitude = offer.Latitude,
                Longitude = offer.Longitude,
                Shifts = offer.Shifts
                    .Where(s => s.IsAvailable)
                    .Select(s => new OfferShiftDetailsDto
                    {
                        ShiftId = s.Id,
                        Date = s.Date,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        IsAvailable = s.IsAvailable
                    }).ToList()
            };
        }

        // =========================================================
        // GET ALL OFFERS
        // =========================================================
        public async Task<Pagination<JobOfferSummaryDto>> GetAllOffersAsync(JobOfferQueryParams query)
        {
            var offerRepo = _unitOfWork.GetRepository<JobOffer, Guid>();

            var spec = new JobOfferPaginatedSpecification(query);
            var countSpec = new JobOfferCountSpecification(query);

            var data = await offerRepo.GetAllAsync(spec);
            var totalCount = await offerRepo.CountAsync(countSpec);


            var dtos = data.Select(o => new JobOfferSummaryDto
            {
                Id = o.Id,
                Title = o.Title,
                Position = o.Position,
                Address = o.Address,
                Address2 = o.Address2,
                City = o.City,
                PostalCode = o.PostalCode,
                Province = o.Province,
                Preferences = new List<string>(o.Preferences ?? Enumerable.Empty<string>()),
                HourlyRate = o.HourlyRate,
                Latitude = o.Latitude,
                Longitude = o.Longitude,
                PosterId = o.CareHomeId ?? o.IndividualId,
                PosterName = GetPosterName(o),
                PosterType = o.CareHomeId.HasValue ? "CareHome" : "Individual"
            }).ToList();

            return new Pagination<JobOfferSummaryDto>(
                query.PageIndex,
                query.PageSize,
                totalCount,
                dtos);
        }

        //pagi
        public async Task<Pagination<JobOffer>> GetOffersAsync(BaseQueryParams query)
        {
            var repo = _unitOfWork.GetRepository<JobOffer, Guid>();

            var countSpec = new PaginationSpecification<JobOffer, Guid>(o => true);

            var totalCount = await repo.CountAsync(countSpec);

            var spec = new PaginationSpecification<JobOffer, Guid>(
                o => true,
                o => o.Title,
                query.PageIndex,
                query.PageSize
            );

            var data = await repo.GetAllAsync(spec);

            return new Pagination<JobOffer>(
                query.PageIndex,
                query.PageSize,
                totalCount,
                data.ToList());
        }

        // =========================================================
        // UPDATE OFFER
        // =========================================================
        public async Task<bool> UpdateOfferAsync(Guid offerId, Guid userId, UpdateJobOfferDto dto)
        {
            var offerRepo = _unitOfWork.GetRepository<JobOffer, Guid>();

            var spec = new JobOfferWithDetailsSpecification(offerId);

            var offer = await offerRepo.GetByIdAsync(spec);

            if (offer == null)
                throw new NotFoundException("Offer not found.");

            if (offer.CareHomeId != userId && offer.IndividualId != userId)
                throw new ForbiddenException("You cannot update this offer.");

            if (!string.IsNullOrWhiteSpace(dto.Title))
                offer.Title = dto.Title;
            if (!string.IsNullOrWhiteSpace(dto.Position))
                offer.Position = dto.Position;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                offer.Description = dto.Description;

            if (!string.IsNullOrWhiteSpace(dto.Address))
                offer.Address = dto.Address;
            
            if(!string.IsNullOrWhiteSpace(dto.Address2))
                offer.Address2 = dto.Address2;

            if (!string.IsNullOrWhiteSpace(dto.City))
                offer.City = dto.City;

            if (!string.IsNullOrWhiteSpace(dto.PostalCode))
                if (dto.Latitude.HasValue)
                offer.Latitude = dto.Latitude.Value;

            if (!string.IsNullOrWhiteSpace(dto.Province))
                offer.Province = dto.Province;

            if(dto.Preferences != null && dto.Preferences.Count > 0)
                offer.Preferences = new List<string>(dto.Preferences ?? Enumerable.Empty<string>());

            if (dto.Longitude.HasValue)
                offer.Longitude = dto.Longitude.Value;

            if (dto.HourlyRate.HasValue)
                offer.HourlyRate = dto.HourlyRate.Value;

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        // =========================================================
        // DELETE OFFER
        // =========================================================
        public async Task<bool> DeleteOfferAsync(Guid offerId, Guid userId)
        {
            var offerRepo = _unitOfWork.GetRepository<JobOffer, Guid>();

            var offer = await offerRepo.GetByIdAsync(offerId);

            if (offer == null)
                throw new NotFoundException("Offer not found.");

            if (offer.CareHomeId != userId && offer.IndividualId != userId)
                throw new ForbiddenException("You cannot delete this offer.");

            var requestRepo = _unitOfWork.GetRepository<JopRequest, Guid>();

            var requestSpec = new JopRequestsByOfferIdSpec(offerId);
            var relatedRequests = await requestRepo.GetAllAsync(requestSpec);

            foreach (var request in relatedRequests)
            {
                requestRepo.Delete(request);
            }

            offerRepo.Delete(offer);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private static string? GetPosterName(JobOffer offer)
        {
            if (offer.CareHomeId.HasValue && offer.CareHomeUser != null)
                return offer.CareHomeUser.LegalName;

            if (offer.IndividualId.HasValue && offer.IndividualCareHomeUser?.ApplicationUser != null)
            {
                var user = offer.IndividualCareHomeUser.ApplicationUser;
                return $"{user.FirstName} {user.LastName}".Trim();
            }

            return null;
        }
    }

}

