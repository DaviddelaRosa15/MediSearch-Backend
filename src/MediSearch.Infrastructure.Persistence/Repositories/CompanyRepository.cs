using MediSearch.Core.Application.Interfaces.Repositories;
using MediSearch.Core.Domain.Entities;
using MediSearch.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediSearch.Infrastructure.Persistence.Repositories
{
    public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
    {
        private readonly ApplicationContext _dbContext;

        public CompanyRepository(ApplicationContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Company> GetByNameAsync(string name)
        {
            var companies = await GetAllAsync();

            Company company = companies.FirstOrDefault(x => x.Name == name);

            return company;
        }

        public async Task<Company> GetByEmailAsync(string email)
        {
            var companies = await GetAllAsync();

            Company company = companies.FirstOrDefault(x => x.Email == email);

            return company;
        }

    }
}
