using Shared.ApplicationDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.ServiceAbstraction.Interfaces.Applications
{
    public interface IApplicationManagementService
    {
        Task<List<OfferApplicationDto>>
            GetApplicationsForOfferAsync(Guid offerId, Guid careHomeId);

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
}
