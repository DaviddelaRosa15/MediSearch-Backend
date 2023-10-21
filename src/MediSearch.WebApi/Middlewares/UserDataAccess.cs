using MediSearch.Core.Application.Dtos.Account;
using MediSearch.Core.Application.Interfaces.Services;
using MediSearch.Core.Domain.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MediSearch.WebApi.Middlewares
{
    public class UserDataAccess
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public UserDataAccess(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<UserDTO> GetUserSession()
        {
            UserDTO userDTO = new();

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                // Obtener el servicio necesario dentro del ámbito creado
                var context = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
                var _accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();
                var jwtSettings = scope.ServiceProvider.GetRequiredService<IOptions<JWTSettings>>();
                var _jwtSettings = jwtSettings.Value;

                string? authorizationHeader = context.HttpContext.Request.Headers["Authorization"];
                string token = "";
                if (authorizationHeader != null)
                {
                    token = authorizationHeader.Substring("Bearer ".Length);

                }

                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key))
                };

                try
                {
                    ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                    if (validatedToken == null)
                    {
                        return null;
                    }
                    var id = claimsPrincipal.FindFirst("uid");
                    userDTO = await _accountService.GetUsersById(id.Value);

                    return userDTO;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }            
        }
    }
}
