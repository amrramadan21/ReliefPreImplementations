using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Relief.Domain.Contracts;
using Relief.Domain.Entities.Offers;
using Relief.Domain.Entities.Users;
using Relief.Domain.Enums;
using Relief.Domain.Exceptions;
using Relief.ServiceAbstraction.Interfaces.Admin;
using Relief.Services.Implementations.Admin.Specifications;
using Relief.Services.Implementations.Admin.Specifications.Pagination;
using Relief.Services.Implementations.Offers.Specifications;
using Shared.AdminDTOs;
using Shared.ApplicationDTO;
using Shared.OffersDTOs.OfferInfoDTO;
using Shared.QueryDTOs;
using Shared.QueryDTOs.Admin;
using System;

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
        public async Task<Pagination<PswVerificationListDto>> GetPendingVerificationsAsync(
               PendingVerificationQueryParams query)
        {
            var pswRepo = _unitOfWork.GetRepository<PswUser, Guid>();

            var spec = new PendingVerificationPaginatedSpecification(query);
            var countSpec = new PendingVerificationCountSpecification();

            var pending = await pswRepo.GetAllAsync(spec);
            var totalCount = await pswRepo.CountAsync(countSpec);

            var dtos = pending.Select(psw => new PswVerificationListDto
            {
                PswUserId = psw.ApplicationUserId,
                FullName = $"{psw.ApplicationUser.FirstName} {psw.ApplicationUser.LastName}",
                Email = psw.ApplicationUser.Email ?? "",
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
            }).ToList();

            return new Pagination<PswVerificationListDto>(
                query.PageIndex,
                query.PageSize,
                totalCount,
                dtos);
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

            //if (!psw.IsProfileCompleted)
            //    throw new BadRequestException("PSW has not completed their profile.");

            if (psw.VerificationStatus == VerificationStatus.Approved)
                throw new ConflictException("PSW is already verified.");

            psw.VerificationStatus = VerificationStatus.Approved;
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

            if (psw.VerificationStatus != VerificationStatus.Approved)
                throw new BadRequestException("PSW has not completed their profile.");

            psw.VerificationStatus = VerificationStatus.Rejected;
            psw.VerificationRejectionReason = reason;

            pswRepo.Update(psw);
            await _unitOfWork.SaveChangesAsync();
        }

        // =====================================================
        // APPLICATIONS — GET By status
        // =====================================================
        public async Task<Pagination<AdminApplicationListDto>> GetApplicationsByStatusAsync(
           AdminApplicationQueryParams query)
        {
            RequestStatus? requestStatus = null;

            if (!string.IsNullOrEmpty(query.Status))
            {
                if (Enum.TryParse<RequestStatus>(query.Status, true, out var parsedStatus))
                {
                    requestStatus = parsedStatus;
                }
                else
                {
                    return new Pagination<AdminApplicationListDto>(
                        query.PageIndex, query.PageSize, 0, new List<AdminApplicationListDto>());
                }
            }

            var requestRepo = _unitOfWork.GetRepository<JopRequest, Guid>();

            var spec = new AdminApplicationPaginatedSpecification(query, requestStatus);
            var countSpec = new AdminApplicationCountSpecification(requestStatus);

            var requests = await requestRepo.GetAllAsync(spec);
            var totalCount = await requestRepo.CountAsync(countSpec);

            var dtos = requests.Select(req =>
            {
                var user = req.PswUser.ApplicationUser;

                return new AdminApplicationListDto
                {
                    JobRequestId = req.Id,
                    OfferId = req.JobOfferId,
                    OfferTitle = req.JobOffer?.Title ?? "",
                    Status = req.Status,
                    AppliedAt = req.CreatedAt,
                    RejectionReason = req.RejectionReason,
                    PswId = req.PswId,
                    PswFullName = user != null
                        ? $"{user.FirstName} {user.LastName}"
                        : "",
                    PswPhone = user?.PhoneNumber ?? "",
                    PswEmail = user?.Email ?? "",
                    VerificationStatus = req.PswUser.VerificationStatus.ToString(),
                    VerificationReason = req.PswUser.VerificationRejectionReason,
                    Shifts = req.Items.Select(i => new ShiftApplicationDto
                    {
                        JobRequestItemId = i.Id,
                        ShiftId = i.OfferShift.Id,
                        Date = i.OfferShift.Date,
                        StartTime = i.OfferShift.StartTime,
                        EndTime = i.OfferShift.EndTime,
                        Status = i.Status
                    }).ToList()
                };
            }).ToList();

            return new Pagination<AdminApplicationListDto>(
                query.PageIndex,
                query.PageSize,
                totalCount,
                dtos);
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
        public async Task<Pagination<JobOfferDetailsDto>> GetAllOffersAsync(AdminOfferQueryParams query)
        {
            var offerRepo = _unitOfWork.GetRepository<JobOffer, Guid>();

            var spec = new AdminOfferPaginatedSpecification(query);
            var countSpec = new AdminOfferCountSpecification();

            var offers = await offerRepo.GetAllAsync(spec);
            var totalCount = await offerRepo.CountAsync(countSpec);

            var dtos = offers.Select(o =>
            {
                // Get owner from whichever navigation is not null
                var owner = o.CareHomeUser?.ApplicationUser
                         ?? o.IndividualCareHomeUser?.ApplicationUser;
                var role = o.CareHomeUser != null ? "Care Home" : o.IndividualCareHomeUser != null ? "Individual" : "Unknown";
                return new JobOfferDetailsDto
                {
                    Id = o.Id,
                    Title = o.Title,
                    Position = o.Position,
                    Description = o.Description,
                    PosterId = owner?.Id,
                    PosterName = GetPosterName(o) ?? "Unknown",
                    PosterType = role,
                    Address2 = o.Address2,
                    City = o.City,
                    PostalCode = o.PostalCode,
                    Province = o.Province,
                    Preferences = new List<string>(o.Preferences ?? Enumerable.Empty<string>()),
                    Address = o.Address,
                    HourlyRate = o.HourlyRate,
                    Latitude = o.Latitude,
                    Longitude = o.Longitude,
                    Shifts = o.Shifts.Select(s => new OfferShiftDetailsDto
                    {
                        ShiftId = s.Id,
                        Date = s.Date,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        IsAvailable = s.IsAvailable
                    }).ToList()
                };
            }).ToList();

            return new Pagination<JobOfferDetailsDto>(
                query.PageIndex,
                query.PageSize,
                totalCount,
                dtos);
        }

        public async Task<List<UserListDto>> GetUsersByRoleAsync(string? role)
        {
            List<ApplicationUser> users;

            // 1. Fetch users based on whether a role was provided
            if (!string.IsNullOrWhiteSpace(role))
            {
                // Use Identity's built-in method to find users by role
                var usersInRole = await _userManager.GetUsersInRoleAsync(role);
                users = usersInRole.ToList();
            }
            else
            {
                // Fallback: Get all users if no role filter is applied
                users = _userManager.Users.ToList();
            }

            var result = new List<UserListDto>();

            // 2. Map the entities to your DTO
            foreach (var user in users)
            {
                // If you need to attach the exact role to the DTO, fetch it.
                // Note: If a user has multiple roles, you might want to adjust this.
                var userRoles = await _userManager.GetRolesAsync(user);
                var primaryRole = userRoles.FirstOrDefault();

                result.Add(new UserListDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Gender = user.Gender.ToString(),
                    Role = primaryRole ?? "No Role"
                });
            }

            return result;
        }

        public async Task<Pagination<PswListDto>> GetAllPswUsersAsync(AdminPswQueryParams query)
        {
            var pswRepo = _unitOfWork.GetRepository<PswUser, Guid>();

            var spec = new PswUserPaginatedSpecification(query);
            var countSpec = new PswUserCountSpecification(query);

            var pswUsers = await pswRepo.GetAllAsync(spec);
            var totalCount = await pswRepo.CountAsync(countSpec);

            var dtos = pswUsers.Select(psw => new PswListDto
            {
                Id = psw.ApplicationUserId,
                FirstName = psw.ApplicationUser.FirstName,
                LastName = psw.ApplicationUser.LastName,
                Email = psw.ApplicationUser.Email,
                PhoneNumber = psw.ApplicationUser.PhoneNumber,
                Gender = psw.ApplicationUser.Gender.ToString(),
                VerificationStatus = psw.VerificationStatus,
                VerificationRejectionReason = psw.VerificationRejectionReason,
                Role = "PSW"
            }).ToList();

            return new Pagination<PswListDto>(
                query.PageIndex,
                query.PageSize,
                totalCount,
                dtos);
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
