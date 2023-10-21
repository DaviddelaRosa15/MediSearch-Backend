using MediSearch.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Domain.Entities
{
	public class Product : AuditableBaseEntity
	{
        public string Name { get; set; }
		public string Description { get; set; }
        public string Classification { get; set; }
		public List<string> Categories { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public List<string>? UrlImages { get; set; }

        //Navigation Properties
        public Company Company { get; set; }
        public string CompanyId { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<FavoriteProduct> FavoriteProducts { get; set; }

        public Product()
        {
            this.Id = "";
        }
    }
}
