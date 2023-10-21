using AutoMapper;
using MediatR;
using MediSearch.Core.Application.Dtos.Comment;
using MediSearch.Core.Application.Dtos.Product;
using MediSearch.Core.Application.Dtos.Reply;
using MediSearch.Core.Application.Interfaces.Repositories;
using MediSearch.Core.Application.Interfaces.Services;
using MediSearch.Core.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MediSearch.Core.Application.Features.Product.Command.AddComment
{
    public class AddCommentCommand : IRequest<CommentDTO>
    {
        [SwaggerParameter(Description = "Comentario")]
        [Required(ErrorMessage = "Debe de especificar un contenido para este comentario.")]
        public string Content { get; set; }

        [SwaggerParameter(Description = "Producto")]
        [Required(ErrorMessage = "Debe de especificar el producto para este comentario.")]
        public string ProductId { get; set; }

        [JsonIgnore]
        public string? UserId { get; set; }
    }

    public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, CommentDTO>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;
        private readonly ICompanyRepository _companyRepository;
        private readonly ICompanyUserRepository _companyUserRepository;

        public AddCommentCommandHandler(ICommentRepository commentRepository, IMapper mapper, IAccountService accountService, ICompanyRepository companyRepository, ICompanyUserRepository companyUserRepository)
        {
            _commentRepository = commentRepository;
            _mapper = mapper;
            _accountService = accountService;
            _companyRepository = companyRepository;
            _companyUserRepository = companyUserRepository;
        }


        public async Task<CommentDTO> Handle(AddCommentCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var valueToAdd = _mapper.Map<Comment>(command);
                var replies = new List<ReplyDTO>();
                var entity = await _commentRepository.AddAsync(valueToAdd);
                string name = "";
                var user = await _accountService.GetUsersById(entity.UserId);
                var result = await _companyUserRepository.GetByUserAsync(entity.UserId);
                if (result != null)
                {
                    var company = await _companyRepository.GetByIdAsync(result.CompanyId);
                    name = company.Name;
                }

                CommentDTO response = new()
                {
                    Id = entity.Id,
                    Content = entity.Content,
                    OwnerName = result == null ? $"{user.FirstName} {user.LastName}" : $"{user.FirstName} {user.LastName}({name})",
                    OwnerImage = user.UrlImage,
                    Replies = replies
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
