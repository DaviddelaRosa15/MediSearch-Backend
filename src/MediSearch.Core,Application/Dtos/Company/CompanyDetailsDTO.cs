using MediSearch.Core.Application.Dtos.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Dtos.Company
{
    public class CompanyDetailsDTO
    {
        public string Id { get; set; }
        public string Ceo { get; set; }
        public string Name { get; set; }
        public string UrlImage { get; set; }
        public string Province { get; set; }
        public string Municipality { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? WebSite { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? Twitter { get; set; }
        public List<ProductHomeDTO> Products { get; set; }
        public bool IsFavorite { get; set; }
    }
}
