using MediatR;
using MediSearch.Core.Application.Interfaces.Repositories;

namespace MediSearch.Core.Application.Features.Admin.Queries.GetProfileCompany
{
    public class GetProfileCompanyQuery : IRequest<GetProfileCompanyQueryResponse>
    {
        public string Id { get; set; }
    }

    public class GetProfileCompanyQueryHandler : IRequestHandler<GetProfileCompanyQuery, GetProfileCompanyQueryResponse>
    {
        private readonly ICompanyRepository _companyRepository;
        public GetProfileCompanyQueryHandler(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<GetProfileCompanyQueryResponse> Handle(GetProfileCompanyQuery request, CancellationToken cancellationToken)
        {
            var company = await _companyRepository.GetByIdAsync(request.Id);
            if (company == null)
            {
                return null;
            }

            GetProfileCompanyQueryResponse response = new()
            {
                Id = company.Id,
                Ceo = company.Ceo,
                Name = company.Name,
                Phone = company.Phone,
                UrlImage = company.UrlImage,
                Email = company.Email,
                Address = company.Address,
                Municipality = company.Municipality,
                Province = company.Province,
                Facebook = company.Facebook,
                Instagram = company.Instagram,
                Twitter = company.Twitter,
                WebSite = company.WebSite
            };

            return response;
        }
    }
}
