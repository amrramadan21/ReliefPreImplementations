using Shared.AdminDTOs;
using Shared.OffersDTOs.OfferInfoDTO;
using Shared.QueryDTOs;
using Shared.QueryDTOs.Admin;

namespace Relief.ServiceAbstraction.Interfaces.Admin
{
    public interface IAdminService
    {
        // PSW Verification
        Task<Pagination<PswVerificationListDto>> GetPendingVerificationsAsync(PendingVerificationQueryParams query);
        Task ApproveVerificationAsync(Guid pswId);
        Task RejectVerificationAsync(Guid pswId, string reason);

        // Application Review
        Task<Pagination<AdminApplicationListDto>> GetApplicationsByStatusAsync(AdminApplicationQueryParams query);
        Task<List<UserListDto>> GetUsersByRoleAsync(string? role);
        Task<Pagination<PswListDto>> GetAllPswUsersAsync(AdminPswQueryParams query);
        Task ApproveApplicationAsync(Guid requestId);
        Task RejectApplicationAsync(Guid requestId, string reason);

        // Offers Monitoring
        Task<Pagination<JobOfferDetailsDto>> GetAllOffersAsync(AdminOfferQueryParams query);
    }
}
