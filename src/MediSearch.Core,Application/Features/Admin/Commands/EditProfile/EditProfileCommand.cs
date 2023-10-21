using AutoMapper;
using MediatR;
using MediSearch.Core.Application.Dtos.Account;
using MediSearch.Core.Application.Helpers;
using MediSearch.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Features.Admin.Commands.EditProfile
{
    public class EditProfileCommand : IRequest<RegisterResponse>
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IFormFile? Image { get; set; }
        public string Province { get; set; }
        public string Municipality { get; set; }
        public string Address { get; set; }
    }

    public class EditProfileCommandHandler : IRequestHandler<EditProfileCommand, RegisterResponse>
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public EditProfileCommandHandler(IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }


        public async Task<RegisterResponse> Handle(EditProfileCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _accountService.GetUsersById(command.Id);
                if (user == null)
                {
                    return new RegisterResponse()
                    {
                        HasError = false,
                        Error = "Usuario no encontrado"
                    };
                }

                var request = _mapper.Map<UserDTO>(command);
                request.UrlImage = ImageUpload.UploadImageUser(command.Image, true, user.UrlImage);

                var response = await _accountService.EditProfile(request);
                if (response.HasError)
                {
                    ImageUpload.DeleteFile(request.UrlImage);
                    return response;
                }

                return response;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un error tratando de actualizar el usuario.");
            }
        }

    }
}
