using AutoMapper;
using MediatR;
using MediSearch.Core.Application.Features.Home.Queries.GetAllCompanies;
using MediSearch.Core.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Features.Home.Queries.GetAllCompanies
{
    public class GetAllCompaniesQuery : IRequest<List<GetAllCompaniesQueryResponse>>
    {

    }

    public class GetAllCompaniesQueryHandler : IRequestHandler<GetAllCompaniesQuery, List<GetAllCompaniesQueryResponse>>
    {
        private readonly ICompanyRepository _companyRepository;

        public GetAllCompaniesQueryHandler(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<List<GetAllCompaniesQueryResponse>> Handle(GetAllCompaniesQuery request, CancellationToken cancellationToken)
        {
            var companies = await _companyRepository.GetAllWithIncludeAsync(new List<string>() { "CompanyType" });
            List<GetAllCompaniesQueryResponse> result = companies.Select(c => new GetAllCompaniesQueryResponse()
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
