using AutoMapper;
using MediatR;
using MediSearch.Core.Application.Dtos.Message;
using MediSearch.Core.Application.Dtos.Product;
using MediSearch.Core.Application.Interfaces.Repositories;
using MediSearch.Core.Application.Interfaces.Services;
using MediSearch.Core.Domain.Entities;

namespace MediSearch.Core.Application.Features.Chat.Queries.GetChats
{
    public class GetChatsQuery : IRequest<List<GetChatsQueryResponse>>
    {
        public string IdUser { get; set; }
    }

    public class GetChatsQueryHandler : IRequestHandler<GetChatsQuery, List<GetChatsQueryResponse>>
    {
        private readonly IHallRepository _hallRepository;
        private readonly IHallUserRepository _hallUserRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IAccountService _accountService;

        public GetChatsQueryHandler(IHallRepository hallRepository, IHallUserRepository hallUserRepository, IMessageRepository messageRepository, IAccountService accountService, ICompanyRepository companyRepository)
        {
            _hallRepository = hallRepository;
            _hallUserRepository = hallUserRepository;
            _messageRepository = messageRepository;
            _companyRepository = companyRepository;
            _accountService = accountService;
        }

        public async Task<List<GetChatsQueryResponse>> Handle(GetChatsQuery request, CancellationToken cancellationToken)
        {
            List<GetChatsQueryResponse> result = await GetChats(request);

            return result;
        }

        private async Task<List<GetChatsQueryResponse>> GetChats(GetChatsQuery query)
        {
            List<GetChatsQueryResponse> responses = new();
            var hall = await _hallUserRepository.GetByUserAsync(query.IdUser);
            if(hall == null || hall.Count == 0)
            {
                return null;
            }

            foreach(var chat in hall)
            {
                GetChatsQueryResponse response = new(); 
                var halls = await _hallUserRepository.GetByHallAsync(chat.HallId);
                var receiver = halls.Where(h => h.UserId != query.IdUser).FirstOrDefault();
                var userData = await _accountService.GetUsersById(receiver.UserId);
                var messages = await _messageRepository.GetByHall(chat.HallId);
                response.Id = receiver.HallId;
                response.ReceiverId = receiver.UserId;
                response.LastMessage = messages.OrderByDescending(m => m.Date).Select(m => new MessageDTO()
                {
                    Id = m.Id,
                    Content = m.Content,
                    Url = m.Url,
                    Date = m.Date,
                    UserId = m.UserId
                }).FirstOrDefault();

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

                responses.Add(response);
            }

            return responses;
        }
    }
}
