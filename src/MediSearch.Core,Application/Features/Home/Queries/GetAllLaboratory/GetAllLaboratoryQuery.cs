using AutoMapper;
using MediatR;
using MediSearch.Core.Application.Dtos.Company;
using MediSearch.Core.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Features.Home.Queries.GetAllLaboratory
{
    public class GetAllLaboratoryQuery : IRequest<List<CompanyDTO>>
    {
        public string? UserId { get; set; }
    }

    public class GetAllLaboratoryQueryHandler : IRequestHandler<GetAllLaboratoryQuery, List<CompanyDTO>>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly ICompanyTypeRepository _companyTypeRepository;
        private readonly IFavoriteCompanyRepository _favoriteCompanyTypeRepository;
        private readonly IMapper _mapper;

        public GetAllLaboratoryQueryHandler(ICompanyRepository companyRepository, ICompanyTypeRepository companyTypeRepository, IMapper mapper, IFavoriteCompanyRepository favoriteCompanyTypeRepository)
        {
            _companyRepository = companyRepository;
            _companyTypeRepository = companyTypeRepository;
            _mapper = mapper;
            _favoriteCompanyTypeRepository = favoriteCompanyTypeRepository;
        }

        public async Task<List<CompanyDTO>> Handle(GetAllLaboratoryQuery query, CancellationToken cancellationToken)
        {
            List<CompanyDTO> laboratories = await GetLaboratories(query.UserId);

            return laboratories;
        }

        private async Task<List<CompanyDTO>> GetLaboratories(string? user)
        {
            List<CompanyDTO> result = new();
            var type = await _companyTypeRepository.GetByNameAsync("Laboratorio");
            var companies = await _companyRepository.GetAllAsync();
            var laboratories = companies.FindAll(x => x.CompanyTypeId == type.Id);
            if (user == null)
            {
                result = _mapper.Map<List<CompanyDTO>>(laboratories);
            }
            else
            {
                foreach (var lab in laboratories)
                {
                    var favorite = await _favoriteCompanyTypeRepository.ValidateFavorite(lab.Id, user);

                    CompanyDTO dto = new()
                    {
                        Id = lab.Id,
                        Name = lab.Name,
                        UrlImage = lab.UrlImage,
                        Phone = lab.Phone,
                        Province = lab.Province,
                        Municipality = lab.Municipality,
                        Address = lab.Address,
                        IsFavorite = favorite != null
                    };

                    result.Add(dto);
                }
            }
            
            return result;
        }
    }
}
