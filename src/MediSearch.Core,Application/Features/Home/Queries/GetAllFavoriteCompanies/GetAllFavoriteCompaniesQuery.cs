using AutoMapper;
using MediatR;
using MediSearch.Core.Application.Dtos.Company;
using MediSearch.Core.Application.Features.Home.Queries.GetAllFavoriteCompanies;
using MediSearch.Core.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Features.Home.Queries.GetAllFavoriteCompanies
{
    public class GetAllFavoriteCompaniesQuery : IRequest<List<GetAllFavoriteCompaniesQueryResponse>>
    {
        public string UserId { get; set; }
    }

    public class GetAllFavoriteCompaniesQueryHandler : IRequestHandler<GetAllFavoriteCompaniesQuery, List<GetAllFavoriteCompaniesQueryResponse>>
    {
        private readonly IFavoriteCompanyRepository _favoriteCompanyRepository;

        public GetAllFavoriteCompaniesQueryHandler(IFavoriteCompanyRepository favoriteCompanyRepository)
        {
            _favoriteCompanyRepository = favoriteCompanyRepository;
        }

        public async Task<List<GetAllFavoriteCompaniesQueryResponse>> Handle(GetAllFavoriteCompaniesQuery query, CancellationToken cancellationToken)
        {
            var companies = await _favoriteCompanyRepository.GetAllWithIncludeAsync(new List<string>() { "Company"});
            var favorites = companies.FindAll(c => c.UserId == query.UserId);
            List<GetAllFavoriteCompaniesQueryResponse> response = new();

            foreach (var item in favorites)
            {
                GetAllFavoriteCompaniesQueryResponse favorite = new()
                {
                    Id = item.Id,
                    CompanyId = item.CompanyId,
                    Name = item.Company.Name,
                    UrlImage = item.Company.UrlImage,
                    Phone = item.Company.Phone,
                    Province = item.Company.Province,
                    Municipality = item.Company.Municipality,
                    Address = item.Company.Address,
                    IsFavorite = true
                };

                response.Add(favorite);
            }

            return response;
        }

    }
}
