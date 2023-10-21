using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Domain.Common
{
	public class AuditableBaseEntity
	{
		public virtual string Id { get; set; }
		public string CreatedBy { get; set; }
		public DateTime Created { get; set; }
		public string? LastModifiedBy { get; set; }
		public DateTime? LastModified { get; set; }
	}
}
