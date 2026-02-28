using Relief.Domain.Contracts;
using Relief.Domain.Entities;
using Relief.Domain.Entities.Users;
using Relief.Domain.Enums;
using Relief.Domain.Exceptions;
using Relief.ServiceAbstraction.Interfaces;
using Shared.ApplyDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations
{
  

    public class ApplyService : IApplyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ApplyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ApplyAsync(Guid pswId, ApplyToOfferDto dto)
        {
            var pswRepo = _unitOfWork.GetRepository<PswUser, Guid>();
            var shiftRepo = _unitOfWork.GetRepository<OfferShift, Guid>();
            var requestRepo = _unitOfWork.GetRepository<JopRequest, Guid>();

            // =========================
            // Validate PSW
            // =========================
            var psw = await pswRepo.GetByIdAsync(pswId);

            if (psw == null)
                throw new NotFoundException("PSW not found.");

            if (!psw.IsProfileCompleted)
                throw new ForbiddenException("Please complete your profile before applying.");

            if (!psw.IsVerified)
                throw new ForbiddenException("Your profile is not verified yet.");

            if (dto.ShiftIds == null || !dto.ShiftIds.Any())
                throw new BadRequestException("At least one shift must be selected.");

            // =========================
            // Prevent Duplicate Apply
            // =========================
            var existingRequests = (await requestRepo.GetAllAsync())
                .Where(r => r.PswId == pswId && r.JobOfferId == dto.OfferId && r.Status == RequestStatus.Pending);

            if (existingRequests.Any())
                throw new ConflictException("You have already applied for this offer.");

            // =========================
            // Create Request
            // =========================
            var request = new JopRequest
            {
                Id = Guid.NewGuid(),
                PswId = pswId,
                JobOfferId = dto.OfferId,
                Status = RequestStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                Items = new List<JobRequestItem>()
            };

            foreach (var shiftId in dto.ShiftIds)
            {
                var shift = await shiftRepo.GetByIdAsync(shiftId);

                if (shift == null)
                    throw new NotFoundException($"Shift with id {shiftId} not found.");

                if (shift.JobOfferId != dto.OfferId)
                    throw new BadRequestException("Selected shift does not belong to this offer.");

                if (!shift.IsAvailable)
                    throw new ConflictException("One of the selected shifts is already booked.");

                var item = new JobRequestItem
                {
                    Id = Guid.NewGuid(),
                    JopRequestId = request.Id,
                    ShiftId = shiftId,
                    Status = RequestStatus.Pending
                };

                request.Items.Add(item);
            }

            await requestRepo.AddAsync(request);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
