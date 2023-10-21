using MediatR;
using MediSearch.Core.Application.Dtos.Message;
using MediSearch.Core.Application.Interfaces.Repositories;
using MediSearch.Core.Application.Interfaces.Services;

namespace MediSearch.Core.Application.Features.Chat.Queries.GetChat
{
    public class GetChatQuery : IRequest<GetChatQueryResponse>
    {
        public string IdHall { get; set; }
        public string IdUser { get; set; }
    }

    public class GetChatQueryHandler : IRequestHandler<GetChatQuery, GetChatQueryResponse>
    {
        private readonly IHallRepository _hallRepository;
        private readonly IHallUserRepository _hallUserRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IAccountService _accountService;

        public GetChatQueryHandler(IHallRepository hallRepository, IHallUserRepository hallUserRepository, IMessageRepository messageRepository, IAccountService accountService, ICompanyRepository companyRepository)
        {
            _hallRepository = hallRepository;
            _hallUserRepository = hallUserRepository;
            _messageRepository = messageRepository;
            _companyRepository = companyRepository;
            _accountService = accountService;
        }

        public async Task<GetChatQueryResponse> Handle(GetChatQuery request, CancellationToken cancellationToken)
        {
            GetChatQueryResponse result = await GetChat(request);

            return result;
        }

        private async Task<GetChatQueryResponse> GetChat(GetChatQuery query)
        {
            var hall = await _hallRepository.GetByIdAsync(query.IdHall);
            if (hall == null)
            {
                return null;
            }

            GetChatQueryResponse response = new();
            var halls = await _hallUserRepository.GetByHallAsync(query.IdHall);
            var receiver = halls.Where(h => h.UserId != query.IdUser).FirstOrDefault();
            var userData = await _accountService.GetUsersById(receiver.UserId);
            var messages = await _messageRepository.GetByHall(query.IdHall);
            response.Id = receiver.HallId;
            response.ReceiverId = receiver.UserId;
            response.Messages = messages.OrderByDescending(m => m.Date).Select(m => new MessageDTO()
            {
                Id = m.Id,
                Content = m.Content,
                Url = m.Url,
                Date = m.Date,
                UserId = m.UserId
            }).ToList();

            if (userData == null)
            {
                var company = await _companyRepository.GetByIdAsync(receiver.UserId);
                response.Name = company.Name;
                response.Image = company.UrlImage;
            }
            else
            {
                response.Name = userData.FirstName + " " + userData.LastName;
                response.Image = userData.UrlImage;
            }

            return response;
        }
    }
}
