using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Relief.Domain.Contracts;
using Relief.Domain.Entities.Offers;
using Relief.Domain.Entities.Users;
using Relief.Domain.Enums;
using Relief.Domain.Exceptions;
using Relief.ServiceAbstraction.Interfaces.Admin;
using Shared.AdminDTOs;
using Shared.OffersDTOs.OfferInfoDTO;

namespace Relief.Services.Implementations.Admin
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminService(
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        // =====================================================
        // PSW VERIFICATION — GET PENDING
        // =====================================================
        public async Task<List<PswVerificationListDto>> GetPendingVerificationsAsync()
        {
            var pswRepo = _unitOfWork.GetRepository<PswUser, Guid>();
            var allPsws = await pswRepo.GetAllAsync();

            var pending = allPsws
                .Where(p => p.IsProfileCompleted &&
                            p.VerificationStatus == VerificationStatus.Pending)
                .ToList();

            var result = new List<PswVerificationListDto>();

            foreach (var psw in pending)
            {
                var user = await _userManager.FindByIdAsync(
                    psw.ApplicationUserId.ToString());

                if (user == null) continue;

                result.Add(new PswVerificationListDto
                {
                    PswUserId = psw.ApplicationUserId,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Email = user.Email ?? "",
                    ProofIdentityType = psw.ProofIdentityType,
                    VerificationStatus = psw.VerificationStatus,
                    RejectionReason = psw.VerificationRejectionReason,
                    ProfileCompletedAt = DateTime.UtcNow,
                    ProofIdentityFileId = psw.ProofIdentityFileId,
                    PswCertificateFileId = psw.PswCertificateFileId,
                    CVFileId = psw.CVFileId,
                    ImmunizationRecordFileId = psw.ImmunizationRecordFileId,
                    CriminalRecordFileId = psw.CriminalRecordFileId,
                    FirstAidOrCPRFileId = psw.FirstAidOrCPRFileId
                });
            }

            return result;
        }

        // =====================================================
        // PSW VERIFICATION — APPROVE
        // =====================================================
        public async Task ApproveVerificationAsync(Guid pswId)
        {
            var pswRepo = _unitOfWork.GetRepository<PswUser, Guid>();
            var psw = await pswRepo.GetByIdAsync(pswId);

            if (psw == null)
                throw new NotFoundException("PSW not found.");

            if (!psw.IsProfileCompleted)
                throw new BadRequestException("PSW has not completed their profile.");

            if (psw.VerificationStatus == VerificationStatus.Approved)
                throw new ConflictException("PSW is already verified.");

            psw.VerificationStatus = VerificationStatus.Approved;
            psw.IsVerified = true;
            psw.VerificationRejectionReason = null;

            pswRepo.Update(psw);
            await _unitOfWork.SaveChangesAsync();
        }

        // =====================================================
        // PSW VERIFICATION — REJECT
        // =====================================================
        public async Task RejectVerificationAsync(Guid pswId, string reason)
        {
            var pswRepo = _unitOfWork.GetRepository<PswUser, Guid>();
            var psw = await pswRepo.GetByIdAsync(pswId);

            if (psw == null)
                throw new NotFoundException("PSW not found.");

            if (!psw.IsProfileCompleted)
                throw new BadRequestException("PSW has not completed their profile.");

            psw.VerificationStatus = VerificationStatus.Rejected;
            psw.IsVerified = false;
            psw.VerificationRejectionReason = reason;

            pswRepo.Update(psw);
            await _unitOfWork.SaveChangesAsync();
        }

        // =====================================================
        // APPLICATIONS — GET PENDING
        // =====================================================
        public async Task<List<AdminApplicationListDto>> GetPendingApplicationsAsync()
        {
            var requestRepo = _unitOfWork.GetRepository<JopRequest, Guid>();
            var allRequests = await requestRepo.GetAllAsync();

            var pending = allRequests
                .Where(r => r.Status == RequestStatus.Pending)
                .ToList();

            var result = new List<AdminApplicationListDto>();

            foreach (var req in pending)
            {
                var offerRepo = _unitOfWork.GetRepository<JobOffer, Guid>();
                var offer = await offerRepo.GetByIdAsync(req.JobOfferId);

                var pswRepo = _unitOfWork.GetRepository<PswUser, Guid>();
                var psw = await pswRepo.GetByIdAsync(req.PswId);

                var user = psw != null
                    ? await _userManager.FindByIdAsync(psw.ApplicationUserId.ToString())
                    : null;

                result.Add(new AdminApplicationListDto
                {
                    JobRequestId = req.Id,
                    OfferId = req.JobOfferId,
                    OfferTitle = offer?.Title ?? "",
                    Status = req.Status,
                    AppliedAt = req.CreatedAt,
                    RejectionReason = req.RejectionReason,
                    PswId = req.PswId,
                    PswFullName = user != null
                        ? $"{user.FirstName} {user.LastName}"
                        : "",
                    IsVerified = psw?.IsVerified ?? false,
                    ShiftCount = req.Items.Count
                });
            }

            return result;
        }

        // =====================================================
        // APPLICATIONS — APPROVE
        // =====================================================
        public async Task ApproveApplicationAsync(Guid requestId)
        {
            var requestRepo = _unitOfWork.GetRepository<JopRequest, Guid>();
            var request = await requestRepo.GetByIdAsync(requestId);

            if (request == null)
                throw new NotFoundException("Application not found.");

            if (request.Status != RequestStatus.Pending)
                throw new ConflictException("Only pending applications can be approved.");

            request.Status = RequestStatus.QualifiedByAdmin;
            request.RejectionReason = null;

            requestRepo.Update(request);
            await _unitOfWork.SaveChangesAsync();
        }

        // =====================================================
        // APPLICATIONS — REJECT
        // =====================================================
        public async Task RejectApplicationAsync(Guid requestId, string reason)
        {
            var requestRepo = _unitOfWork.GetRepository<JopRequest, Guid>();
            var request = await requestRepo.GetByIdAsync(requestId);

            if (request == null)
                throw new NotFoundException("Application not found.");

            if (request.Status != RequestStatus.Pending)
                throw new ConflictException("Only pending applications can be rejected.");

            request.Status = RequestStatus.RejectedByAdmin;
            request.RejectionReason = reason;

            requestRepo.Update(request);
            await _unitOfWork.SaveChangesAsync();
        }

        // =====================================================
        // OFFERS — GET ALL (MONITORING)
        // =====================================================
        public async Task<List<JobOfferSummaryDto>> GetAllOffersAsync()
        {
            var offerRepo = _unitOfWork.GetRepository<JobOffer, Guid>();
            var offers = await offerRepo.GetAllAsync();

            return offers.Select(o => new JobOfferSummaryDto
            {
                Id = o.Id,
                Title = o.Title,
                Address = o.Address,
                HourlyRate = o.HourlyRate,
                Latitude = o.Latitude,
                Longitude = o.Longitude
            }).ToList();
        }
    }
}
