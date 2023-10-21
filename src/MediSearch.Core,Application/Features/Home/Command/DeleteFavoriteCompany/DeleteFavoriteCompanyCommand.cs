using MediatR;
using MediSearch.Core.Application.Interfaces.Repositories;

namespace MediSearch.Core.Application.Features.Home.Command.DeleteFavoriteCompany
{
    public class DeleteFavoriteCompanyCommand : IRequest<DeleteFavoriteCompanyResponse>
    {
        public string CompanyId { get; set; }
        public string UserId { get; set; }
    }

    public class DeleteFavoriteCompanyCommandHandler : IRequestHandler<DeleteFavoriteCompanyCommand, DeleteFavoriteCompanyResponse>
    {
        private readonly IFavoriteCompanyRepository _favoriteCompanyRepository;

        public DeleteFavoriteCompanyCommandHandler(IFavoriteCompanyRepository favoriteCompanyRepository)
        {
            _favoriteCompanyRepository = favoriteCompanyRepository;
        }


        public async Task<DeleteFavoriteCompanyResponse> Handle(DeleteFavoriteCompanyCommand command, CancellationToken cancellationToken)
        {
            DeleteFavoriteCompanyResponse response = new();

            try
            {
                var favorite = await _favoriteCompanyRepository.ValidateFavorite(command.CompanyId, command.UserId);
                if (favorite == null)
                {
                    throw new Exception("Favorito no encontrado");
                }

                await _favoriteCompanyRepository.DeleteAsync(favorite);
                response.CompanyId = favorite.CompanyId;
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
