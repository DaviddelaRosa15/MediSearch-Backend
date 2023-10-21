using AutoMapper;
using MediatR;
using MediSearch.Core.Application.Dtos.Account;
using MediSearch.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace MediSearch.Core.Application.Features.Account.Queries.GetRefreshAccessToken
{
    public class GetRefreshAccessTokenQuery : IRequest<RefreshTokenResponse>
	{

	}

	public class GetRefreshAccessTokenQueryHandler : IRequestHandler<GetRefreshAccessTokenQuery, RefreshTokenResponse>
	{

		private readonly IAccountService _accountService;
		private readonly IMapper _mapper;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public GetRefreshAccessTokenQueryHandler(IAccountService accountService, IMapper mapper, IHttpContextAccessor httpContextAccessor)
		{
			_accountService = accountService;
			_mapper = mapper;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<RefreshTokenResponse> Handle(GetRefreshAccessTokenQuery request, CancellationToken cancellationToken)
		{
			RefreshTokenResponse response = new();
			string cookie = _httpContextAccessor.HttpContext.Request.Cookies["refreshToken"];
			if(cookie == null)
			{
				response.HasError = true;
				return response;
			}

			var result = _accountService.ValidateRefreshToken(cookie);

			if(result.Contains("Error") || result == "")
			{
				response.HasError = true;
				return response;
			}

			var refresh = await _accountService.GenerateJWToken(result);
			response.JWToken = refresh;

			return response;
		}

	}
}
