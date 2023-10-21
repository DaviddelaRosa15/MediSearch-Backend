using MediSearch.Core.Application.Interfaces.Repositories;
using MediSearch.Core.Domain.Entities;
using MediSearch.Infrastructure.Persistence.Contexts;

namespace MediSearch.Infrastructure.Persistence.Repositories
{
    public class FavoriteCompanyRepository : GenericRepository<FavoriteCompany>, IFavoriteCompanyRepository
    {
        private readonly ApplicationContext _dbContext;
        public FavoriteCompanyRepository(ApplicationContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<FavoriteCompany> ValidateFavorite(string company, string user)
        {
            var companies = await GetAllWithIncludeAsync(new List<string>());
            var favorite = companies.Find(x => x.CompanyId == company && x.UserId == user);

            return favorite;
        }

        public async Task<List<FavoriteCompany>> GetAllByUser(string user)
        {
            var companies = await GetAllAsync();
            var favorites = companies.FindAll(x => x.UserId == user);

            return favorites;
        }
    }
}
