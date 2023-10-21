using MediatR;
using MediSearch.Core.Application.Dtos.Account;
using MediSearch.Core.Application.Dtos.Company;
using MediSearch.Core.Application.Dtos.Product;
using MediSearch.Core.Application.Interfaces.Repositories;
using MediSearch.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MediSearch.Core.Application.Features.Home.Queries.GetDataClient
{
    public class GetDataClientQuery : IRequest<GetDataClientQueryResponse>
    {
        public UserDTO User { get; set; }
    }

    public class GetDataClientQueryHandler : IRequestHandler<GetDataClientQuery, GetDataClientQueryResponse>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IProductRepository _productRepository;
        private readonly IFavoriteProductRepository _favoriteProductRepository;
        private readonly IFavoriteCompanyRepository _favoriteCompanyRepository;
        public GetDataClientQueryHandler(ICompanyRepository companyRepository, IProductRepository productRepository, IFavoriteProductRepository favoriteProductRepository, IFavoriteCompanyRepository favoriteCompanyRepository)
        {
            _companyRepository = companyRepository;
            _productRepository = productRepository;
            _favoriteProductRepository = favoriteProductRepository;
            _favoriteCompanyRepository = favoriteCompanyRepository;
        }

        public async Task<GetDataClientQueryResponse> Handle(GetDataClientQuery query, CancellationToken cancellationToken)
        {
            GetDataClientQueryResponse response = new();
            var companies = await _companyRepository.GetAllWithIncludeAsync(new List<string>() { "Products", "CompanyType" });
            var farmacies = companies.FindAll(c => c.CompanyType.Name == "Farmacia");
            var companiesFav = await _favoriteCompanyRepository.GetAllByUser(query.User.Id);
            var productsFav = await _favoriteProductRepository.GetAllByUser(query.User.Id);
            var products = await _productRepository.GetAllAsync();

            List<ProductHomeDTO> lastProducts = new();
            List<ProductHomeDTO> favoriteProducts = new();
            List<CompanyDTO> favoriteCompanies = new();
            List<CompanyDTO> sameProvinceFarmacies = new();

            foreach(var item in products.OrderBy(p => p.Created))
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
                    IsFavorite = productsFav.Any(f => f.ProductId == item.Id),
                    CompanyId = item.CompanyId,
                    NameCompany = company.Name,
                    Province = company.Province,
                    Municipality = company.Municipality,
                    Address = company.Address
                };

                lastProducts.Add(dto);
            }

            foreach (var item in productsFav)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                var company = await _companyRepository.GetByIdAsync(product.CompanyId);

                ProductHomeDTO dto = new()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    UrlImages = product.UrlImages,
                    Classification = product.Classification,
                    Categories = product.Categories,
                    Price = product.Price,
                    Quantity = product.Quantity,
                    Available = product.Quantity > 0,
                    IsFavorite = true,
                    CompanyId = product.CompanyId,
                    NameCompany = company.Name,
                    Province = company.Province,
                    Municipality = company.Municipality,
                    Address = company.Address
                };

                favoriteProducts.Add(dto);
            }

            foreach (var item in companiesFav)
            {
                var company = await _companyRepository.GetByIdAsync(item.CompanyId);

                CompanyDTO dto = new()
                {
                    Id = company.Id,
                    Name = company.Name,
                    UrlImage = company.UrlImage,
                    Phone = company.Phone,
                    Province = company.Province,
                    Municipality = company.Municipality,
                    Address = company.Address,
                    IsFavorite = true
                };

                favoriteCompanies.Add(dto);
            }

            var sameProvince = farmacies.FindAll(f => f.Province == query.User.Province);
            foreach (var item in sameProvince)
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
                    IsFavorite = companiesFav.Any(f => f.CompanyId == item.Id)
                };

                sameProvinceFarmacies.Add(dto);
            }

            response.LastProducts = lastProducts.Take(10).ToList();
            response.FavoriteProducts = favoriteProducts.Take(10).ToList();
            response.FavoriteCompanies = favoriteCompanies.Take(10).ToList();
            response.SameProvinceFarmacies = sameProvinceFarmacies.Take(10).ToList();

            return response;
        }
    }
}
