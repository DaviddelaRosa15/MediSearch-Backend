using AutoMapper;
using MediatR;
using MediSearch.Core.Application.Interfaces.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace MediSearch.Core.Application.Features.Home.Queries.GetCompanyByName
{
    public class GetCompanyByNameQuery : IRequest<List<GetCompanyByNameQueryResponse>>
    {
        [SwaggerParameter(Description = "Nombre")]
        [Required(ErrorMessage = "Debe de ingresar datos para hacer la búsqueda.")]
        public string Name { get; set; }
    }

    public class GetCompanyByNameQueryHandler : IRequestHandler<GetCompanyByNameQuery, List<GetCompanyByNameQueryResponse>>
    {
        private readonly ICompanyRepository _companyRepository;

        public GetCompanyByNameQueryHandler(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<List<GetCompanyByNameQueryResponse>> Handle(GetCompanyByNameQuery query, CancellationToken cancellationToken)
        {
            var companies = await _companyRepository.GetAllWithIncludeAsync(new List<string>() { "CompanyType"});
            List<GetCompanyByNameQueryResponse> result = companies.Where(c => c.Name.ToLower().Contains(query.Name.ToLower())).Select(c => new GetCompanyByNameQueryResponse()
            {
                Id = c.Id,
                Name = c.Name,
                UrlImage = c.UrlImage,
                Type = c.CompanyType.Name 
            }).ToList();

            return result;
        }
    }
}
