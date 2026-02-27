using Relief.Domain.Contracts;
using Relief.Domain.Entities;
using Relief.Domain.Entities.Users;
using Relief.Domain.Enums;
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
            var itemRepo = _unitOfWork.GetRepository<JobRequestItem, Guid>();

            var psw = await pswRepo.GetByIdAsync(pswId);

            if (psw == null)
                throw new Exception("PSW not found");

            if (!psw.IsProfileCompleted || !psw.IsVerified)
                throw new Exception("Profile not verified");

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
                    throw new Exception("Shift not found");

                if (!shift.IsAvailable)
                    throw new Exception("Shift already booked");

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
