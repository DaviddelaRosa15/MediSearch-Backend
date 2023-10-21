using MediSearch.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Interfaces.Repositories
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<List<Message>> GetByHall(string hall);
    }
}
