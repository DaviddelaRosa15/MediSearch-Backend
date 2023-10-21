using MediatR;
using MediSearch.Core.Application.Dtos.Product;
using MediSearch.Core.Application.Helpers;
using MediSearch.Core.Application.Interfaces.Repositories;

namespace MediSearch.Core.Application.Features.Product.Command.DeleteProduct
{
    public class DeleteProductCommand : IRequest<ProductResponse>
    {
        public string Id { get; set; }
        public string CompanyId { get; set; }

    }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, ProductResponse>
    {
        private readonly IProductRepository _productRepository;

        public DeleteProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }


        public async Task<ProductResponse> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            ProductResponse response = new()
            {
                HasError = false
            };

            try
            {
                var product = await _productRepository.GetByIdAsync(command.Id);
                if (product == null)
                {
                    response.HasError = true;
                    response.Error = "Producto no encontrado";
                    return response;
                }else if (product.CompanyId != command.CompanyId){
                    response.HasError= true;
                    return response;
                }

                await _productRepository.DeleteAsync(product);
                ImageUpload.DeleteFiles(command.Id);
                response.IsSuccess = true;
                return response;
            }
            catch (Exception ex)
            {
                response.HasError = true;
                return response;
            }
        }

    }
}
