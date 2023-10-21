using MediSearch.Core.Application.Interfaces.Repositories;
using MediSearch.Core.Domain.Entities;
using MediSearch.Infrastructure.Persistence.Contexts;

namespace MediSearch.Infrastructure.Persistence.Repositories
{
    public class MessageTypeRepository : GenericRepository<MessageType>, IMessageTypeRepository
    {
        private readonly ApplicationContext _dbContext;
        public MessageTypeRepository(ApplicationContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MessageType> GetByNameAsync(string name)
        {
            var types = await GetAllAsync();

           MessageType messageType = types.FirstOrDefault(x => x.Name == name);

            return messageType;
        }
    }
}
