using Relief.Domain.Contracts;
using Relief.Domain.Entities;
using Relief.Domain.Entities.Offers;
using Relief.Domain.Enums;
using Relief.Domain.Exceptions;
using Relief.ServiceAbstraction.Interfaces;
using Relief.ServiceAbstraction.Interfaces.Applications;
using Relief.Services.Implementations.Applications.Specifications;
using Shared.ApplicationDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Relief.Services.Implementations.Applications
{
    public class ApplicationManagementService : IApplicationManagementService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ApplicationManagementService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // =====================================================
        // GET APPLICATIONS FOR OFFER (CareHome View)
        // Only shows Admin-approved applications
        // =====================================================
        public async Task<List<OfferApplicationDto>>
            GetApplicationsForOfferAsync(Guid offerId, Guid careHomeId)
        {
            var offerRepo = _unitOfWork.GetRepository<JobOffer, Guid>();
            var offer = await offerRepo.GetByIdAsync(offerId);

            if (offer == null)
                throw new NotFoundException("Offer not found.");

            if (offer.CareHomeId != careHomeId && offer.IndividualId != careHomeId)
                throw new ForbiddenException("You cannot view applications for this offer.");

            var requestRepo = _unitOfWork.GetRepository<JopRequest, Guid>();
            var spec = new OfferApplicationsSpecification(offerId);

            var requests = await requestRepo.GetAllAsync(spec);

            // CareHome only sees QualifiedByAdmin, Accepted, or RejectedByCareHome
            var filtered = requests.Where(r =>
                r.Status == RequestStatus.QualifiedByAdmin ||
                r.Status == RequestStatus.Accepted ||
                r.Status == RequestStatus.RejectedByCareHome);

            return filtered.Select(r =>
            {
                var user = r.PswUser.ApplicationUser;
                var today = DateTime.UtcNow;

                var age = today.Year - user.BirthOfDate.Year;

                if (user.BirthOfDate.Date > today.AddYears(-age))
                    age--;

                return new OfferApplicationDto
                {
                    JobRequestId = r.Id,
                    AppliedAt = r.CreatedAt,
                    Psw = new PswApplicationBriefDto
                    {
                        PswId = r.PswId,
                        FullName = $"{user.FirstName} {user.LastName}",
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Age = age,
                        IsVerified = r.PswUser.IsVerified,
                        WorkStatus = r.PswUser.WorkStatus,
                        ProofIdentityType = r.PswUser.ProofIdentityType,
                        CVFileId = r.PswUser.CVFileId
                    },
                    Shifts = r.Items.Select(i => new ShiftApplicationDto
                    {
                        JobRequestItemId = i.Id,
                        ShiftId = i.ShiftId,
                        Date = i.OfferShift.Date,
                        StartTime = i.OfferShift.StartTime,
                        EndTime = i.OfferShift.EndTime,
                        Status = r.Status // Use JopRequest status
                    }).ToList()
                };
            }).ToList();
        }

        // =====================================================
        // ACCEPT APPLICATION by CareHome (Atomic Operation)
        // Works on JopRequest.Status (not JobRequestItem)
        // =====================================================
        public async Task AcceptShiftAsync(
            Guid shiftId,
            Guid jobRequestItemId,
            Guid careHomeId)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var itemRepo = _unitOfWork.GetRepository<JobRequestItem, Guid>();

                var itemSpec =
                    new JobRequestItemWithDetailsSpecification(jobRequestItemId);

                var item = await itemRepo.GetByIdAsync(itemSpec);

                if (item == null)
                    throw new NotFoundException("Application item not found.");

                if (item.ShiftId != shiftId)
                    throw new BadRequestException("Shift mismatch.");

                var request = item.JopRequest;

                if (request == null)
                    throw new NotFoundException("Job request not found.");

                // Must be QualifiedByAdmin to be accepted by CareHome
                if (request.Status != RequestStatus.QualifiedByAdmin)
                    throw new ConflictException(
                        "Only admin-approved applications can be accepted.");

                var shift = item.OfferShift;

                if (shift == null)
                    throw new NotFoundException("Shift not found.");

                if (!shift.IsAvailable)
                    throw new ConflictException("Shift already assigned.");

                var offerRepo = _unitOfWork.GetRepository<JobOffer, Guid>();
                var offer = await offerRepo.GetByIdAsync(shift.JobOfferId);

                if (offer == null || (offer.CareHomeId != careHomeId && offer.IndividualId != careHomeId))
                    throw new ForbiddenException("You cannot manage this shift.");

                // Assign shift
                shift.AssignedPswId = request.PswId;
                shift.IsAvailable = false;

                // Update JopRequest status
                request.Status = RequestStatus.Accepted;

                // Reject other QualifiedByAdmin requests for the same offer
                var offerRequestsSpec = new OfferApplicationsSpecification(shift.JobOfferId);
                var requestRepo = _unitOfWork.GetRepository<JopRequest, Guid>();
                var allRequests = await requestRepo.GetAllAsync(offerRequestsSpec);

                foreach (var other in allRequests)
                {
                    if (other.Id != request.Id &&
                        other.Status == RequestStatus.QualifiedByAdmin)
                    {
                        other.Status = RequestStatus.RejectedByCareHome;
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                transaction.Complete();
            }
        }

        // =====================================================
        // REJECT APPLICATION by CareHome
        // Works on JopRequest.Status
        // =====================================================
        public async Task RejectShiftAsync(
            Guid jobRequestItemId,
            Guid careHomeId)
        {
            var itemRepo = _unitOfWork.GetRepository<JobRequestItem, Guid>();

            var itemSpec =
                new JobRequestItemWithDetailsSpecification(jobRequestItemId);

            var item = await itemRepo.GetByIdAsync(itemSpec);

            if (item == null)
                throw new NotFoundException("Application item not found.");

            var request = item.JopRequest;

            if (request == null)
                throw new NotFoundException("Job request not found.");

            // Must be QualifiedByAdmin to be rejected by CareHome
            if (request.Status != RequestStatus.QualifiedByAdmin)
                throw new ConflictException(
                    "Only admin-approved applications can be rejected by CareHome.");

            var shift = item.OfferShift;

            if (shift == null)
                throw new NotFoundException("Shift not found.");

            var offerRepo = _unitOfWork.GetRepository<JobOffer, Guid>();
            var offer = await offerRepo.GetByIdAsync(shift.JobOfferId);

            if (offer == null || (offer.CareHomeId != careHomeId && offer.IndividualId != careHomeId))
                throw new ForbiddenException("You cannot reject this application.");

            // Update JopRequest status
            request.Status = RequestStatus.RejectedByCareHome;

            await _unitOfWork.SaveChangesAsync();
        }

        // =====================================================
        // Cancel Application (PSW)
        // Works on JopRequest.Status
        // =====================================================
        public async Task CancelApplicationAsync(
                Guid jobRequestItemId,
                Guid pswId)
        {
            var itemRepo = _unitOfWork.GetRepository<JobRequestItem, Guid>();

            var spec = new JobRequestItemWithOwnerSpecification(jobRequestItemId);
            var item = await itemRepo.GetByIdAsync(spec);

            if (item == null)
                throw new NotFoundException("Application item not found.");

            var request = item.JopRequest;

            if (request.PswId != pswId)
                throw new ForbiddenException("You cannot cancel this application.");

            if (request.Status == RequestStatus.Accepted)
                throw new ConflictException("Accepted application cannot be cancelled.");

            if (request.Status == RequestStatus.RejectedByAdmin ||
                request.Status == RequestStatus.RejectedByCareHome)
                throw new ConflictException("Application already rejected.");

            // Update JopRequest status
            request.Status = RequestStatus.Canceled;

            await _unitOfWork.SaveChangesAsync();
        }

        // =====================================================
        // PSW View Applications
        // Uses JopRequest.Status
        // =====================================================
        public async Task<List<PswApplicationViewDto>>
                     GetPswApplicationsAsync(Guid pswId)
        {
            var requestRepo = _unitOfWork.GetRepository<JopRequest, Guid>();

            var spec = new PswApplicationsSpecification(pswId);
            var requests = await requestRepo.GetAllAsync(spec);

            return requests.SelectMany(r => r.Items.Select(i => new PswApplicationViewDto
            {
                JobRequestId = r.Id,
                JobRequestItemId = i.Id,
                ShiftId = i.ShiftId,
                OfferTitle = i.OfferShift.JobOffer.Title,
                Date = i.OfferShift.Date,
                StartTime = i.OfferShift.StartTime,
                EndTime = i.OfferShift.EndTime,
                Status = r.Status // Use JopRequest status
            })).ToList();
        }

        // =====================================================
        // Get Applications For CareHome
        // Only shows Admin-approved applications
        // =====================================================
        public async Task<List<OfferApplicationDto>> GetApplicationsForCareHomeAsync(Guid careHomeId)
        {
            var offerRepo = _unitOfWork.GetRepository<JobOffer, Guid>();

            var offers = await offerRepo.GetAllAsync();

            var ownedOffers = offers
                .Where(o => o.CareHomeId == careHomeId || o.IndividualId == careHomeId)
                .Select(o => o.Id)
                .ToList();

            if (!ownedOffers.Any())
                return new List<OfferApplicationDto>();

            var requestRepo = _unitOfWork.GetRepository<JopRequest, Guid>();

            var spec = new CareHomeApplicationsSpecification(ownedOffers);

            var requests = await requestRepo.GetAllAsync(spec);

            // Filter to only Admin-approved requests
            var filtered = requests.Where(r =>
                r.Status == RequestStatus.QualifiedByAdmin ||
                r.Status == RequestStatus.Accepted ||
                r.Status == RequestStatus.RejectedByCareHome);

            return filtered.Select(r =>
            {
                var user = r.PswUser.ApplicationUser;

                var today = DateTime.UtcNow;
                var age = today.Year - user.BirthOfDate.Year;

                if (user.BirthOfDate.Date > today.AddYears(-age))
                    age--;

                return new OfferApplicationDto
                {
                    JobRequestId = r.Id,
                    AppliedAt = r.CreatedAt,

                    Psw = new PswApplicationBriefDto
                    {
                        PswId = r.PswId,
                        FullName = $"{user.FirstName} {user.LastName}",
                        Age = age,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        IsVerified = r.PswUser.IsVerified,
                        WorkStatus = r.PswUser.WorkStatus,
                        ProofIdentityType = r.PswUser.ProofIdentityType,
                        CVFileId = r.PswUser.CVFileId
                    },

                    Shifts = r.Items.Select(i => new ShiftApplicationDto
                    {
                        JobRequestItemId = i.Id,
                        ShiftId = i.ShiftId,
                        Date = i.OfferShift.Date,
                        StartTime = i.OfferShift.StartTime,
                        EndTime = i.OfferShift.EndTime,
                        Status = r.Status // Use JopRequest status
                    }).ToList()
                };
            }).ToList();
        }
    }
}
