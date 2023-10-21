using MediSearch.Core.Application.Interfaces.Repositories;
using MediSearch.Core.Domain.Entities;
using MediSearch.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediSearch.Infrastructure.Persistence.Repositories
{
    public class CompanyUserRepository : GenericRepository<CompanyUser>, ICompanyUserRepository
    {
        private readonly ApplicationContext _dbContext;

        public CompanyUserRepository(ApplicationContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CompanyUser> GetByUserAsync(string user)
        {
            var users = await GetAllWithIncludeAsync(new List<string> { "Company"});

            CompanyUser companyUser = users.FirstOrDefault(x => x.UserId == user);

            return companyUser;
        }
        
        public async Task<List<CompanyUser>> GetByCompanyAsync(string company)
        {
            var users = await GetAllAsync();

            var companies = users.Where(x => x.CompanyId == company);

            return companies.ToList();
        }

        public async Task<bool> ValidateEmployeAsync(string company, string user)
        {
            var users = await GetAllAsync();

            var validation = users.FirstOrDefault(x => x.CompanyId == company && x.UserId == user);

            if (validation != null)
            {
                return true;
            }

            return false;
        }
    }
}
