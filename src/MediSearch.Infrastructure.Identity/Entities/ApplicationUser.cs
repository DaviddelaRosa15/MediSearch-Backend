using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Infrastructure.Identity.Entities
{
	public class ApplicationUser : IdentityUser
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UrlImage { get; set; }
		public string Province { get; set; }
        public string Municipality { get; set; }
        public string Address { get; set; }
    }
}
