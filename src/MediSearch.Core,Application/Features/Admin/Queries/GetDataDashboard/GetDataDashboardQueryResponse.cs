using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Features.Admin.Queries.GetDataDashboard
{
    public class GetDataDashboardQueryResponse
    {
        public int MyProducts { get; set; }
        public int MyUsers { get; set; }
        public int OpposingCompanies { get; set; }
        public int MyChats { get; set; }
        public List<ProvinceCompany> ProvinceCompanies { get; set; }
        public List<MaxProduct> MaxProducts { get; set; }
        public List<MaxClassification> MaxClassifications { get; set; }
        public List<MaxInteraction> MaxInteractions { get; set; }
        public List<ProductFavorites> ProductFavorites { get; set; }
    }

    public class ProvinceCompany
    {
        public string Province { get; set; }
        public int Quantity { get; set; }
    }

    public class MaxProduct
    {
        public string Product { get; set; }
        public int Quantity { get; set; }
    }

    public class MaxClassification
    {
        public string Classification { get; set; }
        public int Quantity { get; set; }
    }

    public class MaxInteraction
    {
        public string Product { get; set; }
        public int Quantity { get; set; }
    }

    public class ProductFavorites
    {
        public string Product { get; set; }
        public int Quantity { get; set; }
    }
}
