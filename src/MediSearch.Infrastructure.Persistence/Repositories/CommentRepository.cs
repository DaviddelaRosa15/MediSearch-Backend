using MediSearch.Core.Application.Interfaces.Repositories;
using MediSearch.Core.Domain.Entities;
using MediSearch.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediSearch.Infrastructure.Persistence.Repositories
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        private readonly ApplicationContext _dbContext;

        public CommentRepository(ApplicationContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Comment>> GetCommentsByProduct(string id)
        {
            var all = await GetAllWithIncludeAsync(new List<string>() { "Replies" });
            var comments = all.FindAll(x => x.ProductId == id);

            return comments;
        }

    }
}
