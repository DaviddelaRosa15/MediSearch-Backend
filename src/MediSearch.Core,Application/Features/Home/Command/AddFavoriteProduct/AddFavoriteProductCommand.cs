using AutoMapper;
using MediatR;
using MediSearch.Core.Application.Dtos.Product;
using MediSearch.Core.Application.Interfaces.Repositories;
using MediSearch.Core.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MediSearch.Core.Application.Features.Home.Command.AddFavoriteProduct
{
    public class AddFavoriteProductCommand : IRequest<ProductResponse>
    {
        [SwaggerParameter(Description = "Producto")]
        [Required(ErrorMessage = "Debe de ingresar el id del producto")]
        public string ProductId { get; set; }

        [JsonIgnore]
        public string? UserId { get; set; }
    }

    public class AddFavoriteProductCommandHandler : IRequestHandler<AddFavoriteProductCommand, ProductResponse>
    {
        private readonly IFavoriteProductRepository _FavoriteProductRepository;
        private readonly IMapper _mapper;

        public AddFavoriteProductCommandHandler(IFavoriteProductRepository FavoriteProductRepository, IMapper mapper)
        {
            _FavoriteProductRepository = FavoriteProductRepository;
            _mapper = mapper;
        }

        public async Task<ProductResponse> Handle(AddFavoriteProductCommand command, CancellationToken cancellationToken)
        {
            try
            {
                ProductResponse response = new();
                var valueToAdd = _mapper.Map<FavoriteProduct>(command);
                var entity = await _FavoriteProductRepository.AddAsync(valueToAdd);

                response.IsSuccess = true;
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
