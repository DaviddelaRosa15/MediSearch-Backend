using MediSearch.Core.Domain.Entities;

namespace MediSearch.Core.Application.Interfaces.Repositories
{
    public interface IFavoriteCompanyRepository : IGenericRepository<FavoriteCompany>
    {
        Task<FavoriteCompany> ValidateFavorite(string company, string user);
        Task<List<FavoriteCompany>> GetAllByUser(string user);
    }
}
