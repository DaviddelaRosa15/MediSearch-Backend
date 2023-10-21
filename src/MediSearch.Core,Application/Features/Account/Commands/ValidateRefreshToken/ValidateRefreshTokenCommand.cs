using MediatR;
using MediSearch.Core.Application.Dtos.Account;
using MediSearch.Core.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Features.Account.Commands.ValidateRefreshToken
{
    public class ValidateRefreshTokenCommand : IRequest<ValidateRefreshTokenResponse>
    {
        public string refreshToken { get; set; }
    }

    public class ValidateRefreshTokenCommandHandler : IRequestHandler<ValidateRefreshTokenCommand, ValidateRefreshTokenResponse>
    {
        private readonly IAccountService _accountService;

        public ValidateRefreshTokenCommandHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }


        public async Task<ValidateRefreshTokenResponse> Handle(ValidateRefreshTokenCommand command, CancellationToken cancellationToken)
        {
            ValidateRefreshTokenResponse response = new();
            try
            {
                var result = _accountService.ValidateRefreshToken(command.refreshToken);
                response.ValidRefreshToken = result != "";
                return response;
            }
            catch (Exception ex)
            {
                response.ValidRefreshToken = false;
                return response;
            }
        }

    }
}
