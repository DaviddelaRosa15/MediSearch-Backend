using AutoMapper;
using MediatR;
using MediSearch.Core.Application.Dtos.Account;
using MediSearch.Core.Application.Interfaces.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace MediSearch.Core.Application.Features.Account.Commands.Authenticate
{
    public class AuthenticateCommand : IRequest<AuthenticationResponse>
	{
		[SwaggerParameter(Description = "Nombre de usuario")]
		[Required(ErrorMessage = "Debe de ingresar su nombre de usuario")]
		public string UserName { get; set; }

		[SwaggerParameter(Description = "Contraseña")]
		[Required(ErrorMessage = "Debe de ingresar su contraseña")]
		public string Password { get; set; }
	}

	public class AuthenticateCommandHandler : IRequestHandler<AuthenticateCommand, AuthenticationResponse>
	{
		private readonly IAccountService _accountService;
		private readonly IMapper _mapper;

		public AuthenticateCommandHandler(IAccountService accountService, IMapper mapper)
		{
			_accountService = accountService;
			_mapper = mapper;
		}


		public async Task<AuthenticationResponse> Handle(AuthenticateCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var request = _mapper.Map<AuthenticationRequest>(command);
				var response = await _accountService.AuthenticateAsync(request);
				return response;
			}
			catch (Exception)
			{
				throw new Exception("Ocurrió un error tratando de autenticar el usuario.");
			}
		}

	}
}
