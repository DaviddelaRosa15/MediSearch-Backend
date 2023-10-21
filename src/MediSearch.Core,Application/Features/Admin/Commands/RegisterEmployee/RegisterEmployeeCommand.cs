using AutoMapper;
using MediatR;
using MediSearch.Core.Application.Dtos.Account;
using MediSearch.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MediSearch.Core.Application.Features.Admin.Commands.RegisterEmployee
{
    public class RegisterEmployeeCommand : IRequest<RegisterResponse>
    {
        [SwaggerParameter(Description = "Nombre")]
        [Required(ErrorMessage = "Debe de ingresar su nombre")]
        public string FirstName { get; set; }

        [SwaggerParameter(Description = "Apellido")]
        [Required(ErrorMessage = "Debe de ingresar su apellido")]
        public string LastName { get; set; }

        [SwaggerParameter(Description = "Teléfono")]
        [Required(ErrorMessage = "Debe de ingresar su telefono")]
        public string PhoneNumber { get; set; }

        [SwaggerParameter(Description = "Correo")]
        [Required(ErrorMessage = "Debe de ingresar su correo")]
        public string Email { get; set; }

        [SwaggerParameter(Description = "Rol")]
        [Required(ErrorMessage = "Debe de ingresar el rol")]
        public string Role { get; set; }

        [SwaggerParameter(Description = "Provincia")]
        [Required(ErrorMessage = "Debe de ingresar su provincia")]
        public string Province { get; set; }

        [SwaggerParameter(Description = "Municìpio")]
        [Required(ErrorMessage = "Debe de ingresar su municipio")]
        public string Municipality { get; set; }

        [SwaggerParameter(Description = "Dirección")]
        [Required(ErrorMessage = "Debe de ingresar su dirección")]
        public string Address { get; set; }
        [JsonIgnore]
        public string? CompanyId { get; set; }
    }

    public class RegisterEmployeeCommandHandler : IRequestHandler<RegisterEmployeeCommand, RegisterResponse>
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        IHttpContextAccessor _httpContextAccessor;

        public RegisterEmployeeCommandHandler(IAccountService accountService, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _accountService = accountService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<RegisterResponse> Handle(RegisterEmployeeCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var request = _mapper.Map<RegisterEmployeeRequest>(command);
                var response = await _accountService.RegisterEmployeeAsync(request);

                return response;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un error tratando de registrar el usuario.");
            }
        }

    }

}
