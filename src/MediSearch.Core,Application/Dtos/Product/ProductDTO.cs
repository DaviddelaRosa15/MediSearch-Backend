using MediSearch.Core.Application.Dtos.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Dtos.Product
{
    public class ProductDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Categories { get; set; }
        public string Classification { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public List<string> UrlImages { get; set; }
        public List<CommentDTO> Comments { get; set; }
    }
}
