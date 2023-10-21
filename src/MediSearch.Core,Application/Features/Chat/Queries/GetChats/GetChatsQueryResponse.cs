using MediSearch.Core.Application.Dtos.Message;
using MediSearch.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Features.Chat.Queries.GetChats
{
    public class GetChatsQueryResponse
    {
        public string Id { get; set; }
        public string ReceiverId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public MessageDTO LastMessage { get; set; }
    }
}
