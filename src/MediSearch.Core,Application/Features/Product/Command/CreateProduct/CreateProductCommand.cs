using AutoMapper;
using MediatR;
using MediSearch.Core.Application.Dtos.Product;
using MediSearch.Core.Application.Helpers;
using MediSearch.Core.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MediSearch.Core.Application.Features.Product.CreateProduct
{
    public class CreateProductCommand : IRequest<ProductResponse>
    {
        [SwaggerParameter(Description = "Nombre que deseas destinar para el producto.")]
        [Required(ErrorMessage = "Debe de especificar un nombre para este producto.")]
        public string Name { get; set; }

        [SwaggerParameter(Description = "Descripción que deseas destinar para el producto.")]
        [Required(ErrorMessage = "Debe de especificar un nombre para este producto.")]
        public string Description { get; set; }

        [SwaggerParameter(Description = "Categorías del producto.")]
        [Required(ErrorMessage = "Debe de especificar categorías para este producto.")]
        public List<string> Categories { get; set; }

        [SwaggerParameter(Description = "Clasificación del producto.")]
        [Required(ErrorMessage = "Debe especificar la clasificación de este producto.")]
        public string Classification { get; set; }

        [SwaggerParameter(Description = "Precio que deseas destinar para el producto.")]
        [Required(ErrorMessage = "Debe de especificar un precio para este producto.")]
        public double Price { get; set; }

        [SwaggerParameter(Description = "Cantidad disponible que tienes del producto.")]
        [Required(ErrorMessage = "Debe de especificar la cantidad del producto.")]
        public int Quantity { get; set; }

        [SwaggerParameter(Description = "Imágenes que deseas destinar para el producto.")]
        [MinLength(1, ErrorMessage = "Debe de subir al menos una imagén para este producto.")]
        public IFormFile[] Images { get; set; }
        [JsonIgnore]
        public string? CompanyId { get; set; }
    }

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductResponse>
    {
        private readonly IProductRepository _productRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public CreateProductCommandHandler(IProductRepository productRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<ProductResponse> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            ProductResponse response = new()
            {
                HasError = false
            };
            string id = "";

            try
            {
                var valueToAdd = _mapper.Map<Domain.Entities.Product>(command);
                valueToAdd = await _productRepository.AddAsync(valueToAdd);
                valueToAdd.UrlImages = await ImageUpload.UploadImagesProduct(command.Images, valueToAdd.Id);
                id = valueToAdd.Id;

                await _productRepository.UpdateAsync(valueToAdd, valueToAdd.Id);
                response.IsSuccess = true;
                return response;
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.Error = ex.Message;
                if (id != "")
                {
                    ImageUpload.DeleteFiles(id);
                }

                return response;
            }
        }

    }
}
