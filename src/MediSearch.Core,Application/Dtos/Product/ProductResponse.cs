using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Dtos.Product
{
    public class ProductResponse
    {
        public bool HasError { get; set; }
        public string Error { get; set; }
        public bool IsSuccess { get; set; }
    }
}
