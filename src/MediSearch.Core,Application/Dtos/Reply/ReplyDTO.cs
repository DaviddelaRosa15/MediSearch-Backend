using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Dtos.Reply
{
    public class ReplyDTO
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string OwnerName { get; set; }
        public string OwnerImage { get; set; }
    }
}
