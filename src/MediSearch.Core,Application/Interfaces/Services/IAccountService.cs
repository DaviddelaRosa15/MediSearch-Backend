using MediSearch.Core.Application.Dtos.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Interfaces.Services
{
	public interface IAccountService
	{
		Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request);
		Task<RegisterResponse> RegisterClientUserAsync(RegisterRequest request, string origin);
		Task<RegisterResponse> RegisterCompanyAsync(RegisterCompanyRequest request, string origin);
		Task<RegisterResponse> RegisterEmployeeAsync(RegisterEmployeeRequest request);
		Task<UserDTO> ValidateEmployee(string id, string company);
		Task DeleteUserAsync(string id);
        Task<ConfirmEmailResponse> ConfirmEmailAsync(string userId, string token);
		Task<ResetPasswordResponse> ResetPasswordAsync(string email);
		ConfirmCodeResponse ConfirmCode(string code);
		Task<ResetPasswordResponse> ChangePasswordAsync(string password);
		Task<string> GenerateJWToken(string userId);
		string GenerateRefreshToken(string userId);
		string ValidateRefreshToken(string token);
		Task<List<UserDTO>> GetUsersByCompany(string id);
		Task<UserDTO> GetUsersById(string id);
		Task<RegisterResponse> EditProfile(UserDTO user);

    }
}
