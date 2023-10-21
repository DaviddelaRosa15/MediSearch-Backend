using MediatR;
using MediSearch.Core.Application.Interfaces.Repositories;

namespace MediSearch.Core.Application.Features.Home.Command.DeleteFavoriteProduct
{
    public class DeleteFavoriteProductCommand : IRequest<DeleteFavoriteProductResponse>
    {
        public string ProductId { get; set; }
        public string UserId { get; set; }
    }

    public class DeleteFavoriteProductCommandHandler : IRequestHandler<DeleteFavoriteProductCommand, DeleteFavoriteProductResponse>
    {
        private readonly IFavoriteProductRepository _favoriteProductRepository;

        public DeleteFavoriteProductCommandHandler(IFavoriteProductRepository favoriteProductRepository)
        {
            _favoriteProductRepository = favoriteProductRepository;
        }


        public async Task<DeleteFavoriteProductResponse> Handle(DeleteFavoriteProductCommand command, CancellationToken cancellationToken)
        {
            DeleteFavoriteProductResponse response = new();

            try
            {
                var favorite = await _favoriteProductRepository.ValidateFavorite(command.ProductId, command.UserId);
                if (favorite == null)
                {
                    throw new Exception("Favorito no encontrado");
                }

                await _favoriteProductRepository.DeleteAsync(favorite);
                response.ProductId = favorite.ProductId;
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
