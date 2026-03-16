using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.QueryDTOs.JobOffer
{
    public class JobOfferQueryParams : BaseQueryParams
    {
        public Guid? CareHomeId { get; set; }
    }
}
