using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Dtos.Account
{
	public class RegisterRequest
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
		public string UrlImage { get; set; }
        public string Province { get; set; }
        public string Municipality { get; set; }
        public string Address { get; set; }
	}
}
