using MediSearch.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Domain.Entities
{
    public class Reply : AuditableBaseEntity
    {
        public string Content { get; set; }
        public string UserId { get; set; }

        //Navigation Properties
        public Comment Comment { get; set; }
        public string CommentId { get; set; }

        public Reply()
        {
            this.Id = "";
        }
    }
}
