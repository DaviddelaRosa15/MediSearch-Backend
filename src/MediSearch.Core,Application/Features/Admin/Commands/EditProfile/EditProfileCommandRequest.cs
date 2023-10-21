using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Features.Admin.Commands.EditProfile
{
    public class EditProfileCommandRequest
    {
        [SwaggerParameter(Description = "Nombre")]
        [Required(ErrorMessage = "Debe de ingresar su nombre")]
        public string FirstName { get; set; }

        [SwaggerParameter(Description = "Apellido")]
        [Required(ErrorMessage = "Debe de ingresar su apellido")]
        public string LastName { get; set; }

        [SwaggerParameter(Description = "Foto de perfil")]
        public IFormFile? Image { get; set; }

        [SwaggerParameter(Description = "Provincia")]
        [Required(ErrorMessage = "Debe de ingresar su provincia")]
        public string Province { get; set; }

        [SwaggerParameter(Description = "Municìpio")]
        [Required(ErrorMessage = "Debe de ingresar su municipio")]
        public string Municipality { get; set; }

        [SwaggerParameter(Description = "Dirección")]
        [Required(ErrorMessage = "Debe de ingresar su dirección")]
        public string Address { get; set; }
    }
}
