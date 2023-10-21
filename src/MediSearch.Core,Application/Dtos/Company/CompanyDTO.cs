using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Dtos.Company
{
    public class CompanyDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UrlImage { get; set; }
        public string Phone { get; set; }
        public string Province { get; set; }
        public string Municipality { get; set; }
        public string Address { get; set; }
        public bool IsFavorite { get; set; }
    }
}
