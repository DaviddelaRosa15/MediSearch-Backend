using MediSearch.Core.Application.Dtos.Company;
using MediSearch.Core.Application.Dtos.Product;
using MediSearch.Core.Application.Enums;
using MediSearch.Core.Application.Features.Home.Command.AddFavoriteCompany;
using MediSearch.Core.Application.Features.Home.Command.AddFavoriteProduct;
using MediSearch.Core.Application.Features.Home.Command.DeleteFavoriteCompany;
using MediSearch.Core.Application.Features.Home.Command.DeleteFavoriteProduct;
using MediSearch.Core.Application.Features.Home.Queries.GetAllCompanies;
using MediSearch.Core.Application.Features.Home.Queries.GetAllFarmacy;
using MediSearch.Core.Application.Features.Home.Queries.GetAllFavoriteCompanies;
using MediSearch.Core.Application.Features.Home.Queries.GetAllFavoriteProducts;
using MediSearch.Core.Application.Features.Home.Queries.GetAllLaboratory;
using MediSearch.Core.Application.Features.Home.Queries.GetCompanyByName;
using MediSearch.Core.Application.Features.Home.Queries.GetCompanyDetails;
using MediSearch.Core.Application.Features.Home.Queries.GetDataClient;
using MediSearch.Core.Application.Features.Home.Queries.GetLastData;
using MediSearch.Core.Application.Features.Home.Queries.GetProduct;
using MediSearch.Core.Application.Features.Home.Queries.GetProductsFarmacy;
using MediSearch.Core.Application.Features.Home.Queries.GetProductsLaboratory;
using MediSearch.Core.Application.Features.Product.Command.DeleteProduct;
using MediSearch.Core.Application.Features.Product.CreateProduct;
using MediSearch.WebApi.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;

namespace MediSearch.WebApi.Controllers.v1
{
    [SwaggerTag("Inicio")]
    public class HomeController : BaseApiController
    {

        private readonly IServiceScopeFactory _serviceScopeFactory;
        public HomeController(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        [HttpGet("get-products-farmacy")]
        [SwaggerOperation(
           Summary = "Obtener todos los productos de las farmacias.",
            Description = "Nos permite obtener todos los productos que las farmacias han registrado en el sistema."
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProductHomeDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProductResponse))]
        public async Task<IActionResult> GetProductsFarmacy()
        {

            try
            {
                List<ProductHomeDTO> result = new();
                UserDataAccess userData = new(_serviceScopeFactory);
                var user = await userData.GetUserSession();
                if(user == null)
                {
                    result = await Mediator.Send(new GetProductsFarmacyQuery());
                }
                else
                {
                    result = await Mediator.Send(new GetProductsFarmacyQuery() { UserId = user.CompanyId != "Client" ? user.CompanyId : user.Id });
                }

                if (result == null || result.Count == 0)
                    return NotFound();

                return Ok(result);

            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        [HttpGet("get-products-laboratory")]
        [SwaggerOperation(
           Summary = "Obtener todos los productos de los laboratorios.",
            Description = "Nos permite obtener todos los productos que los laboratoios han registrado en el sistema."
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProductHomeDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProductResponse))]
        public async Task<IActionResult> GetProductsLaboratory()
        {

            try
            {
                List<ProductHomeDTO> result = new();
                UserDataAccess userData = new(_serviceScopeFactory);
                var user = await userData.GetUserSession();
                if (user == null)
                {
                    result = await Mediator.Send(new GetProductsLaboratoryQuery());
                }
                else
                {
                    result = await Mediator.Send(new GetProductsLaboratoryQuery() { UserId = user.CompanyId != "Client" ? user.CompanyId : user.Id });
                }

                if (result == null || result.Count == 0)
                    return NotFound();

                return Ok(result);

            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        [HttpGet("get-product/{id}")]
        [SwaggerOperation(
           Summary = "Obtener las informaciones de un producto.",
            Description = "Nos permite obtener todas las informaciones de un producto y de la empresa que lo registró."
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProductQueryResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProduct(string id)
        {
            try
            {
                GetProductQueryResponse result = new();
                UserDataAccess userData = new(_serviceScopeFactory);
                var user = await userData.GetUserSession();
                if (user == null)
                {
                    result = await Mediator.Send(new GetProductQuery() { Id = id });
                }
                else
                {
                    result = await Mediator.Send(new GetProductQuery() {Id = id, UserId = user.CompanyId != "Client" ? user.CompanyId : user.Id });
                }

                if (result == null)
                    return NotFound();

                return Ok(result);

            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        [HttpGet("get-all-farmacy")]
        [SwaggerOperation(
           Summary = "Obtener todas las farmacias.",
            Description = "Nos permite obtener todas las farmacias en el sistema."
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CompanyDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProductResponse))]
        public async Task<IActionResult> GetAllFarmacy()
        {

            try
            {
                List<CompanyDTO> result = new();
                UserDataAccess userData = new(_serviceScopeFactory);
                var user = await userData.GetUserSession();
                if (user != null)
                {
                    result = await Mediator.Send(new GetAllFarmacyQuery() { UserId = user.CompanyId != "Client" ? user.CompanyId : user.Id});
                }
                else
                {
                    result = await Mediator.Send(new GetAllFarmacyQuery());
                }


                if (result == null || result.Count == 0)
                    return NotFound();

                return Ok(result);

            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        [HttpGet("get-all-laboratory")]
        [SwaggerOperation(
           Summary = "Obtener todas los laboratorios.",
            Description = "Nos permite obtener todas los laboratorios en el sistema."
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CompanyDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProductResponse))]
        public async Task<IActionResult> GetAllLaboratory()
        {

            try
            {
                List<CompanyDTO> result = new();
                UserDataAccess userData = new(_serviceScopeFactory);
                var user = await userData.GetUserSession();
                if (user != null)
                {
                    result = await Mediator.Send(new GetAllLaboratoryQuery() { UserId = user.CompanyId != "Client" ? user.CompanyId : user.Id });
                }
                else
                {
                    result = await Mediator.Send(new GetAllLaboratoryQuery());
                }

                if (result == null || result.Count == 0)
                    return NotFound();

                return Ok(result);

            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        [HttpGet("get-all-companies")]
        [SwaggerOperation(
           Summary = "Obtener todas las empresas.",
            Description = "Nos permite obtener todos las empresas en el sistema."
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllCompaniesQueryResponse>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCompanies()
        {
            try
            {

                var result = await Mediator.Send(new GetAllCompaniesQuery());

                if (result == null || result.Count == 0)
                    return NotFound();

                return Ok(result);

            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        [HttpGet("get-company/{id}")]
        [SwaggerOperation(
           Summary = "Obtener las informaciones de una empresa.",
            Description = "Nos permite obtener todos los campos de una empresa."
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyDetailsDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCompanyDetails(string id)
        {
            try
            {
                CompanyDetailsDTO result = new();
                UserDataAccess userData = new(_serviceScopeFactory);
                var user = await userData.GetUserSession();
                if (user != null)
                {
                    result = await Mediator.Send(new GetCompanyDetailsQuery() { Id = id, UserId = user.CompanyId != "Client" ? user.CompanyId : user.Id });
                }
                else
                {
                    result = await Mediator.Send(new GetCompanyDetailsQuery() { Id = id});
                }

                if (result == null)
                    return NotFound();

                return Ok(result);

            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        [HttpGet("get-company-name")]
        [SwaggerOperation(
           Summary = "Obtener empresas por nombre.",
            Description = "Nos permite obtener todas las empresas que coincidan con la busqueda por nombre."
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetCompanyByNameQueryResponse>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCompanyByName(GetCompanyByNameQuery query)
        {
            try
            {

                var result = await Mediator.Send(query);

                if (result == null || result.Count == 0)
                    return NotFound();

                return Ok(result);

            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        [Authorize]
        [HttpGet("get-all-favorite-company")]
        [SwaggerOperation(
           Summary = "Obtener empresas favoritas.",
            Description = "Nos permite obtener todas las empresas que tenemos en la sección de favoritos."
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllFavoriteCompaniesQueryResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllFavoriteCompanies()
        {
            try
            {
                UserDataAccess userData = new(_serviceScopeFactory);
                var user = await userData.GetUserSession();
                var result = await Mediator.Send(new GetAllFavoriteCompaniesQuery() { UserId = user.CompanyId != "Client" ? user.CompanyId : user.Id });

                return Ok(result);
            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        [Authorize]
        [HttpGet("get-all-favorite-product")]
        [SwaggerOperation(
           Summary = "Obtener productos favoritos.",
            Description = "Nos permite obtener todos los productos que tenemos en la sección de favoritos."
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllFavoriteProductsQueryResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllFavoriteProducts()
        {
            try
            {
                UserDataAccess userData = new(_serviceScopeFactory);
                var user = await userData.GetUserSession();
                var result = await Mediator.Send(new GetAllFavoriteProductsQuery() { UserId = user.CompanyId != "Client" ? user.CompanyId : user.Id });

                return Ok(result);
            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        [Authorize(Roles = "Client")]
        [HttpGet("get-data-client")]
        [SwaggerOperation(
           Summary = "Obtener datos relacionados al cliente logueado.",
            Description = "Nos permite obtener datos para que el cliente esté al tanto de la plataforma."
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetDataClientQueryResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDataClient()
        {
            try
            {
                UserDataAccess userData = new(_serviceScopeFactory);
                var user = await userData.GetUserSession();
                var result = await Mediator.Send(new GetDataClientQuery() { User = user });

                return Ok(result);
            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        [HttpGet("get-last-data")]
        [SwaggerOperation(
           Summary = "Obtener ultimas actualizaciones de la plataforma.",
            Description = "Nos permite obtener los últimos datos registrados en la plataforma."
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetLastDataQueryResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetLastData()
        {
            try
            {
                var result = await Mediator.Send(new GetLastDataQuery() { });

                return Ok(result);
            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        [Authorize]
        [HttpPost("add-favorite-company")]
        [SwaggerOperation(
           Summary = "Añade una empresa a tus favoritos.",
            Description = "Permite colocar a una empresa en la sección de favoritos."
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProductResponse))]
        public async Task<IActionResult> AddFavoriteCompany([FromBody] AddFavoriteCompanyCommand command)
        {

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                UserDataAccess userData = new(_serviceScopeFactory);
                var user = await userData.GetUserSession();
                command.UserId = user.CompanyId != "Client" ? user.CompanyId : user.Id;
                var result = await Mediator.Send(command);

                return Ok(result.IsSuccess);

            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        [Authorize]
        [HttpPost("add-favorite-product")]
        [SwaggerOperation(
           Summary = "Añade un producto a tus favoritos.",
            Description = "Permite colocar a un producto en la sección de favoritos."
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProductResponse))]
        public async Task<IActionResult> AddFavoriteProduct([FromBody] AddFavoriteProductCommand command)
        {

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                UserDataAccess userData = new(_serviceScopeFactory);
                var user = await userData.GetUserSession();
                command.UserId = user.CompanyId != "Client" ? user.CompanyId : user.Id;
                var result = await Mediator.Send(command);

                return Ok(result.IsSuccess);

            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        [Authorize]
        [HttpDelete("delete-favorite-company/{companyId}")]
        [SwaggerOperation(
            Summary = "Permite eliminar una empresa de favoritos.",
            Description = "Maneja el apartado de eliminación de favoritos, debe de especificar los parametros correspondientes."
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DeleteFavoriteCompanyResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteFavoriteCompany(string companyId)
        {
            try
            {
                UserDataAccess userData = new(_serviceScopeFactory);
                var user = await userData.GetUserSession();
                var result = await Mediator.Send(new DeleteFavoriteCompanyCommand() { CompanyId = companyId, UserId = user.CompanyId != "Client" ? user.CompanyId : user.Id });

                return Ok(result);
            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        [Authorize]
        [HttpDelete("delete-favorite-product/{productId}")]
        [SwaggerOperation(
            Summary = "Permite eliminar un producto de favoritos.",
            Description = "Maneja el apartado de eliminación de favoritos, debe de especificar los parametros correspondientes."
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DeleteFavoriteProductResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteFavoriteProduct(string productId)
        {
            try
            {
                UserDataAccess userData = new(_serviceScopeFactory);
                var user = await userData.GetUserSession();
                var result = await Mediator.Send(new DeleteFavoriteProductCommand() { ProductId = productId, UserId = user.CompanyId != "Client" ? user.CompanyId : user.Id });

                return Ok(result);
            }
            catch (Exception e)
            {
                return new JsonResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }
    }
}
