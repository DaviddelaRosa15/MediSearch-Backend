using AutoMapper;
using MediatR;
using MediSearch.Core.Application.Dtos.Product;
using MediSearch.Core.Application.Interfaces.Repositories;

namespace MediSearch.Core.Application.Features.Home.Queries.GetProductsLaboratory
{
    public class GetProductsLaboratoryQuery : IRequest<List<ProductHomeDTO>>
    {
        public string? UserId { get; set; }
    }

    public class GetProductsLaboratoryQueryHandler : IRequestHandler<GetProductsLaboratoryQuery, List<ProductHomeDTO>>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICompanyTypeRepository _typeRepository;
        private readonly IFavoriteProductRepository _favoriteProductRepository;
        private readonly IMapper _mapper;

        public GetProductsLaboratoryQueryHandler(IProductRepository productRepository, ICompanyTypeRepository typeRepository, IMapper mapper, IFavoriteProductRepository favoriteProductRepository)
        {
            _productRepository = productRepository;
            _typeRepository = typeRepository;
            _mapper = mapper;
            _favoriteProductRepository = favoriteProductRepository;
        }

        public async Task<List<ProductHomeDTO>> Handle(GetProductsLaboratoryQuery query, CancellationToken cancellationToken)
        {
            var result = await GetAllProducts(query.UserId);

            return result;
        }

        public async Task<List<ProductHomeDTO>> GetAllProducts(string? user)
        {
            List<ProductHomeDTO> response = new();
            var products = await _productRepository.GetAllWithIncludeAsync(new List<string>() { "Company" });
            var laboratory = await _typeRepository.GetByNameAsync("Laboratorio");
            if (user == null)
            {
                response = products.Where(p => p.Company.CompanyTypeId == laboratory.Id).Select(p => new ProductHomeDTO()
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
                var result = products.Where(p => p.Company.CompanyTypeId == laboratory.Id).ToList();

                foreach (var lab in result)
                {
                    var favorite = await _favoriteProductRepository.ValidateFavorite(lab.Id, user);

                    ProductHomeDTO dto = new ProductHomeDTO()
                    {
                        Id = lab.Id,
                        Name = lab.Name,
                        Description = lab.Description,
                        UrlImages = lab.UrlImages,
                        Classification = lab.Classification,
                        Categories = lab.Categories,
                        Price = lab.Price,
                        Quantity = lab.Quantity,
                        Available = lab.Quantity > 0,
                        IsFavorite = favorite != null,
                        CompanyId = lab.CompanyId,
                        NameCompany = lab.Company.Name,
                        Province = lab.Company.Province,
                        Municipality = lab.Company.Municipality,
                        Address = lab.Company.Address
                    };

                    response.Add(dto);
                }
            }


            return response;
        }

    }
}
