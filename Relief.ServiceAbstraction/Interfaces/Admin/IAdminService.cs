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
        Task<List<AdminApplicationListDto>> GetPendingApplicationsAsync();
        Task ApproveApplicationAsync(Guid requestId);
        Task RejectApplicationAsync(Guid requestId, string reason);

        // Offers Monitoring
        Task<List<JobOfferSummaryDto>> GetAllOffersAsync();
    }
}
