using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Dtos.Product
{
    public class ProductHomeDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Categories { get; set; }
        public string Classification { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public List<string>? UrlImages { get; set; }
        public bool Available { get; set; }
        public string CompanyId { get; set; }
        public string NameCompany { get; set; }
        public string Province { get; set; }
        public string Municipality { get; set; }
        public string Address { get; set; }
        public bool IsFavorite { get; set; }
    }
}
