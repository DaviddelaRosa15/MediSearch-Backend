using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Dtos.Account
{
	public class ConfirmEmailResponse
	{
        public bool HasError { get; set; }
		public string Error { get; set; }
        public string NameUser { get; set; }
    }
}
