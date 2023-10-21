using AutoMapper;
using MediatR;
using MediSearch.Core.Application.Dtos.Product;
using MediSearch.Core.Application.Helpers;
using MediSearch.Core.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MediSearch.Core.Application.Features.Product.Command.UpdateProduct
{
    public class UpdateProductCommand : IRequest<ProductResponse>
    {
        [SwaggerParameter(Description = "Id del producto a actualizar.")]
        [Required(ErrorMessage = "Debe de especificar un nombre para este producto.")]
        public string Id { get; set; }

        [SwaggerParameter(Description = "Nombre nuevo para el producto.")]
        [Required(ErrorMessage = "Debe de especificar un nombre para este producto.")]
        public string Name { get; set; }

        [SwaggerParameter(Description = "Descripción nueva para el producto.")]
        [Required(ErrorMessage = "Debe de especificar un nombre para este producto.")]
        public string Description { get; set; }

        [SwaggerParameter(Description = "Categorías del producto.")]
        [Required(ErrorMessage = "Debe de especificar categorías para este producto.")]
        public List<string> Categories { get; set; }

        [SwaggerParameter(Description = "Clasificación del producto.")]
        [Required(ErrorMessage = "Debe especificar la clasificación de este producto.")]
        public string Classification { get; set; }

        [SwaggerParameter(Description = "Precio nuevo para el producto.")]
        [Required(ErrorMessage = "Debe de especificar un precio para este producto.")]
        public double Price { get; set; }

        [SwaggerParameter(Description = "Cantidad disponible que tienes del producto.")]
        [Required(ErrorMessage = "Debe de especificar la cantidad del producto.")]
        public int Quantity { get; set; }

        [SwaggerParameter(Description = "Imágenes que deseas destinar para el producto.")]
        public IFormFile[]? Images { get; set; }
        [JsonIgnore]
        public string? CompanyId { get; set; }

    }

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductResponse>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public UpdateProductCommandHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }


        public async Task<ProductResponse> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            ProductResponse response = new()
            {
                HasError = false
            };

            try
            {
                var valueToUpdate = await _productRepository.GetByIdAsync(command.Id);
                if (valueToUpdate == null)
                {
                    response.HasError = true;
                    response.Error = "Producto no encontrado";
                    return response;
                }

                var newValues = _mapper.Map<Domain.Entities.Product>(command);
                newValues.Created = valueToUpdate.Created;
                newValues.CreatedBy = valueToUpdate.CreatedBy;
                if (command.Images != null && command.Images.Length != 0)
                {
                    newValues.UrlImages = await ImageUpload.UploadImagesProduct(command.Images, newValues.Id, true, valueToUpdate.UrlImages);
                }
                else
                {
                    newValues.UrlImages = valueToUpdate.UrlImages;
                }

                await _productRepository.UpdateAsync(newValues, newValues.Id);
                response.IsSuccess = true;
                return response;
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.Error = ex.Message;

                return response;
            }
        }

    }

}
