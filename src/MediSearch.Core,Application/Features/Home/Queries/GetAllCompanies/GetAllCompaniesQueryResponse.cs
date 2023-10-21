using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Features.Home.Queries.GetAllCompanies
{
    public class GetAllCompaniesQueryResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UrlImage { get; set; }
        public string Type { get; set; }
    }
}
