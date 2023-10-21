using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Dtos.Account
{
	public class RefreshTokenResponse
	{
		[JsonIgnore]
		public bool HasError { get; set; }
		[JsonIgnore]
		public string Error { get; set; }
		public string JWToken { get; set; }
		
	}
}
