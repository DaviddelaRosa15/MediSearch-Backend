using AutoMapper;
using MediatR;
using MediSearch.Core.Application.Dtos.Product;
using MediSearch.Core.Application.Interfaces.Repositories;
using MediSearch.Core.Domain.Entities;

namespace MediSearch.Core.Application.Features.Home.Queries.GetProductsFarmacy
{
    public class GetProductsFarmacyQuery : IRequest<List<ProductHomeDTO>>
    {
        public string? UserId { get; set; }
    }

    public class GetProductsFarmacyQueryHandler : IRequestHandler<GetProductsFarmacyQuery, List<ProductHomeDTO>>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICompanyTypeRepository _typeRepository;
        private readonly IFavoriteProductRepository _favoriteProductRepository;
        private readonly IMapper _mapper;

        public GetProductsFarmacyQueryHandler(IProductRepository productRepository, ICompanyTypeRepository typeRepository, IMapper mapper, IFavoriteProductRepository favoriteProductRepository)
        {
            _productRepository = productRepository;
            _typeRepository = typeRepository;
            _mapper = mapper;
            _favoriteProductRepository = favoriteProductRepository;
        }

        public async Task<List<ProductHomeDTO>> Handle(GetProductsFarmacyQuery query, CancellationToken cancellationToken)
        {
            var result = await GetAllProducts(query.UserId);

            return result;
        }

        public async Task<List<ProductHomeDTO>> GetAllProducts(string? user)
        {
            List<ProductHomeDTO> response = new();
            var products = await _productRepository.GetAllWithIncludeAsync(new List<string>() { "Company" });
            var farmacy = await _typeRepository.GetByNameAsync("Farmacia");
            if (user == null)
            {
                response = products.Where(p => p.Company.CompanyTypeId == farmacy.Id).Select(p => new ProductHomeDTO()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    UrlImages = p.UrlImages,
                    Classification = p.Classification,
                    Categories = p.Categories,
                    Quantity = p.Quantity,
                    Price = p.Price,
                    Available = p.Quantity > 0,
                    CompanyId = p.CompanyId,
                    NameCompany = p.Company.Name,
                    Province = p.Company.Province,
                    Municipality = p.Company.Municipality,
                    Address = p.Company.Address
                }).ToList();
            }
            else
            {
                var result = products.Where(p => p.Company.CompanyTypeId == farmacy.Id).ToList();

                foreach (var farm in result)
                {
                    var favorite = await _favoriteProductRepository.ValidateFavorite(farm.Id, user);

                    ProductHomeDTO dto = new ProductHomeDTO()
                    {
                        Id = farm.Id,
                        Name = farm.Name,
                        Description = farm.Description,
                        UrlImages = farm.UrlImages,
                        Classification = farm.Classification,
                        Categories = farm.Categories,
                        Price = farm.Price,
                        Quantity = farm.Quantity,
                        Available = farm.Quantity > 0,
                        IsFavorite = favorite != null,
                        CompanyId = farm.CompanyId,
                        NameCompany = farm.Company.Name,
                        Province = farm.Company.Province,
                        Municipality = farm.Company.Municipality,
                        Address = farm.Company.Address
                    };

                    response.Add(dto);
                }
            }
            

            return response;
        }

    }
}
