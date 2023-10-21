using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Dtos.Account
{
    public class RegisterCompanyRequest : RegisterRequest
    {
        public string Ceo { get; set; }
        public string NameCompany { get; set; }
        public string UrlImageLogo { get; set; }
        public string ProvinceCompany { get; set; }
        public string MunicipalityCompany { get; set; }
        public string AddressCompany { get; set; }
        public string EmailCompany { get; set; }
        public string PhoneCompany { get; set; }
        public string CompanyTypeId { get; set; }
        public string? WebSite { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? Twitter { get; set; }
    }
}
