using AutoMapper;
using MediSearch.Core.Application.Dtos.Account;
using MediSearch.Core.Application.Features.Admin.Commands.DeleteEmployee;
using MediSearch.Core.Application.Features.Admin.Commands.EditProfile;
using MediSearch.Core.Application.Features.Admin.Commands.EditProfileCompany;
using MediSearch.Core.Application.Features.Admin.Commands.RegisterEmployee;
using MediSearch.Core.Application.Features.Admin.Queries.GetDataDashboard;
using MediSearch.Core.Application.Features.Admin.Queries.GetProfile;
using MediSearch.Core.Application.Features.Admin.Queries.GetProfileCompany;
using MediSearch.Core.Application.Features.Admin.Queries.GetUsersCompany;
using MediSearch.WebApi.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;

namespace MediSearch.WebApi.Controllers.v1
{
    [SwaggerTag("Administración de empresa")]
    public class AdminController : BaseApiController
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;
        public AdminController(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _mapper = mapper;
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet("get-all-employees")]
        [SwaggerOperation(
            Summary = "Todos los empleados de la empresa.",
            Description = "Permite obtener todos los empleados que tiene la empresa registrados en el sistema."
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                UserDataAccess userData = new(_serviceScopeFactory);
                var user = await userData.GetUserSession();
                var result = await Mediator.Send(new GetUsersCompanyQuery() { CompanyId = user.CompanyId });

                if (result == null || result.Count == 0)
                    return NotFound("Esta empresa no tiene empleados registrados");

                return Ok(result);

            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        [Authorize]
        [HttpGet("get-profile")]
        [SwaggerOperation(
            Summary = "Datos del usuario logueado.",
            Description = "Permite obtener los datos que tiene el usuario en el sistema."
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProfileQueryResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                UserDataAccess userData = new(_serviceScopeFactory);
                var user = await userData.GetUserSession();
                var result = await Mediator.Send(new GetProfileQuery() { Id = user.Id });

                if (result == null)
                    return NotFound("No se encontró el usuario");

                return Ok(result);

            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }
        
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet("get-profile-company")]
        [SwaggerOperation(
            Summary = "Datos de la empresa del usuario logueado.",
            Description = "Permite obtener los datos que tiene la empresa en el sistema."
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProfileCompanyQueryResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProfileCompany()
        {
            try
            {
                UserDataAccess userData = new(_serviceScopeFactory);
                var user = await userData.GetUserSession();
                var result = await Mediator.Send(new GetProfileCompanyQuery() { Id = user.CompanyId });

                if (result == null)
                    return NotFound("No se encontró la empresa");

                return Ok(result);

            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet("get-data-dashboard")]
        [SwaggerOperation(
            Summary = "Datos estadísticos para tableros",
            Description = "Permite obtener los datos que se van a visualizar en los tableros."
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetDataDashboardQueryResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDataDashboard()
        {
            try
            {
                UserDataAccess userData = new(_serviceScopeFactory);
                var user = await userData.GetUserSession();
                var result = await Mediator.Send(new GetDataDashboardQuery() { CompanyId = user.CompanyId });

                return Ok(result);

            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("register-employee")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RegisterResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RegisterResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(RegisterResponse))]
        [SwaggerOperation(
           Summary = "Registro de empleados",
           Description = "Registra un empleado a tu empresa"
        )]
        public async Task<IActionResult> RegisterEmployee([FromBody] RegisterEmployeeCommand command)
        {
            try
            {
                UserDataAccess userData = new(_serviceScopeFactory);
                var user = await userData.GetUserSession();
                command.CompanyId = user.CompanyId;
                var response = await Mediator.Send(command);

                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                if (response.HasError)
                {

                    if (response.Error.Contains("Error") && !response.Error.Contains("password"))
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, response.Error);
                    }
                    return BadRequest(response.Error);
                }

                return Ok(response.IsSuccess);
            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("delete-employee/{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [SwaggerOperation(
            Summary = "Permite eliminar un empleado.",
            Description = "Maneja el apartado de eliminación, debe de especificar los parametros correspondientes."
        )]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            try
            {
                UserDataAccess userData = new(_serviceScopeFactory);
                var user = await userData.GetUserSession();
                var result = await Mediator.Send(new DeleteEmployeeCommand() { Id = id, CompanyId = user.CompanyId });

                if (result == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError);

                return NoContent();

            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        [Authorize]
        [HttpPut("edit-profile")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RegisterResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(RegisterResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
           Summary = "Editar el perfil",
           Description = "Cambie los datos de su perfil"
        )]
        public async Task<IActionResult> EditProfile([FromForm] EditProfileCommandRequest request)
        {
            try
            {
                UserDataAccess userData = new(_serviceScopeFactory);
                var user = await userData.GetUserSession();

                var command = _mapper.Map<EditProfileCommand>(request);
                command.Id = user.Id;

                var response = await Mediator.Send(command);

                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                if (response.HasError)
                {

                    if (response.Error.Contains("Usuario"))
                    {
                        return NotFound(response);
                    }
                    return StatusCode(StatusCodes.Status500InternalServerError, response.Error);
                }

                return Ok(response.IsSuccess);
            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPut("edit-profile-company")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RegisterResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
           Summary = "Editar el perfil de la empresa",
           Description = "Cambie los datos del perfil de la empresa"
        )]
        public async Task<IActionResult> EditProfileCompany([FromForm] EditProfileCompanyCommandRequest request)
        {
            try
            {
                UserDataAccess userData = new(_serviceScopeFactory);
                var user = await userData.GetUserSession();

                var command = _mapper.Map<EditProfileCompanyCommand>(request);
                command.Id = user.CompanyId;

                var response = await Mediator.Send(command);

                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                if (response == null)
                {
                    return NotFound("Empresa no encontrada");
                }

                return Ok(response.IsSuccess);
            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
