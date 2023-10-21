using AutoMapper;
using MediatR;
using MediSearch.Core.Application.Dtos.Product;
using MediSearch.Core.Application.Interfaces.Repositories;
using MediSearch.Core.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MediSearch.Core.Application.Features.Home.Command.AddFavoriteCompany
{
    public class AddFavoriteCompanyCommand : IRequest<ProductResponse>
    {
        [SwaggerParameter(Description = "Empresa")]
        [Required(ErrorMessage = "Debe de ingresar el id del Empresa")]
        public string CompanyId { get; set; }

        [JsonIgnore]
        public string? UserId { get; set; }
    }

    public class AddFavoriteCompanyCommandHandler : IRequestHandler<AddFavoriteCompanyCommand, ProductResponse>
    {
        private readonly IFavoriteCompanyRepository _FavoriteCompanyRepository;
        private readonly IMapper _mapper;

        public AddFavoriteCompanyCommandHandler(IFavoriteCompanyRepository FavoriteCompanyRepository, IMapper mapper)
        {
            _FavoriteCompanyRepository = FavoriteCompanyRepository;
            _mapper = mapper;
        }

        public async Task<ProductResponse> Handle(AddFavoriteCompanyCommand command, CancellationToken cancellationToken)
        {
            try
            {
                ProductResponse response = new();
                var valueToAdd = _mapper.Map<FavoriteCompany>(command);
                var entity = await _FavoriteCompanyRepository.AddAsync(valueToAdd);

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
