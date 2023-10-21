using MediSearch.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Interfaces.Repositories
{
    public interface IMessageTypeRepository : IGenericRepository<MessageType>
    {
        Task<MessageType> GetByNameAsync(string name);
    }
}
