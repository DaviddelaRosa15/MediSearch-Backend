using MediatR;
using MediSearch.Core.Application.Interfaces.Repositories;

namespace MediSearch.Core.Application.Features.Home.Queries.GetAllFavoriteProducts
{
    public class GetAllFavoriteProductsQuery : IRequest<List<GetAllFavoriteProductsQueryResponse>>
    {
        public string UserId { get; set; }
    }

    public class GetAllFavoriteProductsQueryHandler : IRequestHandler<GetAllFavoriteProductsQuery, List<GetAllFavoriteProductsQueryResponse>>
    {
        private readonly IFavoriteProductRepository _favoriteProductRepository;
        private readonly ICompanyRepository _companyRepository;

        public GetAllFavoriteProductsQueryHandler(IFavoriteProductRepository favoriteProductRepository, ICompanyRepository companyRepository)
        {
            _favoriteProductRepository = favoriteProductRepository;
            _companyRepository = companyRepository;
        }

        public async Task<List<GetAllFavoriteProductsQueryResponse>> Handle(GetAllFavoriteProductsQuery query, CancellationToken cancellationToken)
        {
            var products = await _favoriteProductRepository.GetAllWithIncludeAsync(new List<string>() { "Product"});
            var favorites = products.FindAll(c => c.UserId == query.UserId);
            List<GetAllFavoriteProductsQueryResponse> response = new();

            foreach (var item in favorites)
            {
                var company = await _companyRepository.GetByIdAsync(item.Product.CompanyId);

                GetAllFavoriteProductsQueryResponse favorite = new()
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Name = item.Product.Name,
                    Description = item.Product.Description,
                    Classification = item.Product.Classification,
                    Categories = item.Product.Categories,
                    UrlImages = item.Product.UrlImages,
                    Price = item.Product.Price,
                    Quantity = item.Product.Quantity,
                    Available = item.Product.Quantity > 0,
                    CompanyId = item.Product.CompanyId,
                    NameCompany = company.Name,
                    Province = company.Province,
                    Municipality = company.Municipality,
                    Address = company.Address,
                    IsFavorite = true
                };

                response.Add(favorite);
            }

            return response;
        }

    }
}
