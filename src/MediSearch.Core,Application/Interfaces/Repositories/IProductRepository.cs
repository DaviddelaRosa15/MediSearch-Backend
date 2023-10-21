using MediSearch.Core.Domain.Entities;

namespace MediSearch.Core.Application.Interfaces.Repositories;
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<List<Product>> GetProductsByCompany(string company);
    }
