using MediatR;
using MediSearch.Core.Application.Dtos.Account;
using MediSearch.Core.Application.Features.Account.Commands.Authenticate;
using MediSearch.Core.Application.Features.Account.Commands.ConfirmCode;
using MediSearch.Core.Application.Features.Account.Commands.ConfirmEmail;
using MediSearch.Core.Application.Features.Account.Commands.RegisterClient;
using MediSearch.Core.Application.Features.Account.Commands.RegisterCompany;
using MediSearch.Core.Application.Features.Account.Commands.ResetPassword;
using MediSearch.Core.Application.Features.Account.Commands.ChangePassword;
using MediSearch.Core.Application.Features.Account.Queries.GetRefreshAccessToken;
using MediSearch.Core.Application.Helpers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;
using MediSearch.Core.Application.Features.Account.Commands.ValidateRefreshToken;

namespace MediSearch.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[Controller]")]
    [SwaggerTag("Sistema de membresia")]
    public class AccountController : ControllerBase
    {
        private IMediator _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        private readonly IHostEnvironment env;

        public AccountController(IHostEnvironment hostEnvironment)
        {
            env = hostEnvironment;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticationResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthenticationResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(AuthenticationResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(AuthenticationResponse))]
        [SwaggerOperation(
           Summary = "Iniciar sesión",
           Description = "Autentica al usuario y devuelve un JWT Token"
        )]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateCommand command)
        {
            try
            {
                var response = await Mediator.Send(command);

                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                if (response.HasError)
                {
                    if (response.Error.Contains("No existe una cuenta registrada con este usuario"))
                    {
                        return NotFound(response.Error);
                    }
                    if (response.Error.Contains("correo"))
                    {
                        return StatusCode(StatusCodes.Status401Unauthorized, response.Error);
                    }
                    return BadRequest(response.Error);
                }

                Response.Cookies.Append("refreshToken", response.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddDays(5),
                    SameSite = SameSiteMode.None
                });

                return Ok(response);
            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        [HttpGet("refresh-access-token")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticationResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(AuthenticationResponse))]
        [SwaggerOperation(
           Summary = "Obtener nuevo access token",
           Description = "Valida el refresh token y devuelve un JWT Token de acceso nuevo"
        )]
        public async Task<IActionResult> RefreshAccesToken()
        {
            try
            {
                var response = await Mediator.Send(new GetRefreshAccessTokenQuery());

                if (response.HasError)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                return Ok(response);
            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [HttpGet("validate-refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ValidateRefreshTokenResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(AuthenticationResponse))]
        [SwaggerOperation(
           Summary = "Validar el refresh token",
           Description = "Verifica si existe refresh token y lo valida"
        )]
        public async Task<IActionResult> ValidateRefreshToken()
        {
            try
            {
                var token = Request.Cookies["refreshToken"];
                if (token == null)
                {
                    return Ok(new ValidateRefreshTokenResponse() { ValidRefreshToken = false });
                }
                var response = await Mediator.Send(new ValidateRefreshTokenCommand() { refreshToken = token});

                return Ok(response);
            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [HttpPost("register-client")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RegisterResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RegisterResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(RegisterResponse))]
        [SwaggerOperation(
           Summary = "Registro de usuario cliente",
           Description = "Registra a un usuario de tipo cliente"
        )]
        public async Task<IActionResult> RegisterClient([FromForm] RegisterClientCommand command)
        {
            try
            {
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

        [HttpPost("register-company")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RegisterResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RegisterResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(RegisterResponse))]
        [SwaggerOperation(
            Summary = "Registro de empresa",
            Description = "Registra a un usuario de tipo administrador junto con su empresa"
            )]
        public async Task<IActionResult> RegisterCompany([FromForm] RegisterCompanyCommand command)
        {
            try
            {
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

        [HttpGet("confirm-email")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ConfirmEmailResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ConfirmEmailResponse))]
        [SwaggerOperation(
           Summary = "Comfirmar al usuario ",
           Description = "Confirma la cuenta del usuario"
        )]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            try
            {
                ConfirmEmailCommand command = new()
                {
                    UserId = userId,
                    Token = token
                };
                var response = await Mediator.Send(command);

                if (response.HasError)
                {
                    if (response.Error.Contains("error"))
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, response.Error);
                    }
                    return NotFound(response.Error);
                }

                return RedirectToAction("thanks", new { name = response.NameUser });
            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [HttpGet("thanks")]
        public IActionResult Thanks(string name)
        {
            string htmlBody = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Bienvenido/a al Sistema</title>
    <style>
        /* Estilos adicionales */
        body {
            font-family: Arial, sans-serif;
        }
        .container {
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f8f8f8;
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
            <h1 class='title'>¡Bienvenido/a al Sistema!</h1>
        </div>
        <div class='message'>
            <p>Hola [Nombre],</p>
            <p>Gracias por confirmar tu cuenta. Ahora tienes acceso completo a todas las funcionalidades del sistema.</p>
            <p>Disfruta de todas las características y no dudes en ponerte en contacto con nosotros si tienes alguna pregunta o necesitas asistencia.</p>
            <p><a href='http://localhost:5173/' class='button'>Acceder al Sistema</a></p>
        </div>
        <div class='footer'>
            <p>Atentamente,</p>
            <p>El equipo de MediSearch</p>
        </div>
    </div>
</body>
</html>
";

            string html = htmlBody.Replace("[Nombre]", name);
            return Content(html, "text/html");
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResetPasswordResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResetPasswordResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResetPasswordResponse))]
        [SwaggerOperation(
            Summary = "Restablecer contraseña",
            Description = "Permite que el usuario cambie su contraseña si se le olvidó"
            )]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            try
            {
                var response = await Mediator.Send(command);

                if (response.HasError)
                {
                    if (response.Error.Contains("error"))
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, response.Error);
                    }
                    return NotFound(response.Error);
                }

                return Ok(response.IsSuccess);
            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [HttpPost("confirm-code")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ConfirmCodeResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ConfirmCodeResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ConfirmCodeResponse))]
        [SwaggerOperation(
            Summary = "Confirmar código",
            Description = "Permite que el usuario ingrese el código de confirmación que se le envió por correo"
            )]
        public async Task<IActionResult> ConfirmCode([FromBody] ConfirmCodeCommand command)
        {
            try
            {
                var response = await Mediator.Send(command);

                if (response.HasError)
                {
                    if (response.Error.Contains("error"))
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, response.Error);
                    }
                    return NotFound(response.Error);
                }

                return Ok(response.IsSuccess);
            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [HttpPost("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResetPasswordResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResetPasswordResponse))]
        [SwaggerOperation(
            Summary = "Restablecer contraseña",
            Description = "Permite que el usuario cambie su contraseña si se le olvidó"
            )]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            try
            {
                var response = await Mediator.Send(command);

                if (response.HasError)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, response.Error);
                }

                HttpContext.Session.Remove("user");
                HttpContext.Session.Remove("confirmCode");

                return Ok(response.IsSuccess);
            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [HttpGet("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerOperation(
           Summary = "Salir de sesión",
           Description = "Borra el refresh token"
        )]
        public async Task<IActionResult> Logout()
        {
            try
            {
                Response.Cookies.Delete("refreshToken", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddDays(5),
                    SameSite = SameSiteMode.None
                });

                return NoContent();
            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}