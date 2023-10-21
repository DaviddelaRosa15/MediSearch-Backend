using AutoMapper;
using MediatR;
using MediSearch.Core.Application.Dtos.Reply;
using MediSearch.Core.Application.Interfaces.Repositories;
using MediSearch.Core.Application.Interfaces.Services;
using MediSearch.Core.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MediSearch.Core.Application.Features.Product.Command.AddReply
{
    public class AddReplyCommand : IRequest<ReplyDTO>
    {
        [SwaggerParameter(Description = "Respuesta")]
        [Required(ErrorMessage = "Debe de especificar un contenido para esta respuesta.")]
        public string Content { get; set; }

        [SwaggerParameter(Description = "Comentario")]
        [Required(ErrorMessage = "Debe de especificar el comentario para esta respuesta.")]
        public string CommentId { get; set; }

        [JsonIgnore]
        public string? UserId { get; set; }
    }

    public class AddReplyCommandHandler : IRequestHandler<AddReplyCommand, ReplyDTO>
    {
        private readonly IReplyRepository _replyRepository;
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;
        private readonly ICompanyRepository _companyRepository;
        private readonly ICompanyUserRepository _companyUserRepository;

        public AddReplyCommandHandler(IReplyRepository replyRepository, IMapper mapper, IAccountService accountService, ICompanyRepository companyRepository, ICompanyUserRepository companyUserRepository)
        {
            _replyRepository = replyRepository;
            _mapper = mapper;
            _accountService = accountService;
            _companyRepository = companyRepository;
            _companyUserRepository = companyUserRepository;
        }


        public async Task<ReplyDTO> Handle(AddReplyCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var valueToAdd = _mapper.Map<Reply>(command);
                var entity = await _replyRepository.AddAsync(valueToAdd);
                string name = "";
                var user = await _accountService.GetUsersById(entity.UserId);
                var result = await _companyUserRepository.GetByUserAsync(entity.UserId);
                if (result != null)
                {
                    var company = await _companyRepository.GetByIdAsync(result.CompanyId);
                    name = company.Name;
                }

                ReplyDTO response = new()
                {
                    Id = entity.Id,
                    Content = entity.Content,
                    OwnerName = result == null ? $"{user.FirstName} {user.LastName}" : $"{user.FirstName} {user.LastName}({name})",
                    OwnerImage = user.UrlImage
                };

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
