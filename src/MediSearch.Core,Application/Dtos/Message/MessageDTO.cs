using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Dtos.Message
{
    public class MessageDTO
    {
        public string Id { get; set; }
        public string? Content { get; set; }
        public string? Url { get; set; }
        public DateTime Date { get; set; }
        public string UserId { get; set; }
    }
}
