using Relief.Domain.Contracts;
using Relief.Domain.Entities;
using Relief.ServiceAbstraction.Interfaces;
using Relief.Services.Specifications;
using Shared.OffersDTOs.CreateDTO;
using Shared.OffersDTOs.OfferInfoDTO;
using Shared.OffersDTOs.UpdateDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations
{
    public class OfferService : IOfferService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OfferService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> CreateOfferAsync(Guid careHomeId, CreateJobOfferDto dto)
        {
            //  Basic Offer Validation
            if (dto.Shifts == null || !dto.Shifts.Any())
                throw new Exception("Offer must contain at least one shift.");

            //  Create Offer
            var offer = new JobOffer
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                Address = dto.Address,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                HourlyRate = dto.HourlyRate,
                CareHomeId = careHomeId
            };


            var shiftRepo = _unitOfWork.GetRepository<OfferShift, Guid>();
            //  Days & Shifts Validation + Mapping
            foreach (var shiftDto in dto.Shifts)
            {
                if (shiftDto.StartTime == null || shiftDto.EndTime == null)
                    throw new Exception($"Day {shiftDto.Date:yyyy-MM-dd} must contain at least one shift.");

                // Validate shifts times
                if (shiftDto.StartTime >= shiftDto.EndTime)
                    throw new Exception(
                        $"Invalid shift time on {shiftDto.Date:yyyy-MM-dd}: StartTime must be before EndTime."
                    );

                // Validate overlapping shifts
                var shiftSpecification = new JobOfferShiftSpecification(careHomeId, shiftDto.Date);
                var existingShiftsForDate = await shiftRepo.GetAllAsync(shiftSpecification);

                // 2. Check if the new shift overlaps with any existing shift
                foreach (var existingShift in existingShiftsForDate) // IMPORTANT !! recheck in another time
                {
                    // The Overlap Formula
                    bool isOverlapping = shiftDto.StartTime < existingShift.EndTime &&
                                         shiftDto.EndTime > existingShift.StartTime;

                    if (isOverlapping)
                    {
                        throw new Exception($"Overlap detected! The shift from {shiftDto.StartTime} to {shiftDto.EndTime} on {shiftDto.Date:yyyy-MM-dd} interrupts an existing shift ({existingShift.StartTime} to {existingShift.EndTime}).");
                    }
                }


                // Create Day
                var shift = new OfferShift
                {
                    Id = Guid.NewGuid(),
                    Date = shiftDto.Date,
                    JobOfferId = offer.Id,
                    StartTime = shiftDto.StartTime,
                    EndTime = shiftDto.EndTime
                };

                offer.Shifts.Add(shift);
            }

            //  Save
            var repository = _unitOfWork.GetRepository<JobOffer, Guid>();
            await repository.AddAsync(offer);

            await _unitOfWork.SaveChangesAsync();
            return offer.Id;
        }

        public async Task<JobOfferDetailsDto?> GetOfferByIdAsync(Guid id)
        {
            var _offerRepo = _unitOfWork.GetRepository<JobOffer, Guid>();

            var spec = new JobOfferWithDetailsSpecification(id);
            var offer = await _offerRepo.GetByIdAsync(spec);

            if (offer == null)
                return null;

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

        public async Task<List<JobOfferSummaryDto>> GetAllOffersAsync()
        {
            var _offerRepo = _unitOfWork.GetRepository<JobOffer, Guid>();
            var offers = await _offerRepo.GetAllAsync();


            return offers.Select(o => new JobOfferSummaryDto
            {
                Id = o.Id,
                Title = o.Title,
                Address = o.Address,
                HourlyRate = o.HourlyRate,
                AvailableDaysCount = o.Shifts.Count
            }).ToList();
        }

        public async Task<bool> UpdateOfferAsync(Guid offerId, Guid careHomeId, UpdateJobOfferDto dto)
        {
            var _offerRepo = _unitOfWork.GetRepository<JobOffer, Guid>();
            var spec = new JobOfferWithDetailsSpecification(offerId);
            var offer = await _offerRepo.GetByIdAsync(spec);
            Console.WriteLine("=================================");
            Console.WriteLine(offer.Id);
            Console.WriteLine(offer.Shifts.Count);
            Console.WriteLine("=================================");

            if (offer == null)
                return false;

            if (offer.CareHomeId != careHomeId)
                throw new UnauthorizedAccessException("You cannot update this offer.");

            // Update only main fields (stable version)
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


            // 🔥 مهم: مش هنعدل Days دلوقتي عشان نثبت الـ Update الأول
            if (dto.Shifts != null && dto.Shifts.Any())
            {
                foreach (var dayDto in dto.Shifts)
                {
                    // لو فيه DayId يبقى تعديل
                    if (dayDto.ShiftId.HasValue)
                    {
                        var existingShift = offer.Shifts
                            .FirstOrDefault(d => d.Id == dayDto.ShiftId.Value);

                        // doesn't handle dubliation yet
                        if (existingShift != null)
                        {
                            if(dayDto.Date != null)
                                existingShift.Date = dayDto.Date;
                            if(dayDto.StartTime != null)
                                existingShift.StartTime = dayDto.StartTime;
                            if(dayDto.EndTime != null)
                                existingShift.EndTime = dayDto.EndTime;
                        }
                        else
                        {
                            // لو DayId جاي بس مش موجود في الداتابيز
                            // نمنع اللخبطة
                            throw new Exception("Day not found for update.");
                        }
                    }
                }
            }

            var repository = _unitOfWork.GetRepository<JobOffer, Guid>();
            //repository.Update(offer);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }


        public async Task<bool> DeleteOfferAsync(Guid offerId, Guid careHomeId)
        {
            var _offerRepo = _unitOfWork.GetRepository<JobOffer, Guid>();
            var offer = await _offerRepo.GetByIdAsync(offerId);

            if (offer == null)
                return false;

            if (offer.CareHomeId != careHomeId)
                throw new UnauthorizedAccessException("You cannot delete this offer.");

            var repository = _unitOfWork.GetRepository<JobOffer, Guid>();
            repository.Delete(offer);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

    }
}

