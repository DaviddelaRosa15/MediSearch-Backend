using MediSearch.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Domain.Entities
{
	public class HallUser : AuditableBaseEntity
	{
		public string UserId { get; set; }

		//Navigation Properties
		public Hall Hall { get; set; }
		public string HallId { get; set; }

        public HallUser()
        {
            this.Id = "";
        }
    }
}
