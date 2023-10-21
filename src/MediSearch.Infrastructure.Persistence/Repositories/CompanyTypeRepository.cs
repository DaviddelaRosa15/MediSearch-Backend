using MediSearch.Core.Application.Interfaces.Repositories;
using MediSearch.Core.Domain.Entities;
using MediSearch.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediSearch.Infrastructure.Persistence.Repositories
{
    public class CompanyTypeRepository : GenericRepository<CompanyType>, ICompanyTypeRepository
    {
        private readonly ApplicationContext _dbContext;

        public CompanyTypeRepository(ApplicationContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CompanyType> GetByNameAsync(string name)
        {
            var types = await GetAllAsync();

            CompanyType companyType = types.FirstOrDefault(x => x.Name == name);

            return companyType;
        }

    }
}
