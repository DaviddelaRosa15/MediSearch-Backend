using AutoMapper;
using MediatR;
using MediSearch.Core.Application.Dtos.Account;
using MediSearch.Core.Application.Helpers;
using MediSearch.Core.Application.Interfaces.Repositories;
using MediSearch.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace MediSearch.Core.Application.Features.Account.Commands.RegisterCompany
{
    public class RegisterCompanyCommand : IRequest<RegisterResponse>
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

        [SwaggerParameter(Description = "Ceo")]
        [Required(ErrorMessage = "Debe de ingresar descrpción sobre el dueño de la empresa")]
        public string Ceo { get; set; }

        [SwaggerParameter(Description = "Nombre de la empresa")]
        [Required(ErrorMessage = "Debe de ingresar el nombre de la empresa")]
        public string NameCompany { get; set; }

        [SwaggerParameter(Description = "Imagén de logo")]
        [Required(ErrorMessage = "Debe de ingresar su logo")]
        public IFormFile ImageLogo { get; set; }

        [SwaggerParameter(Description = "Provincia de la empresa")]
        [Required(ErrorMessage = "Debe de ingresar la provincia donde está la empresa")]
        public string ProvinceCompany { get; set; }

        [SwaggerParameter(Description = "Municipio de la empresa")]
        [Required(ErrorMessage = "Debe de ingresar el municipio donde está la empresa")]
        public string MunicipalityCompany { get; set; }

        [SwaggerParameter(Description = "Dirección de la empresa")]
        [Required(ErrorMessage = "Debe de ingresar la dirección de la empresa")]
        public string AddressCompany { get; set; }

        [SwaggerParameter(Description = "Correo de la empresa")]
        [Required(ErrorMessage = "Debe de ingresar el correo de la empresa")]
        public string EmailCompany { get; set; }

        [SwaggerParameter(Description = "Teléfono")]
        [Required(ErrorMessage = "Debe de ingresar el teléfono de la empresa")]
        public string PhoneCompany { get; set; }

        [SwaggerParameter(Description = "Tipo de empresa")]
        [Required(ErrorMessage = "Debe de ingresar el nombre del tipo de empresa")]
        public string CompanyType { get; set; }

        [SwaggerParameter(Description = "Página web")]
        public string? WebSite { get; set; }

        [SwaggerParameter(Description = "Facebook")]
        public string? Facebook { get; set; }

        [SwaggerParameter(Description = "Instagram")]
        public string? Instagram { get; set; }

        [SwaggerParameter(Description = "Twitter")]
        public string? Twitter { get; set; }
    }

	public class RegisterCompanyCommandHandler : IRequestHandler<RegisterCompanyCommand, RegisterResponse>
	{
		private readonly IAccountService _accountService;
		private readonly IMapper _mapper;
		private readonly ICompanyTypeRepository _companyTypeRepository;
		IHttpContextAccessor _httpContextAccessor;

		public RegisterCompanyCommandHandler(IAccountService accountService, IMapper mapper, ICompanyTypeRepository companyTypeRepository, IHttpContextAccessor httpContextAccessor)
		{
			_accountService = accountService;
			_mapper = mapper;
			_companyTypeRepository = companyTypeRepository;
			_httpContextAccessor = httpContextAccessor;
		}


		public async Task<RegisterResponse> Handle(RegisterCompanyCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var origin = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
				var request = _mapper.Map<RegisterCompanyRequest>(command);
				var type = await _companyTypeRepository.GetByNameAsync(command.CompanyType);

				request.CompanyTypeId = type.Id; 
				request.UrlImage = ImageUpload.UploadImageUser(command.Image);
				request.UrlImageLogo = ImageUpload.UploadImageCompany(command.ImageLogo);
				var response = await _accountService.RegisterCompanyAsync(request, origin);
				if(response.HasError)
				{
                    ImageUpload.DeleteFile(request.UrlImage);
                    ImageUpload.DeleteFile(request.UrlImageLogo);
                }
                return response;
			}
			catch (Exception)
			{
				throw new Exception("Ocurrió un error tratando de registrar la empresa.");
			}
		}

	}
}
