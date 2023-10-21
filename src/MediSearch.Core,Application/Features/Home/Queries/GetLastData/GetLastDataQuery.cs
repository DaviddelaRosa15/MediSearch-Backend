using MediatR;
using MediSearch.Core.Application.Dtos.Company;
using MediSearch.Core.Application.Dtos.Product;
using MediSearch.Core.Application.Interfaces.Repositories;
using MediSearch.Core.Domain.Entities;

namespace MediSearch.Core.Application.Features.Home.Queries.GetLastData
{
    public class GetLastDataQuery : IRequest<GetLastDataQueryResponse>
    {

    }

    public class GetLastDataQueryHandler : IRequestHandler<GetLastDataQuery, GetLastDataQueryResponse>
    {
        private readonly ICompanyRepository _companyRepository;
        public GetLastDataQueryHandler(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<GetLastDataQueryResponse> Handle(GetLastDataQuery query, CancellationToken cancellationToken)
        {
            GetLastDataQueryResponse response = new();
            var companies = await _companyRepository.GetAllWithIncludeAsync(new List<string>() { "Products", "CompanyType" });
            var farmacies = companies.FindAll(c => c.CompanyType.Name == "Farmacia");
            var laboratories = companies.FindAll(c => c.CompanyType.Name == "Laboratorio");

            List<CompanyDTO> lastFarmacies = new();
            List<CompanyDTO> lastLaboratories = new();
            List<ProductHomeDTO> lastProductsFarmacies = new();
            List<ProductHomeDTO> lastProductsLaboratories = new();
            List<Domain.Entities.Product> productsFarmacy = new(); 
            List<Domain.Entities.Product> productsLaboratory = new(); 

            foreach (var item in farmacies.OrderBy(f => f.Created))
            {
                CompanyDTO dto = new()
                {
                    Id = item.Id,
                    Name = item.Name,
                    UrlImage = item.UrlImage,
                    Phone = item.Phone,
                    Province = item.Province,
                    Municipality = item.Municipality,
                    Address = item.Address,
                    IsFavorite = false
                };

                foreach (var product in item.Products)
                {
                    productsFarmacy.Add(product);
                }

                lastFarmacies.Add(dto);
            }

            foreach (var item in laboratories.OrderBy(l => l.Created))
            {
                CompanyDTO dto = new()
                {
                    Id = item.Id,
                    Name = item.Name,
                    UrlImage = item.UrlImage,
                    Phone = item.Phone,
                    Province = item.Province,
                    Municipality = item.Municipality,
                    Address = item.Address,
                    IsFavorite = false
                };

                foreach (var product in item.Products)
                {
                    productsLaboratory.Add(product);
                }

                lastLaboratories.Add(dto);
            }

            foreach (var item in productsFarmacy.OrderBy(p => p.Created))
            {
                var company = await _companyRepository.GetByIdAsync(item.CompanyId);

                ProductHomeDTO dto = new()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    UrlImages = item.UrlImages,
                    Classification = item.Classification,
                    Categories = item.Categories,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Available = item.Quantity > 0,
                    IsFavorite = false,
                    CompanyId = item.CompanyId,
                    NameCompany = company.Name,
                    Province = company.Province,
                    Municipality = company.Municipality,
                    Address = company.Address
                };

                lastProductsFarmacies.Add(dto);
            }

            foreach (var item in productsLaboratory.OrderBy(p => p.Created))
            {
                var company = await _companyRepository.GetByIdAsync(item.CompanyId);

                ProductHomeDTO dto = new()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    UrlImages = item.UrlImages,
                    Classification = item.Classification,
                    Categories = item.Categories,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Available = item.Quantity > 0,
                    IsFavorite = false,
                    CompanyId = item.CompanyId,
                    NameCompany = company.Name,
                    Province = company.Province,
                    Municipality = company.Municipality,
                    Address = company.Address
                };

                lastProductsLaboratories.Add(dto);
            }

            response.LastFarmacies = lastFarmacies.Take(10).ToList();
            response.LastLaboratories = lastLaboratories.Take(10).ToList();
            response.LastProductsFarmacies = lastProductsFarmacies.Take(10).ToList();
            response.LastProductsLaboratories = lastProductsLaboratories.Take(10).ToList();

            return response;
        }
    }
}
