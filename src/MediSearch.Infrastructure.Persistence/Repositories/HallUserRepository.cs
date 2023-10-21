using MediSearch.Core.Application.Interfaces.Repositories;
using MediSearch.Core.Domain.Entities;
using MediSearch.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Infrastructure.Persistence.Repositories
{
    public class HallUserRepository : GenericRepository<HallUser>, IHallUserRepository
    {
        private readonly ApplicationContext _dbContext;
        public HallUserRepository(ApplicationContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<HallUser>> GetByUserAsync(string user)
        {
            var hall = await GetAllAsync();

            List<HallUser> halls = hall.Where(x => x.UserId == user).ToList();

            return halls;
        }

        public async Task<List<HallUser>> GetByHallAsync(string hallId)
        {
            var hall = await GetAllAsync();

            List<HallUser> halls = hall.Where(x => x.HallId == hallId).ToList();

            return halls;
        }

        public async Task<HallUser> ValidateChat(string user, string receiver)
        {
            HallUser exists = new();
            var hall = await GetAllAsync();

            List<HallUser> halls = hall.Where(x => x.UserId == user).ToList();

            foreach (var item in halls)
            {
                var users = hall.Where(x => x.HallId == item.HallId).ToList();

                var exist = users.Where(x => x.UserId == receiver).FirstOrDefault();
                if (exist != null)
                {
                    exists = exist;
                    break;
                }
            }

            return exists;
        }
    }
}
