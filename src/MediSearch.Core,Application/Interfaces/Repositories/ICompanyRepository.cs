using MediSearch.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Interfaces.Repositories
{
    public interface ICompanyRepository : IGenericRepository<Company>
    {
        Task<Company> GetByNameAsync(string name);
        Task<Company> GetByEmailAsync(string email);
    }
}
