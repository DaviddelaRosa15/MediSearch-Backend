using MediSearch.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Domain.Entities
{
	public class Message : AuditableBaseEntity
	{
        public string UserId { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public string? Url { get; set; }

        //Navigation Properties
        public Hall Hall { get; set; }
        public string HallId { get; set; }
        public MessageType MessageType { get; set; }
        public string MessageTypeId { get; set; }

        public Message()
        {
            this.Id = "";
        }
    }
}
