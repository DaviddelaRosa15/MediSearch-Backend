using MediatR;
using MediSearch.Core.Application.Dtos.Account;
using MediSearch.Core.Application.Interfaces.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace MediSearch.Core.Application.Features.Account.Commands.ResetPassword
{
    public class ResetPasswordCommand : IRequest<ResetPasswordResponse>
	{
        [SwaggerParameter(Description = "Correo")]
        [Required(ErrorMessage = "Debe de ingresar su correo")]
        public string Email { get; set; }
    }

	public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordResponse>
	{
		private readonly IAccountService _accountService;

		public ResetPasswordCommandHandler(IAccountService accountService)
		{
			_accountService = accountService;
		}


		public async Task<ResetPasswordResponse> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
		{
			ResetPasswordResponse response = new();
			try
			{
				response = await _accountService.ResetPasswordAsync(command.Email);
				return response;
			}
			catch (Exception ex)
			{
				response.HasError = true;
				response.Error = "Ocurrió un error.";
				return response;
			}
		}

	}
}
