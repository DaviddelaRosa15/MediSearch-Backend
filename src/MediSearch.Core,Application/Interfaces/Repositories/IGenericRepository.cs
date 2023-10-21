using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Interfaces.Repositories
{
	public interface IGenericRepository<Entity> where Entity : class
	{
		Task<Entity> AddAsync(Entity entity);
		Task UpdateAsync(Entity entity, string id);
		Task DeleteAsync(Entity entity);
		Task<List<Entity>> GetAllAsync();
		Task<Entity> GetByIdAsync(string id);
		Task<List<Entity>> GetAllWithIncludeAsync(List<string> properties);
	}
}
