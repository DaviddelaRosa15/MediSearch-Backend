using MediSearch.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Interfaces.Repositories
{
    public interface IFavoriteProductRepository : IGenericRepository<FavoriteProduct>
    {
        Task<FavoriteProduct> ValidateFavorite(string product, string user);
        Task<List<FavoriteProduct>> GetAllByProduct(string product);
        Task<List<FavoriteProduct>> GetAllByUser(string user);
    }
}
