using Relief.Domain.Entities.Offers;
using Shared.ApplyDTOs;
using Shared.QueryDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.ServiceAbstraction.Interfaces.Applications
{
    public interface IApplyService
    {
        Task ApplyAsync(Guid pswId, ApplyToOfferDto dto);
       
    }
}
