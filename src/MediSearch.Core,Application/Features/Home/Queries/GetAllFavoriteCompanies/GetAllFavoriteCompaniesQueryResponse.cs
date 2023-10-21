using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Features.Home.Queries.GetAllFavoriteCompanies
{
    public class GetAllFavoriteCompaniesQueryResponse
    {
        public string Id { get; set; }
        public string CompanyId { get; set; }
        public string Name { get; set; }
        public string UrlImage { get; set; }
        public string Phone { get; set; }
        public string Province { get; set; }
        public string Municipality { get; set; }
        public string Address { get; set; }
        public bool IsFavorite { get; set; }
    }
}
