using MediSearch.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Domain.Entities
{
	public class CompanyUser : AuditableBaseEntity
	{
        public string UserId { get; set; }

        //Navigation Properties
        public Company Company { get; set; }
        public string CompanyId { get; set; }

        public CompanyUser()
        {
            this.Id = "";
        }

    }
}
