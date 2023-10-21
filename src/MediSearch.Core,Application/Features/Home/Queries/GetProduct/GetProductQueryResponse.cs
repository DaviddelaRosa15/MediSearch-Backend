using MediSearch.Core.Application.Dtos.Comment;
using MediSearch.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Features.Home.Queries.GetProduct
{
    public class GetProductQueryResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Categories { get; set; }
        public string Classification { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public bool Available { get; set; }
        public List<string>? Images { get; set; }
        public string CompanyId { get; set; }
        public string NameCompany { get; set; }
        public string Ceo { get; set; }
        public string Logo { get; set; }
        public string Province { get; set; }
        public string Municipality { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? WebSite { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? Twitter { get; set; }
        public List<CommentDTO> Comments { get; set; }
        public bool IsFavorite { get; set; }
    }
}
