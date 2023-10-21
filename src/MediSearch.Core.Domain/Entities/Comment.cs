using MediSearch.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Domain.Entities
{
    public class Comment : AuditableBaseEntity
    {
        public string Content { get; set; }
        public string UserId { get; set; }

        //Navigation Properties
        public Product Product { get; set; }
        public string ProductId { get; set; }
        public ICollection<Reply> Replies { get; set; }

        public Comment()
        {
            this.Id = "";
        }
    }
}
