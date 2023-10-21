using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Features.Admin.Commands.EditProfileCompany
{
    public class EditProfileCompanyCommandRequest
    {
        [SwaggerParameter(Description = "Ceo")]
        [Required(ErrorMessage = "Debe de ingresar el ceo")]
        public string Ceo { get; set; }

        [SwaggerParameter(Description = "Nombre")]
        [Required(ErrorMessage = "Debe de ingresar el nombre")]
        public string Name { get; set; }

        [SwaggerParameter(Description = "Correo")]
        [Required(ErrorMessage = "Debe de ingresar el correo")]
        public string Email { get; set; }

        [SwaggerParameter(Description = "Teléfono")]
        [Required(ErrorMessage = "Debe de ingresar el teléfono")]
        public string Phone { get; set; }

        [SwaggerParameter(Description = "Logo")]
        public IFormFile? Logo { get; set; }

        [SwaggerParameter(Description = "Provincia")]
        [Required(ErrorMessage = "Debe de ingresar su provincia")]
        public string Province { get; set; }

        [SwaggerParameter(Description = "Municìpio")]
        [Required(ErrorMessage = "Debe de ingresar su municipio")]
        public string Municipality { get; set; }

        [SwaggerParameter(Description = "Dirección")]
        [Required(ErrorMessage = "Debe de ingresar su dirección")]
        public string Address { get; set; }

        [SwaggerParameter(Description = "Sitio Web")]
        public string? WebSite { get; set; }

        [SwaggerParameter(Description = "Facebook")]
        public string? Facebook { get; set; }

        [SwaggerParameter(Description = "Instagram")]
        public string? Instagram { get; set; }

        [SwaggerParameter(Description = "Twitter")]
        public string? Twitter { get; set; }
    }
}
