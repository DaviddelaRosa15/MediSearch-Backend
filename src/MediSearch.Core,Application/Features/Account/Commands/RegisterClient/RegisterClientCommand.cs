using AutoMapper;
using MediatR;
using MediSearch.Core.Application.Dtos.Account;
using MediSearch.Core.Application.Helpers;
using MediSearch.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace MediSearch.Core.Application.Features.Account.Commands.RegisterClient
{
    public class RegisterClientCommand : IRequest<RegisterResponse>
	{
		[SwaggerParameter(Description = "Nombre")]
		[Required(ErrorMessage = "Debe de ingresar su nombre")]
		public string FirstName { get; set; }

		[SwaggerParameter(Description = "Apellido")]
		[Required(ErrorMessage = "Debe de ingresar su apellido")]
		public string LastName { get; set; }

		[SwaggerParameter(Description = "Nombre de usuario")]
		[Required(ErrorMessage = "Debe de ingresar su nombre de usuario")]
		public string UserName { get; set; }

		[SwaggerParameter(Description = "Contraseña")]
		[Required(ErrorMessage = "Debe de ingresar su contraseña")]
		public string Password { get; set; }

		[SwaggerParameter(Description = "Teléfono")]
		[Required(ErrorMessage = "Debe de ingresar su telefono")]
		public string PhoneNumber { get; set; }

		[SwaggerParameter(Description = "Correo")]
		[Required(ErrorMessage = "Debe de ingresar su correo")]
		public string Email { get; set; }

		[SwaggerParameter(Description = "Foto de perfil")]
		[Required(ErrorMessage = "Debe de subir una foto suya")]
		public IFormFile Image { get; set; }

		[SwaggerParameter(Description = "Provincia")]
		[Required(ErrorMessage = "Debe de ingresar su provincia")]
		public string Province { get; set; }

		[SwaggerParameter(Description = "Municìpio")]
		[Required(ErrorMessage = "Debe de ingresar su municipio")]
		public string Municipality { get; set; }

		[SwaggerParameter(Description = "Dirección")]
		[Required(ErrorMessage = "Debe de ingresar su dirección")]
		public string Address { get; set; }
	}

	public class RegisterClientCommandHandler : IRequestHandler<RegisterClientCommand, RegisterResponse>
	{
		private readonly IAccountService _accountService;
		private readonly IMapper _mapper;
		IHttpContextAccessor _httpContextAccessor;

		public RegisterClientCommandHandler(IAccountService accountService, IMapper mapper, IHttpContextAccessor httpContextAccessor)
		{
			_accountService = accountService;
			_mapper = mapper;
			_httpContextAccessor = httpContextAccessor;
		}


		public async Task<RegisterResponse> Handle(RegisterClientCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var origin = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
                var request = _mapper.Map<RegisterRequest>(command);
				request.UrlImage = ImageUpload.UploadImageUser(command.Image);
				var response = await _accountService.RegisterClientUserAsync(request, origin);
				if (response.HasError)
				{
                    ImageUpload.DeleteFile(request.UrlImage);
                }

                return response;
			}
			catch (Exception)
			{
				throw new Exception("Ocurrió un error tratando de registrar el usuario.");
			}
		}

	}
}
