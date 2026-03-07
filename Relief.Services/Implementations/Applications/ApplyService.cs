using Relief.Domain.Contracts;
using Relief.Domain.Entities.Offers;
using Relief.Domain.Entities.Users;
using Relief.Domain.Enums;
using Relief.Domain.Exceptions;
using Relief.ServiceAbstraction.Interfaces.Applications;
using Relief.Services.Common;
using Relief.Services.Specifications;
using Shared.ApplyDTOs;
using Shared.QueryDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations.Applications
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
            var spec = new PswOfferApplicationSpecification(pswId, dto.OfferId);

            var existingRequests = await requestRepo.GetAllAsync(spec);

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
                CreatedAt = DateTime.UtcNow,
                Items = new List<JobRequestItem>()
            };

            var offerRepo = _unitOfWork.GetRepository<JobOffer, Guid>();

            var offer = await offerRepo.GetByIdAsync(dto.OfferId);

            if (offer == null)
                throw new NotFoundException("Offer not found.");

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

        // =========================
        // Request Query with Pagination & Filtering
        // ========================= 
        public async Task<Pagination<JopRequest>> GetRequestsAsync(
                        Guid careHomeId,
                        RequestQueryParams query)
        {
            var repo = _unitOfWork.GetRepository<JopRequest, Guid>();

            Expression<Func<JopRequest, bool>> criteria =
                r => r.JobOffer.CareHomeId == careHomeId;

            var countSpec = new PaginationSpecification<JopRequest, Guid>(criteria);

            var totalCount = await repo.CountAsync(countSpec);

            PaginationSpecification<JopRequest, Guid> spec;

            // dynamic sorting
            if (query.Sort == "-createdAt")
            {
                spec = new PaginationSpecification<JopRequest, Guid>(
                    criteria,
                    r => r.CreatedAt,
                    desc: true,
                    query.PageIndex,
                    query.PageSize
                );
            }
            else
            {
                spec = new PaginationSpecification<JopRequest, Guid>(
                    criteria,
                    r => r.CreatedAt,
                    query.PageIndex,
                    query.PageSize
                );
            }

            var data = await repo.GetAllAsync(spec);

            return new Pagination<JopRequest>(
                query.PageIndex,
                query.PageSize,
                totalCount,
                data.ToList());
        }
    }
}
