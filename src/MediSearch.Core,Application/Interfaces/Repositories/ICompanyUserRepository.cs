using MediSearch.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Interfaces.Repositories
{
    public interface ICompanyUserRepository : IGenericRepository<CompanyUser>
    {
        Task<CompanyUser> GetByUserAsync(string user);
        Task<List<CompanyUser>> GetByCompanyAsync(string company);
        Task<bool> ValidateEmployeAsync(string company, string user);
    }
}
