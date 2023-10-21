using MediSearch.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Domain.Entities
{
	public class MessageType : AuditableBaseEntity
	{
        public string Name { get; set; }

        //Navigation Properties
        public ICollection<Message> Messages { get; set; }

        public MessageType()
        {
            this.Id = "";
        }
    }
}
