using MediSearch.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Interfaces.Repositories
{
    public interface ICompanyTypeRepository : IGenericRepository<CompanyType>
    {
        Task<CompanyType> GetByNameAsync(string name);
    }
}
