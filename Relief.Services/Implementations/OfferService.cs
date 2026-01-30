using Relief.Domain.Entities;
using Relief.ServiceAbstraction.Interfaces;
using Shared.OffersDTOs.CreateDTO;
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

                    day.Shifts.Add(shift);
                }

                offer.Days.Add(day);
            }

            //  Save
            await _offerRepo.AddAsync(offer);

            return offer.Id;
        }


    }
}

