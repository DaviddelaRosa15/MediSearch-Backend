using MediSearch.Core.Application.Dtos.Company;
using MediSearch.Core.Application.Dtos.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Features.Home.Queries.GetDataClient
{
    public class GetDataClientQueryResponse
    {
        public List<ProductHomeDTO> LastProducts { get; set; }
        public List<CompanyDTO> SameProvinceFarmacies { get; set; }
        public List<ProductHomeDTO> FavoriteProducts { get; set; }
        public List<CompanyDTO> FavoriteCompanies { get; set; }
    }
}
