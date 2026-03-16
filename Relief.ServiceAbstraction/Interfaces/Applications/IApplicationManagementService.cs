using Shared.ApplicationDTO;
using Shared.QueryDTOs;
using Shared.QueryDTOs.CareHome;
using Shared.QueryDTOs.Psw;

public interface IApplicationManagementService
{
    Task<List<OfferApplicationDto>>
        GetApplicationsForOfferAsync(Guid offerId, Guid careHomeId);

    Task<Pagination<OfferApplicationDto>>
        GetApplicationsForCareHomeAsync(Guid careHomeId, CareHomeApplicationQueryParams query);

    Task AcceptShiftAsync(
        Guid shiftId,
        Guid jobRequestItemId,
        Guid careHomeId);

    Task RejectShiftAsync(
        Guid jobRequestItemId,
        Guid careHomeId);

    Task<Pagination<PswApplicationViewDto>> GetPswApplicationsAsync(Guid pswId, PswApplicationQueryParams query);

    Task CancelApplicationAsync(Guid jobRequestItemId, Guid pswId);
}