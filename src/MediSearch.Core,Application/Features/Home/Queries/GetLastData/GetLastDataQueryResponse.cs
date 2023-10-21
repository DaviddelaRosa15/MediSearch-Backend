using MediSearch.Core.Application.Dtos.Company;
using MediSearch.Core.Application.Dtos.Product;

namespace MediSearch.Core.Application.Features.Home.Queries.GetLastData
{
    public class GetLastDataQueryResponse
    {
        public List<CompanyDTO> LastFarmacies { get; set; }
        public List<CompanyDTO> LastLaboratories { get; set; }
        public List<ProductHomeDTO> LastProductsLaboratories { get; set; }
        public List<ProductHomeDTO> LastProductsFarmacies { get; set; }
    }
}
