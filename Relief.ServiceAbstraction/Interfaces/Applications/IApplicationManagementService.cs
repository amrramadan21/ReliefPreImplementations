using Shared.ApplicationDTO;

public interface IApplicationManagementService
{
    Task<List<OfferApplicationDto>>
        GetApplicationsForOfferAsync(Guid offerId, Guid careHomeId);

    Task<List<OfferApplicationDto>>
        GetApplicationsForCareHomeAsync(Guid careHomeId);

    Task AcceptShiftAsync(
        Guid shiftId,
        Guid jobRequestItemId,
        Guid careHomeId);

    Task RejectShiftAsync(
        Guid jobRequestItemId,
        Guid careHomeId);

    Task<List<PswApplicationViewDto>>
        GetPswApplicationsAsync(Guid pswId);

    Task CancelApplicationAsync(Guid jobRequestItemId, Guid pswId);
}