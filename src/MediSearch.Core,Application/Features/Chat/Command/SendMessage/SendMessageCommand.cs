using MediatR;
using MediSearch.Core.Application.Dtos.Message;
using MediSearch.Core.Application.Helpers;
using MediSearch.Core.Application.Interfaces.Repositories;
using MediSearch.Core.Application.Interfaces.Services;
using MediSearch.Core.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MediSearch.Core.Application.Features.Chat.Command.SendMessage
{
    public class SendMessageCommand : IRequest<MessageDTO>
    {
        [JsonIgnore]
        public string? IdUser { get; set; }
        
        [SwaggerParameter(Description = "Receptor")]
        [Required(ErrorMessage = "Debe de especificar el id del usuario que va a recibir el mensaje.")]
        public string IdReceiver { get; set; }

        [SwaggerParameter(Description = "Contenido")]
        public string? Content { get; set; }
        
        [SwaggerParameter(Description = "Archivo adjunto")]
        public IFormFile? File { get; set; }
    }

    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, MessageDTO>
    {
        private readonly IHallRepository _hallRepository;
        private readonly IHallUserRepository _hallUserRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMessageTypeRepository _messageTypeRepository;

        public SendMessageCommandHandler(IHallRepository hallRepository, IHallUserRepository hallUserRepository, IMessageRepository messageRepository, IMessageTypeRepository messageTypeRepository)
        {
            _hallRepository = hallRepository;
            _hallUserRepository = hallUserRepository;
            _messageRepository = messageRepository;
            _messageTypeRepository = messageTypeRepository;
        }

        public async Task<MessageDTO> Handle(SendMessageCommand command, CancellationToken cancellationToken)
        {
            try
            {
                MessageDTO response = new();
                Message newMessage = new();
                var existChat = await _hallUserRepository.ValidateChat(command.IdUser, command.IdReceiver);
                if (existChat.Id != "")
                {
                    newMessage.HallId = existChat.HallId;
                }
                else
                {
                    Hall hall = new Hall()
                    {
                        Date = DateTime.UtcNow
                    };

                    var newHall = await _hallRepository.AddAsync(hall);

                    HallUser user = new()
                    {
                        UserId = command.IdUser,
                        HallId = newHall.Id
                    };
                    
                    HallUser receiver = new()
                    {
                        UserId = command.IdReceiver,
                        HallId = newHall.Id
                    };

                    await _hallUserRepository.AddAsync(user);
                    await _hallUserRepository.AddAsync(receiver);

                    newMessage.HallId = newHall.Id;
                }

                newMessage.Date = DateTime.UtcNow;
                newMessage.UserId = command.IdUser;
                newMessage.Content = command.Content != null ? command.Content : "";
                newMessage.Url = command.File != null ? ImageUpload.UploadImageChat(command.File, newMessage.HallId) : null;
                
                if (newMessage.Url != null)
                {
                    var type = await _messageTypeRepository.GetByNameAsync("Foto");
                    newMessage.MessageTypeId = type.Id;
                }
                else
                {
                    var type = await _messageTypeRepository.GetByNameAsync("Texto");
                    newMessage.MessageTypeId = type.Id;
                }
                
                var entity = await _messageRepository.AddAsync(newMessage);
                response.Id = entity.Id;
                response.Content = entity.Content;
                response.Url = entity.Url;
                response.Date = entity.Date;
                response.UserId = entity.UserId;

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
