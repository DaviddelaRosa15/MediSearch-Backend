using MediSearch.Core.Application.Dtos.Message;
using MediSearch.Core.Application.Dtos.Product;
using MediSearch.Core.Application.Features.Chat.Command.SendMessage;
using MediSearch.Core.Application.Features.Chat.Queries.GetChat;
using MediSearch.Core.Application.Features.Chat.Queries.GetChats;
using MediSearch.Core.Domain.Entities;
using MediSearch.WebApi.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MediSearch.WebApi.Controllers.v1
{
    [Authorize]
    [SwaggerTag("Mensajería")]
    public class ChatController : BaseApiController
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public ChatController(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        [HttpGet("get-all-chats")]
        [SwaggerOperation(
            Summary = "Todos los chats que tiene el usuario.",
            Description = "Permite obtener todos los chats que tiene el usuario con otros usuarios."
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetChatsQueryResponse>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllChats()
        {
            try
            {
                List<GetChatsQueryResponse> result = new();
                UserDataAccess userData = new(_serviceScopeFactory);
                var user = await userData.GetUserSession();

                if (user.CompanyId == "Client")
                {
                    result = await Mediator.Send(new GetChatsQuery() { IdUser = user.Id });
                }
                else
                {
                    result = await Mediator.Send(new GetChatsQuery() { IdUser = user.CompanyId });
                }

                if (result == null || result.Count == 0)
                    return NotFound("Este usuario no ha iniciado chat");

                return Ok(result);

            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        [HttpGet("get-chat/{id}")]
        [SwaggerOperation(
            Summary = "Mensajes que tiene el usuario en ese chat(sala).",
            Description = "Permite obtener todos los mensajes que tiene el usuario con otro usuario en ese chat(sala)."
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetChatQueryResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetChat(string id)
        {
            try
            {
                GetChatQueryResponse result = new();
                UserDataAccess userData = new(_serviceScopeFactory);
                var user = await userData.GetUserSession();

                if (user.CompanyId == "Client")
                {
                    result = await Mediator.Send(new GetChatQuery() { IdHall = id, IdUser = user.Id });
                }
                else
                {
                    result = await Mediator.Send(new GetChatQuery() { IdHall = id, IdUser = user.CompanyId });
                }

                if (result == null)
                    return NotFound("Este usuario no tiene mensajes en esta sala");

                return Ok(result);

            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        [HttpPost("send-message")]
        [SwaggerOperation(
            Summary = "Enviar mensajes.",
            Description = "Permite enviar mensajes a otros usuarios."
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendMessage([FromForm] SendMessageCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                MessageDTO result = new();
                UserDataAccess userData = new(_serviceScopeFactory);
                var user = await userData.GetUserSession();

                if (user.CompanyId == "Client")
                {
                    command.IdUser = user.Id;
                    result = await Mediator.Send(command);
                }
                else
                {
                    command.IdUser = user.CompanyId;
                    result = await Mediator.Send(command);
                }

                return Ok(result);

            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }
    }
}
