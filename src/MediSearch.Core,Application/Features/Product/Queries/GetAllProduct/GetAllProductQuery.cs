using AutoMapper;
using MediatR;
using MediSearch.Core.Application.Dtos.Product;
using MediSearch.Core.Application.Interfaces.Repositories;

namespace MediSearch.Core.Application.Features.Product.Queries.GetAllProduct
{
    public class GetAllProductQuery : IRequest<List<ProductDTO>>
    {
        public string CompanyId { get; set; }
    }

    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQuery, List<ProductDTO>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public GetAllProductQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<List<ProductDTO>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
        {
            List<ProductDTO> products = await GetProductsCompany(request.CompanyId);

            return products;
        }

        private async Task<List<ProductDTO>> GetProductsCompany(string company)
        {
            var products = await _productRepository.GetAllAsync();
            var productsCompany = products.FindAll(x => x.CompanyId == company);
            var result = _mapper.Map<List<ProductDTO>>(productsCompany);

            return result;
        }
    }
}
