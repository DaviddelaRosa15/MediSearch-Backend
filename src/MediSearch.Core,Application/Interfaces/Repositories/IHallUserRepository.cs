using MediSearch.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Interfaces.Repositories
{
    public interface IHallUserRepository : IGenericRepository<HallUser>
    {
        Task<List<HallUser>> GetByUserAsync(string user);
        Task<List<HallUser>> GetByHallAsync(string hallId);
        Task<HallUser> ValidateChat(string user, string receiver);
    }
}
