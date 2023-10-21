using MediSearch.Core.Application.Dtos.Account;
using MediSearch.Core.Application.Dtos.Email;
using MediSearch.Core.Application.Enums;
using MediSearch.Core.Application.Helpers;
using MediSearch.Core.Application.Interfaces.Repositories;
using MediSearch.Core.Application.Interfaces.Services;
using MediSearch.Core.Domain.Entities;
using MediSearch.Core.Domain.Settings;
using MediSearch.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Specialized;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MediSearch.Infrastructure.Identity.Services
{
    public class AccountService : IAccountService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly JWTSettings _jwtSettings;
        private readonly RefreshJWTSettings _refreshSettings;
        IHttpContextAccessor _httpContextAccessor;
        private readonly ICompanyRepository _companyRepository;
        private readonly ICompanyUserRepository _companyUserRepository;
        private readonly ICompanyTypeRepository _companyTypeRepository;


        public AccountService(
              UserManager<ApplicationUser> userManager,
              SignInManager<ApplicationUser> signInManager,
              IEmailService emailService,
              IOptions<JWTSettings> jwtSettings,
              IOptions<RefreshJWTSettings> refreshSettings,
              IHttpContextAccessor httpContextAccessor,
              ICompanyRepository companyRepository,
              ICompanyUserRepository companyUserRepository,
              ICompanyTypeRepository companyTypeRepository
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _jwtSettings = jwtSettings.Value;
            _refreshSettings = refreshSettings.Value;
            _httpContextAccessor = httpContextAccessor;
            _companyRepository = companyRepository;
            _companyUserRepository = companyUserRepository;
            _companyTypeRepository = companyTypeRepository;
        }

        public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request)
        {
            AuthenticationResponse response = new();
            var users = GetAllUsers();

            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                response.HasError = true;
                response.Error = $"No existe una cuenta registrada con este usuario: {request.UserName}";
                return response;
            }

            var isConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            if (!isConfirmed)
            {
                response.HasError = true;
                response.Error = "El usuario no ha confirmado su cuenta. Revise su correo electrónico";
                return response;
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                response.HasError = true;
                response.Error = $"Usuario o contraseña inválidos";
                return response;
            }

            response.JWToken = await GenerateJWToken(user.Id);
            response.RefreshToken = GenerateRefreshToken(user.Id);

            return response;
        }

        public async Task<RegisterResponse> RegisterClientUserAsync(RegisterRequest request, string origin)
        {
            RegisterResponse response = await ValidateUserBeforeRegistrationAsync(request);
            if (response.HasError)
            {
                return response;
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                UrlImage = request.UrlImage,
                Province = request.Province,
                Municipality = request.Municipality,
                Address = request.Address
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Roles.Client.ToString());
                var verificationUri = await SendVerificationEmailUri(user, origin);
                await _emailService.SendAsync(new EmailRequest()
                {
                    To = user.Email,
                    Body = MakeEmailForConfirm(verificationUri, user.FirstName + " " + user.LastName),
                    Subject = "Confirmar Cuenta"
                });
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    response.Error += $"Error: {error.Description}";
                }
                response.HasError = true;
                return response;
            }
            response.IsSuccess = true;
            return response;
        }

        public async Task<RegisterResponse> RegisterCompanyAsync(RegisterCompanyRequest request, string origin)
        {
            RegisterResponse response = await ValidateUserBeforeRegistrationAsync(request);
            if (response.HasError)
            {
                return response;
            }

            var result = await _companyRepository.GetByNameAsync(request.NameCompany);
            if (result != null)
            {
                response.HasError = true;
                response.Error = $"El nombre de empresa '{request.NameCompany}' ya está siendo usado.";
            }

            result = await _companyRepository.GetByEmailAsync(request.EmailCompany);
            if (result != null)
            {
                response.HasError = true;
                response.Error = $"El correo '{request.EmailCompany}' ya está siendo usado.";
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                UrlImage = request.UrlImage,
                Province = request.Province,
                Municipality = request.Municipality,
                Address = request.Address
            };

            var company = new Company()
            {
                Ceo = request.Ceo,
                Email = request.EmailCompany,
                Name = request.NameCompany,
                UrlImage = request.UrlImageLogo,
                Facebook = request.Facebook,
                Twitter = request.Twitter,
                Instagram = request.Instagram,
                Province = request.ProvinceCompany,
                Municipality = request.MunicipalityCompany,
                Address = request.AddressCompany,
                Phone = request.PhoneCompany,
                WebSite = request.WebSite,
                CompanyTypeId = request.CompanyTypeId
            };

            var success = await _userManager.CreateAsync(user, request.Password);
            if (success.Succeeded)
            {
                try
                {
                    await _userManager.AddToRoleAsync(user, Roles.SuperAdmin.ToString());
                    var entity = await _companyRepository.AddAsync(company);
                    var companyUser = new CompanyUser()
                    {
                        UserId = user.Id,
                        CompanyId = entity.Id
                    };
                    await _companyUserRepository.AddAsync(companyUser);

                    var verificationUri = await SendVerificationEmailUri(user, origin);
                    await _emailService.SendAsync(new EmailRequest()
                    {
                        To = user.Email,
                        Body = MakeEmailForConfirm(verificationUri, user.FirstName + " " + user.LastName),
                        Subject = "Confirmar Cuenta"
                    });
                }
                catch (Exception ex)
                {
                    response.HasError = true;
                    response.Error = "Error: " + ex.Message;
                    return response;
                }
            }
            else
            {
                foreach (var error in success.Errors)
                {
                    response.Error += $"{error.Description}";
                }
                response.HasError = true;

                return response;
            }
            response.IsSuccess = true;
            return response;
        }

        public async Task<RegisterResponse> RegisterEmployeeAsync(RegisterEmployeeRequest request)
        {
            RegisterResponse response = new()
            {
                HasError = false
            };

            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail != null)
            {
                response.HasError = true;
                response.Error = $"El correo '{request.Email}' ya está siendo usado.";
                return response;
            }

            var company = await _companyRepository.GetByIdAsync(request.CompanyId);
            string random = Nanoid.Nanoid.Generate("0123456789", 4);
            string userName = request.FirstName + random;
            string password = $"M{company.Created.Day}#{company.Name.Substring(0, 5)}";

            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = userName,
                PhoneNumber = request.PhoneNumber,
                UrlImage = "/Assets/Images/default.jpg",
                Province = request.Province,
                Municipality = request.Municipality,
                Address = request.Address,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, request.Role);

                var companyUser = new CompanyUser()
                {
                    UserId = user.Id,
                    CompanyId = company.Id
                };
                await _companyUserRepository.AddAsync(companyUser);

                await _emailService.SendAsync(new EmailRequest()
                {
                    To = user.Email,
                    Body = MakeEmailForEmployee(new List<string> { user.FirstName + " " + user.LastName, userName, password, company.Name, request.Role }),
                    Subject = "Registro Exitoso"
                });
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    response.Error += $"Error: {error.Description}";
                }
                response.HasError = true;
                return response;
            }
            response.IsSuccess = true;
            return response;
        }

        public async Task<ConfirmEmailResponse> ConfirmEmailAsync(string userId, string token)
        {
            ConfirmEmailResponse response = new()
            {
                HasError = false
            };
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                response.HasError = true;
                response.Error = "No existe cuenta registrada con este usuario";
                return response;
            }

            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                await _emailService.SendAsync(new EmailRequest()
                {
                    To = user.Email,
                    Body = MakeEmailForConfirmed(user.FirstName + " " + user.LastName),
                    Subject = "Cuenta Confirmada"
                });
                response.NameUser = user.FirstName + " " + user.LastName;
                return response;
            }
            else
            {
                response.HasError = true;
                response.Error = $"Ocurrió un error mientras se confirmaba la cuenta para el correo: {user.Email}";
                return response;
            }
        }

        public async Task<ResetPasswordResponse> ResetPasswordAsync(string email)
        {
            ResetPasswordResponse response = new()
            {
                HasError = false
            };

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                response.HasError = true;
                response.Error = "No existe cuenta registrada con este correo";
                return response;
            }

            Guid guid = Guid.NewGuid();
            string format = guid.ToString();
            string code = format.Replace("-", "");
            code = code.Substring(5, 6);

            _httpContextAccessor.HttpContext.Session.SetString("confirmCode", code);
            _httpContextAccessor.HttpContext.Session.SetString("user", user.Id);

            response.IsSuccess = true;
            await _emailService.SendAsync(new EmailRequest()
            {
                To = user.Email,
                Body = MakeEmailForReset(user, code),
                Subject = "Código de Confirmación"
            });

            return response;
        }

        public async Task<ResetPasswordResponse> ChangePasswordAsync(string password)
        {
            ResetPasswordResponse response = new()
            {
                HasError = false
            };

            var userId = _httpContextAccessor.HttpContext.Session.GetString("user");
            if (userId == null)
            {
                response.HasError = true;
                response.Error = "Sucedió un error en el sistema";
                return response;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                response.HasError = true;
                response.Error = "Sucedió un error en el sistema";
                return response;
            }
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, resetToken, password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    response.Error += $"{error.Description}";
                }
                response.HasError = true;

                return response;
            }

            response.IsSuccess = true;
            await _emailService.SendAsync(new EmailRequest()
            {
                To = user.Email,
                Body = MakeEmailForChange(user),
                Subject = "Cambio de Contraseña"
            });

            return response;
        }

        public async Task<UserDTO> ValidateEmployee(string id, string company)
        {
            var user = await _companyUserRepository.ValidateEmployeAsync(company, id);

            if (!user)
            {
                return null;
            }

            var appUser = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(appUser);

            UserDTO dto = new()
            {
                Id = appUser.Id,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                Email = appUser.Email,
                PhoneNumber = appUser.PhoneNumber,
                UrlImage = appUser.UrlImage,
                Address = appUser.Address,
                Province = appUser.Province,
                Municipality = appUser.Municipality,
                Role = roles.First(),
                CompanyId = company
            };

            return dto;
        }

        public async Task DeleteUserAsync(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);

            var company = await _companyUserRepository.GetByUserAsync(id);
            if (company != null)
            {
                 await _companyUserRepository.DeleteAsync(company);
            }
        }

        public ConfirmCodeResponse ConfirmCode(string code)
        {
            ConfirmCodeResponse response = new()
            {
                HasError = false
            };

            var confirm = _httpContextAccessor.HttpContext.Session.GetString("confirmCode");
            if (confirm == null)
            {
                response.HasError = true;
                response.Error = "Ocurrió un error confirmando el código";
                return response;
            }

            if (!confirm.Equals(code))
            {
                response.HasError = true;
                response.Error = "El código de confirmación no es correcto";
                return response;
            }

            response.IsSuccess = true;
            return response;
        }

        public async Task<string> GenerateJWToken(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var isEmployee = await _companyUserRepository.GetByUserAsync(userId);
            string roletype = "Cliente";
            string nameCompany = "Cliente";
            if(isEmployee != null){
                var company = await _companyRepository.GetByIdAsync(isEmployee.CompanyId);
                var type = await _companyTypeRepository.GetByIdAsync(company.CompanyTypeId);
                roletype = type.Name;
                nameCompany = company.Name;
            }

            var roleClaims = new List<Claim>();

            foreach (var role in roles)
            {
                roleClaims.Add(new Claim("roles", role));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim("uid", user.Id),
                new Claim("UrlImage", user.UrlImage),
                new Claim("RoleType", roletype),
                new Claim("Company", nameCompany)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmectricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredetials = new SigningCredentials(symmectricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredetials);


            string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return token;
        }

        public string GenerateRefreshToken(string userId)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", userId)
            };

            var symmectricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_refreshSettings.Key));
            var signingCredetials = new SigningCredentials(symmectricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _refreshSettings.Issuer,
                audience: _refreshSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_refreshSettings.DurationInDays),
                signingCredentials: signingCredetials);

            string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;
        }

        public string ValidateRefreshToken(string token)
        {
            string userId = "";
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = _refreshSettings.Issuer,
                ValidAudience = _refreshSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_refreshSettings.Key))
            };

            try
            {
                ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                if (validatedToken == null)
                {
                    return "Error: El token no es válido";
                }
                var id = claimsPrincipal.FindFirst("uid");
                userId = id.Value;
            }
            catch (SecurityTokenValidationException ex)
            {
                return "Error de validación del token JWT: " + ex.Message;
            }
            catch (Exception ex)
            {
                return "Error al decodificar el token JWT: " + ex.Message;
            }

            return userId;
        }

        public async Task<List<UserDTO>> GetUsersByCompany(string id)
        {
            List<UserDTO> userDTOs = new();
            var company = await _companyUserRepository.GetByCompanyAsync(id);

            if (company == null || company.Count == 0)
            {
                return null;
            }

            foreach (var item in company)
            {
                var user = await _userManager.FindByIdAsync(item.UserId);
                var roles = await _userManager.GetRolesAsync(user);

                UserDTO dto = new()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    UrlImage = user.UrlImage,
                    Address = user.Address,
                    Province = user.Province,
                    Municipality = user.Municipality,
                    Role = roles.First(),
                    CompanyId = id
                };

                userDTOs.Add(dto);
            }

            return userDTOs;
        }

        public async Task<UserDTO> GetUsersById(string id)
        {
            var company = await _companyUserRepository.GetByUserAsync(id);
            var user = await _userManager.FindByIdAsync(id);

            if(user == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            UserDTO dto = new()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UrlImage = user.UrlImage,
                Address = user.Address,
                Province = user.Province,
                Municipality = user.Municipality,
                Role = roles.First(),
                CompanyId = company == null ? "Client" : company.CompanyId
            };

            return dto;
        }

        public async Task<RegisterResponse> EditProfile(UserDTO user)
        {
            RegisterResponse response = new();
            var userToUpdate = await _userManager.FindByIdAsync(user.Id);

            userToUpdate.FirstName = user.FirstName;
            userToUpdate.LastName = user.LastName;
            userToUpdate.UrlImage = user.UrlImage;
            userToUpdate.Address = user.Address;
            userToUpdate.Province = user.Province;
            userToUpdate.Municipality = user.Municipality;

            try
            {
                await _userManager.UpdateAsync(userToUpdate);
            }catch(Exception ex)
            {
                return new RegisterResponse()
                {
                    HasError = true,
                    Error = ex.Message
                };
            }
            

            response.IsSuccess = true;
            return response;
        }

        #region Private Methods
        private async Task<RegisterResponse> ValidateUserBeforeRegistrationAsync(RegisterRequest request)
        {
            RegisterResponse response = new()
            {
                HasError = false
            };
            var user = _userManager.Users.ToList();

            var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
            if (userWithSameUserName != null)
            {
                response.HasError = true;
                response.Error = $"El nombre de usuario '{request.UserName}' ya está siendo usado.";
                return response;
            }

            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail != null)
            {
                response.HasError = true;
                response.Error = $"El correo '{request.Email}' ya está siendo usado.";
                return response;
            }

            return response;
        }

        private async Task<string> SendVerificationEmailUri(ApplicationUser user, string origin)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "api/v1/Account/confirm-email";
            var Uri = new Uri(string.Concat($"{origin}/", route));
            var verificationUri = QueryHelpers.AddQueryString(Uri.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "token", code);

            return verificationUri;
        }

        private string MakeEmailForConfirm(string verificationUri, string user)
        {
            string htmlBody = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Confirmar Cuenta</title>
    <style>
        /* Estilos adicionales */
        body {
            font-family: Arial, sans-serif;
        }
        .container {
            max-width: 400px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f5f5f5;
            border-radius: 5px;
        }
        .header {
            text-align: center;
            margin-bottom: 20px;
        }
        .title {
            font-size: 24px;
            margin-bottom: 10px;
        }
        .message {
            font-size: 16px;
            margin-bottom: 20px;
        }
        .button {
            display: inline-block;
            font-weight: 400;
            text-align: center;
            white-space: nowrap;
            vertical-align: middle;
            user-select: none;
            border: 1px solid transparent;
            padding: 0.375rem 0.75rem;
            font-size: 1rem;
            line-height: 1.5;
            border-radius: 0.25rem;
            transition: color 0.15s ease-in-out, background-color 0.15s ease-in-out, border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
            color: #fff;
            background-color: #007bff;
            border-color: #007bff;
            text-decoration: none;
        }
        .footer {
            text-align: center;
            font-size: 14px;
        }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1 class='title'>Confirmar Cuenta</h1>
        </div>
        <div class='message'>
            <p>Hola [Nombre],</p>
            <p>Gracias por registrarte en nuestro sitio. Para completar el proceso de registro, por favor, haz clic en el siguiente botón:</p>
            <p><a href='[URL]' class='button'>Confirmar Cuenta</a></p>
        </div>
        <div class='footer'>
            <p>Atentamente,</p>
            <p>El equipo de MediSearch</p>
        </div>
    </div>
</body>
</html>
";

            string html = htmlBody.Replace("[URL]", verificationUri);
            html = html.Replace("[Nombre]", user);
            return html;
        }

        private string MakeEmailForConfirmed(string user)
        {
            string htmlBody = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Cuenta Confirmada</title>
    <style>
        /* Estilos adicionales */
        body {
            font-family: Arial, sans-serif;
        }
        .container {
            max-width: 400px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f5f5f5;
            border-radius: 5px;
        }
        .header {
            text-align: center;
            margin-bottom: 20px;
        }
        .title {
            font-size: 24px;
            margin-bottom: 10px;
        }
        .message {
            font-size: 16px;
            margin-bottom: 20px;
        }
        .footer {
            text-align: center;
            font-size: 14px;
        }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1 class='title'>¡Cuenta Confirmada!</h1>
        </div>
        <div class='message'>
            <p>Hola Nombre,</p>
            <p>Te damos la bienvenida a nuestra comunidad. Gracias por confirmar tu cuenta.</p>
            <p>Si tienes alguna pregunta o necesitas ayuda, no dudes en contactarnos.</p>
            <p>¡Disfruta de la aplicación y que tengas un gran día!</p>
        </div>
        <div class='footer'>
            <p>Atentamente,</p>
            <p>El equipo de MediSearch</p>
        </div>
    </div>
</body>
</html>
";
            string html = htmlBody.Replace("Nombre", user);
            return html;
        }

        private string MakeEmailForReset(ApplicationUser user, string code)
        {
            string htmlBody = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Código de Confirmación</title>
    <style>
        /* Estilos adicionales */
        body {
            font-family: Arial, sans-serif;
        }
        .container {
            max-width: 400px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f5f5f5;
            border-radius: 5px;
        }
        .header {
            text-align: center;
            margin-bottom: 20px;
        }
        .title {
            font-size: 24px;
            margin-bottom: 10px;
        }
        .message {
            font-size: 16px;
            margin-bottom: 20px;
        }
        .code {
            font-size: 32px;
            text-align: center;
            margin-bottom: 20px;
        }
        .footer {
            text-align: center;
            font-size: 14px;
        }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1 class='title'>Código de Confirmación</h1>
        </div>
        <div class='message'>
            <p>Hola [Nombre],</p>
            <p>Aquí tienes tu código de confirmación:</p>
        </div>
        <div class='code'>
            <p>[Código]</p>
        </div>
        <div class='footer'>
            <p>Por favor, ingresa este código para poder continuar con el proceso de restablecer la contraseña.</p>
			<p>Si no fuiste tú quien solicitó esta acción, revisa tu cuenta</p>
            <p>Atentamente,</p>
            <p>El equipo de MediSearch</p>
        </div>
    </div>
</body>
</html>
";

            string html = htmlBody.Replace("[Nombre]", user.FirstName + " " + user.LastName);
            html = html.Replace("[Código]", code);
            return html;
        }

        private string MakeEmailForChange(ApplicationUser user)
        {
            string htmlBody = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Cambio de Contraseña</title>
    <style>
        /* Estilos adicionales */
        body {
            font-family: Arial, sans-serif;
        }
        .container {
            max-width: 400px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f5f5f5;
            border-radius: 5px;
        }
        .header {
            text-align: center;
            margin-bottom: 20px;
        }
        .title {
            font-size: 24px;
            margin-bottom: 10px;
        }
        .message {
            font-size: 16px;
            margin-bottom: 20px;
        }
        .footer {
            text-align: center;
            font-size: 14px;
        }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1 class='title'>Cambio de Contraseña</h1>
        </div>
        <div class='message'>
            <p>Hola [Nombre],</p>
            <p>Tu contraseña ha sido cambiada correctamente.</p>
            <p>Si no realizaste este cambio, por favor, ponte en contacto con nosotros lo antes posible.</p>
        </div>
        <div class='footer'>
            <p>Atentamente,</p>
            <p>El equipo de MediSearch</p>
        </div>
    </div>
</body>
</html>
";

            string html = htmlBody.Replace("[Nombre]", user.FirstName + " " + user.LastName);
            return html;
        }

        private string MakeEmailForEmployee(List<string> requirements)
        {
            string htmlBody = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Registro Exitoso</title>
    <style>
        /* Estilos adicionales */
        body {
            font-family: Arial, sans-serif;
        }
        .container {
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f5f5f5;
            border-radius: 10px;
        }
        .header {
            text-align: center;
            margin-bottom: 20px;
        }
        .title {
            font-size: 24px;
            margin-bottom: 10px;
        }
        .message {
            font-size: 16px;
            margin-bottom: 20px;
        }
        .details {
            margin-bottom: 10px;
        }
        .details p {
            margin: 5px 0;
        }
        .password {
            font-weight: bold;
        }
        .warning {
            font-style: italic;
            color: red;
        }
        .button {
            display: inline-block;
            font-weight: 400;
            text-align: center;
            white-space: nowrap;
            vertical-align: middle;
            user-select: none;
            border: 1px solid transparent;
            padding: 0.375rem 0.75rem;
            font-size: 1rem;
            line-height: 1.5;
            border-radius: 0.25rem;
            transition: color 0.15s ease-in-out, background-color 0.15s ease-in-out, border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
            color: #fff;
            background-color: #007bff;
            border-color: #007bff;
            text-decoration: none;
        }
        .footer {
            text-align: center;
            font-size: 14px;
        }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1 class='title'>¡Registro Exitoso!</h1>
        </div>
        <div class='message'>
            <p>Estimado/a [Nombre],</p>
            <p>Te informamos que has sido registrado/a correctamente en [Empresa].</p>
            <div class='details'>
                <p><strong>Empresa:</strong> [Empresa]</p>
                <p><strong>Rol:</strong> [Rol]</p>
                <p><strong>Nombre de Usuario:</strong> [Usuario]</p>
                <p><strong>Contraseña por Defecto:</strong> <span class='password'>[Contraseña]</span></p>
            </div>
            <p class='warning'>Te recomendamos que cambies tu contraseña por defecto por motivos de seguridad.</p>
            <p>Puedes cambiarla desde la sección de olvidaste la contraseña de la página de inicio de sesión</p>
            <p>Ya puedes acceder al sistema y disfrutar de todas sus funcionalidades:</p>
            <p><a href='[URL]' class='button'>Ir al Sistema</a></p>
        </div>
        <div class='footer'>
            <p>Atentamente,</p>
            <p>El equipo de MediSearch</p>
        </div>
    </div>
</body>
</html>
";

            string html = htmlBody.Replace("[Nombre]", requirements[0]);
            html = html.Replace("[Usuario]", requirements[1]);
            html = html.Replace("[Contraseña]", requirements[2]);
            html = html.Replace("[Empresa]", requirements[3]);
            html = html.Replace("[Rol]", requirements[4]);
            return html;
        }

        private async Task<List<ApplicationUser>> GetAllUsers()
        {
            var list = _userManager.Users.ToList();

            return list;
        }
        #endregion

    }
}
