using Relief.Domain.Contracts;
using Relief.Domain.Entities.Offers;
using Relief.Domain.Entities.Users;
using Relief.Domain.Exceptions;
using Relief.ServiceAbstraction.Interfaces.Offers;
using Relief.Services.Implementations;
using Relief.Services.Implementations.Offers.Specifications;
using Shared.OffersDTOs.CreateDTO;
using Shared.OffersDTOs.OfferInfoDTO;
using Shared.OffersDTOs.UpdateDTO;
using Shared.QueryDTOs;
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
                Address = dto.Address,
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
                HourlyRate = offer.HourlyRate,
                Address = offer.Address,
                Latitude = offer.Latitude,
                Longitude = offer.Longitude,
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
                HourlyRate = offer.HourlyRate,
                Address = offer.Address,
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
        public async Task<List<JobOfferSummaryDto>> GetAllOffersAsync(Guid? userId)
        {
            var offerRepo = _unitOfWork.GetRepository<JobOffer, Guid>();

            IEnumerable<JobOffer> results;

            if (userId.HasValue)
            {
                var spec = new OffersByOwnerSpecification(userId.Value);

                results = await offerRepo.GetAllAsync(spec);
            }
            else
            {
                results = await offerRepo.GetAllAsync();
            }

            return results.Select(o => new JobOfferSummaryDto
            {
                Id = o.Id,
                Title = o.Title,
                Address = o.Address,
                HourlyRate = o.HourlyRate,
                Latitude = o.Latitude,
                Longitude = o.Longitude
            }).ToList();
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

            if (!string.IsNullOrWhiteSpace(dto.Description))
                offer.Description = dto.Description;

            if (!string.IsNullOrWhiteSpace(dto.Address))
                offer.Address = dto.Address;

            if (dto.Latitude.HasValue)
                offer.Latitude = dto.Latitude.Value;

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

            offerRepo.Delete(offer);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}

