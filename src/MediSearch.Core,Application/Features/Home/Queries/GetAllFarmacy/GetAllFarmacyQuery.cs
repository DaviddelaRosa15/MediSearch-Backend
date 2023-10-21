using AutoMapper;
using MediatR;
using MediSearch.Core.Application.Dtos.Company;
using MediSearch.Core.Application.Interfaces.Repositories;

namespace MediSearch.Core.Application.Features.Home.Queries.GetAllFarmacy
{
    public class GetAllFarmacyQuery : IRequest<List<CompanyDTO>>
    {
        public string? UserId { get; set; }
    }

    public class GetAllFarmacyQueryHandler : IRequestHandler<GetAllFarmacyQuery, List<CompanyDTO>>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly ICompanyTypeRepository _companyTypeRepository;
        private readonly IFavoriteCompanyRepository _favoriteCompanyTypeRepository;
        private readonly IMapper _mapper;

        public GetAllFarmacyQueryHandler(ICompanyRepository companyRepository, ICompanyTypeRepository companyTypeRepository, IMapper mapper, IFavoriteCompanyRepository favoriteCompanyTypeRepository)
        {
            _companyRepository = companyRepository;
            _companyTypeRepository = companyTypeRepository;
            _mapper = mapper;
            _favoriteCompanyTypeRepository = favoriteCompanyTypeRepository;
        }

        public async Task<List<CompanyDTO>> Handle(GetAllFarmacyQuery query, CancellationToken cancellationToken)
        {
            List<CompanyDTO> farmacies = await GetFarmacies(query.UserId);

            return farmacies;
        }

        private async Task<List<CompanyDTO>> GetFarmacies(string? user)
        {
            List<CompanyDTO> result = new();
            var type = await _companyTypeRepository.GetByNameAsync("Farmacia");
            var companies = await _companyRepository.GetAllAsync();
            var farmacies = companies.FindAll(x => x.CompanyTypeId == type.Id);
            if (user == null)
            {
                result = _mapper.Map<List<CompanyDTO>>(farmacies);
            }
            else
            {
foreach (var farm in farmacies)
            {
                var favorite = await _favoriteCompanyTypeRepository.ValidateFavorite(farm.Id, user);

                CompanyDTO dto = new()
                {
                    Id = farm.Id,
                    Name = farm.Name,
                    UrlImage = farm.UrlImage,
                    Phone = farm.Phone,
                    Province = farm.Province,
                    Municipality = farm.Municipality,
                    Address = farm.Address,
                    IsFavorite = favorite != null
                };

                result.Add(dto);
            }
            }
            
            return result;
        }
    }
}
