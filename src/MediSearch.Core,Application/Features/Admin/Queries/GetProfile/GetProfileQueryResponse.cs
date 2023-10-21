using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Features.Admin.Queries.GetProfile
{
    public class GetProfileQueryResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UrlImage { get; set; }
        public string Province { get; set; }
        public string Municipality { get; set; }
        public string Address { get; set; }
    }
}
