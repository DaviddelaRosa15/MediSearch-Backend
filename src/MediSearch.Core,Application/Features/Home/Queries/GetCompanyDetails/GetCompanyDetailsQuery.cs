using AutoMapper;
using MediatR;
using MediSearch.Core.Application.Dtos.Company;
using MediSearch.Core.Application.Dtos.Product;
using MediSearch.Core.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Features.Home.Queries.GetCompanyDetails
{
    public class GetCompanyDetailsQuery : IRequest<CompanyDetailsDTO>
    {
        public string Id { get; set; }
        public string? UserId { get; set; }
    }

    public class GetCompanyDetailsQueryHandler : IRequestHandler<GetCompanyDetailsQuery, CompanyDetailsDTO>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IFavoriteCompanyRepository _favoriteCompanyRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public GetCompanyDetailsQueryHandler(ICompanyRepository companyRepository, IProductRepository productRepository, IMapper mapper, IFavoriteCompanyRepository favoriteCompanyRepository)
        {
            _companyRepository = companyRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _favoriteCompanyRepository = favoriteCompanyRepository;
        }

        public async Task<CompanyDetailsDTO> Handle(GetCompanyDetailsQuery query, CancellationToken cancellationToken)
        {
            CompanyDetailsDTO result = new();
            var company = await _companyRepository.GetByIdAsync(query.Id);
            result = _mapper.Map<CompanyDetailsDTO>(company);

            if(company != null)
            {
                var products = await _productRepository.GetProductsByCompany(company.Id);
                result.Products = products.Select(p => new ProductHomeDTO()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    UrlImages = p.UrlImages,
                    Categories = p.Categories,
                    Classification = p.Classification,
                    Quantity = p.Quantity,
                    Price = p.Price,
                    Available = p.Quantity > 0
                }).ToList();

                if (query.UserId != null)
                {
                    var favorite = await _favoriteCompanyRepository.ValidateFavorite(company.Id, query.UserId);
                    result.IsFavorite = favorite != null;
                }
            }

            return result;
        }

    }
}
