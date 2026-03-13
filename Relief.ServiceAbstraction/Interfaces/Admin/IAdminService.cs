using Shared.AdminDTOs;
using Shared.OffersDTOs.OfferInfoDTO;

namespace Relief.ServiceAbstraction.Interfaces.Admin
{
    public interface IAdminService
    {
        // PSW Verification
        Task<List<PswVerificationListDto>> GetPendingVerificationsAsync();
        Task ApproveVerificationAsync(Guid pswId);
        Task RejectVerificationAsync(Guid pswId, string reason);

        // Application Review
        Task<List<AdminApplicationListDto>> GetApplicationsByStatusAsync(string? status = null);
        Task<List<UserListDto>> GetUsersByRoleAsync(string? role);
        Task<List<PswListDto>> GetAllPswUsersAsync();
        Task ApproveApplicationAsync(Guid requestId);
        Task RejectApplicationAsync(Guid requestId, string reason);

        // Offers Monitoring
        Task<List<JobOfferDetailsDto>> GetAllOffersAsync();
    }
}
