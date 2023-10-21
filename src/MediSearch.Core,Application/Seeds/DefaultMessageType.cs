using MediSearch.Core.Application.Interfaces.Repositories;
using MediSearch.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Seeds
{
    public static class DefaultMessageType
    {
        public static async Task SeedAsync(IMessageTypeRepository typeRepository)
        {
            List<string> types = new()
            {
                "Texto",
                "Foto"
            };

            foreach(var item in types) {
                var exist = await typeRepository.GetByNameAsync(item);

                if (exist == null)
                {
                    MessageType type = new()
                    {
                        Name = item
                    };

                    await typeRepository.AddAsync(type);
                }
            }
        }
    }
}
