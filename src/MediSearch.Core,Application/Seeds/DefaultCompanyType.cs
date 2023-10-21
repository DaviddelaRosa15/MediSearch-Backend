using MediSearch.Core.Application.Interfaces.Repositories;
using MediSearch.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Seeds
{
    public static class DefaultCompanyType
    {
        public static async Task SeedAsync(ICompanyTypeRepository typeRepository)
        {
            List<string> types = new()
            {
                "Farmacia",
                "Laboratorio"
            };

            foreach(var item in types) {
                var exist = await typeRepository.GetByNameAsync(item);

                if (exist == null)
                {
                    CompanyType type = new()
                    {
                        Name = item
                    };

                    await typeRepository.AddAsync(type);
                }
            }
        }
    }
}
