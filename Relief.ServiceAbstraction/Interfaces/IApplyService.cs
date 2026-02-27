using Shared.ApplyDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.ServiceAbstraction.Interfaces
{
    public interface IApplyService
    {
        Task ApplyAsync(Guid pswId, ApplyToOfferDto dto);
    }
}
