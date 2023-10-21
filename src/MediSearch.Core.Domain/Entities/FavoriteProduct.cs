using MediSearch.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Domain.Entities
{
    public class FavoriteProduct : AuditableBaseEntity
    {
        public string UserId { get; set; }

        //Navigation Properties
        public Product Product { get; set; }
        public string ProductId { get; set; }

        public FavoriteProduct()
        {
            this.Id = "";
        }
    }
}
