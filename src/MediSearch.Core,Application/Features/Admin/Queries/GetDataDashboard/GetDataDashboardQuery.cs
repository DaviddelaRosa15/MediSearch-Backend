using MediatR;
using MediSearch.Core.Application.Interfaces.Repositories;

namespace MediSearch.Core.Application.Features.Admin.Queries.GetDataDashboard
{
    public class GetDataDashboardQuery : IRequest<GetDataDashboardQueryResponse>
    {
        public string CompanyId { get; set; }
    }

    public class GetDataDashboardQueryHandler : IRequestHandler<GetDataDashboardQuery, GetDataDashboardQueryResponse>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly ICompanyUserRepository _companyUserRepository;
        private readonly IHallUserRepository _hallUserRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IFavoriteProductRepository _favoriteProductRepository;
        public GetDataDashboardQueryHandler(ICompanyRepository companyRepository, ICompanyUserRepository companyUserRepository, IHallUserRepository hallUserRepository, ICommentRepository commentRepository, IFavoriteProductRepository favoriteProductRepository)
        {
            _companyRepository = companyRepository;
            _companyUserRepository = companyUserRepository;
            _hallUserRepository = hallUserRepository;
            _commentRepository = commentRepository;
            _favoriteProductRepository = favoriteProductRepository;
        }

        public async Task<GetDataDashboardQueryResponse> Handle(GetDataDashboardQuery query, CancellationToken cancellationToken)
        {
            GetDataDashboardQueryResponse response = new();
            var companies = await _companyRepository.GetAllWithIncludeAsync(new List<string>() { "Products", "CompanyType" });
            var company = companies.Find(p => p.Id == query.CompanyId);
            var companyUser = await _companyUserRepository.GetAllAsync();
            var hallUser = await _hallUserRepository.GetAllAsync();

            List<MaxInteraction> maxes = new();
            List<ProductFavorites> favorites = new();
            if (company.Products.Count != 0)
            {
                foreach (var product in company.Products)
                {
                    MaxInteraction maxInteraction = new();
                    ProductFavorites favorite = new();
                    maxInteraction.Product = product.Name;
                    favorite.Product = product.Name;

                    var counFavorites = await _favoriteProductRepository.GetAllByProduct(product.Id);
                    favorite.Quantity = counFavorites.Count;

                    var comments = await _commentRepository.GetCommentsByProduct(product.Id);
                    maxInteraction.Quantity = comments.Count;

                    foreach (var comment in comments)
                    {
                        maxInteraction.Quantity = maxInteraction.Quantity + comment.Replies.Count;
                    }

                    if (maxInteraction.Quantity != 0)
                    {
                        maxes.Add(maxInteraction);
                    }

                    if (favorite.Quantity != 0)
                    {
                        favorites.Add(favorite);
                    }
                }
            }
            response.MyProducts = company.Products.Count;
            response.MyUsers = companyUser.Count(c => c.CompanyId == query.CompanyId);
            response.OpposingCompanies = companies.Count(c => c.CompanyTypeId != company.CompanyTypeId);
            response.MyChats = hallUser.Count(h => h.UserId == company.Id);
            response.ProvinceCompanies = companies.Where(c => c.CompanyTypeId != company.CompanyTypeId).GroupBy(c => c.Province).Select(c => new ProvinceCompany()
            {
                Province = c.Key,
                Quantity = c.Count()
            }).OrderByDescending(c => c.Quantity).Take(4).ToList();
            response.MaxProducts = company.Products.OrderByDescending(p => p.Quantity).Take(10).Select(p => new MaxProduct()
            {
                Product = p.Name,
                Quantity = p.Quantity
            }).ToList();
            response.MaxClassifications = company.Products.GroupBy(p => p.Classification).Select(p => new MaxClassification()
            {
                Classification = p.Key,
                Quantity = p.Count()
            }).OrderByDescending(p => p.Quantity).Take(8).ToList();
            response.MaxInteractions = maxes.OrderByDescending(m => m.Quantity).Take(5).ToList();
            response.ProductFavorites = favorites.OrderByDescending(p => p.Quantity).Take(6).ToList();

            return response;
        }
    }
}
