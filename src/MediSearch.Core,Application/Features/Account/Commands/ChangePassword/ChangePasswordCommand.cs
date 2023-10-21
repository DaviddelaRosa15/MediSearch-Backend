using MediatR;
using MediSearch.Core.Application.Dtos.Account;
using MediSearch.Core.Application.Interfaces.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace MediSearch.Core.Application.Features.Account.Commands.ChangePassword
{
    public class ChangePasswordCommand : IRequest<ResetPasswordResponse>
	{
        [SwaggerParameter(Description = "Nueva contraseña")]
        [Required(ErrorMessage = "Debe de ingresar la nueva contraseña para su usuario")]
        public string NewPassword { get; set; }
    }

	public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ResetPasswordResponse>
	{
		private readonly IAccountService _accountService;

		public ChangePasswordCommandHandler(IAccountService accountService)
		{
			_accountService = accountService;
		}


		public async Task<ResetPasswordResponse> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
		{
			ResetPasswordResponse response = new();
			try
			{
				response = await _accountService.ChangePasswordAsync(command.NewPassword);
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
