using Relief.Domain.Entities;
using Relief.ServiceAbstraction.Interfaces;
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
        private readonly IJobOfferRepository _offerRepo;
        public OfferService(IJobOfferRepository offerRepo) 
        {
            _offerRepo = offerRepo;
        }
        public async Task<Guid> CreateOfferAsync(Guid careHomeId, CreateJobOfferDto dto)
        {
            //  Basic Offer Validation
            if (dto.Days == null || !dto.Days.Any())
                throw new Exception("Offer must contain at least one day.");

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

            //  Days & Shifts Validation + Mapping
            foreach (var dayDto in dto.Days)
            {
                if (dayDto.Shifts == null || !dayDto.Shifts.Any())
                    throw new Exception($"Day {dayDto.Date:yyyy-MM-dd} must contain at least one shift.");

                // Validate shifts times
                foreach (var shiftDto in dayDto.Shifts)
                {
                    if (shiftDto.StartTime >= shiftDto.EndTime)
                        throw new Exception(
                            $"Invalid shift time on {dayDto.Date:yyyy-MM-dd}: StartTime must be before EndTime."
                        );
                }

                // Validate overlapping shifts
                var orderedShifts = dayDto.Shifts
                    .OrderBy(s => s.StartTime)
                    .ToList();

                for (int i = 0; i < orderedShifts.Count - 1; i++)
                {
                    if (orderedShifts[i].EndTime > orderedShifts[i + 1].StartTime)
                        throw new Exception(
                            $"Overlapping shifts detected on {dayDto.Date:yyyy-MM-dd}."
                        );
                }

                // Create Day
                var day = new OfferDay
                {
                    Id = Guid.NewGuid(),
                    Date = dayDto.Date,
                    JobOfferId = offer.Id
                };

                // Create Shifts
                foreach (var shiftDto in dayDto.Shifts)
                {
                    var shift = new Shift
                    {
                        Id = Guid.NewGuid(),
                        StartTime = shiftDto.StartTime,
                        EndTime = shiftDto.EndTime,
                        IsAvailable = true
                    };

                    day.Shifts.Add(shift);                }

                offer.Days.Add(day);
            }

            //  Save
            await _offerRepo.AddAsync(offer);

            return offer.Id;
        }

        public async Task<JobOfferDetailsDto?> GetOfferByIdAsync(Guid id)
        {
            var offer = await _offerRepo.GetByIdAsync(id);

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
                Days = offer.Days.Select(d => new OfferDayDetailsDto
                {
                    DayId = d.Id,
                    Date = d.Date,
                    Shifts = d.Shifts.Select(s => new ShiftDetailsDto
                    {
                        ShiftId = s.Id,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        IsAvailable = s.IsAvailable
                    }).ToList()
                }).ToList()
            };
        }

        public async Task<List<JobOfferSummaryDto>> GetAllOffersAsync(int pageNumber,int pageSize)
        {
            var offers = await _offerRepo.GetPagedAsync(pageNumber,pageSize);

           
            return offers.Select(o => new JobOfferSummaryDto
            {
                Id = o.Id,
                Title = o.Title,
                Address = o.Address,
                HourlyRate = o.HourlyRate,
                AvailableDaysCount = o.Days.Count,
                AvailableShiftsCount = o.Days
                    .SelectMany(d => d.Shifts)
                    .Count(s => s.IsAvailable)
            }).ToList();
        }

        public async Task<bool> UpdateOfferAsync(
      Guid offerId,
      Guid careHomeId,
      UpdateJobOfferDto dto)
        {
            var offer = await _offerRepo.GetByIdAsync(offerId);

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
            if (dto.Days != null && dto.Days.Any())
            {
                foreach (var dayDto in dto.Days)
                {
                    // لو فيه DayId يبقى تعديل
                    if (dayDto.DayId.HasValue)
                    {
                        var existingDay = offer.Days
                            .FirstOrDefault(d => d.Id == dayDto.DayId.Value);

                        if (existingDay != null)
                        {
                            existingDay.Date = dayDto.Date;
                        }
                        else
                        {
                            // لو DayId جاي بس مش موجود في الداتابيز
                            // نمنع اللخبطة
                            throw new Exception("Day not found for update.");
                        }
                    }
                    else
                    {
                        // إضافة Day جديدة
                        var newDay = new OfferDay
                        {
                            Id = Guid.NewGuid(),
                            Date = dayDto.Date,
                            JobOfferId = offer.Id
                        };

                        offer.Days.Add(newDay);
                    }
                }
            }



            await _offerRepo.UpdateAsync();

            return true;
        }


        public async Task<bool> DeleteOfferAsync(Guid offerId, Guid careHomeId)
        {
            var offer = await _offerRepo.GetByIdAsync(offerId);

            if (offer == null)
                return false;

            if (offer.CareHomeId != careHomeId)
                throw new UnauthorizedAccessException("You cannot delete this offer.");

            await _offerRepo.DeleteAsync(offer);

            return true;
        }

    }
}

