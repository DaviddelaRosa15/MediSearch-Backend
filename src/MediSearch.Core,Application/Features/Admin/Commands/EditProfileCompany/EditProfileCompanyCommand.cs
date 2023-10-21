using AutoMapper;
using MediatR;
using MediSearch.Core.Application.Dtos.Account;
using MediSearch.Core.Application.Helpers;
using MediSearch.Core.Application.Interfaces.Repositories;
using MediSearch.Core.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace MediSearch.Core.Application.Features.Admin.Commands.EditProfileCompany
{
    public class EditProfileCompanyCommand : IRequest<RegisterResponse>
    {
        public string Id { get; set; }
        public string Ceo { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public IFormFile? Logo { get; set; }
        public string Province { get; set; }
        public string Municipality { get; set; }
        public string Address { get; set; }
        public string? WebSite { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? Twitter { get; set; }
    }

    public class EditProfileCompanyCommandHandler : IRequestHandler<EditProfileCompanyCommand, RegisterResponse>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public EditProfileCompanyCommandHandler(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }


        public async Task<RegisterResponse> Handle(EditProfileCompanyCommand command, CancellationToken cancellationToken)
        {
            try
            {
                RegisterResponse response = new();
                var company = await _companyRepository.GetByIdAsync(command.Id);
                if (company == null)
                {
                    return null;
                }

                company.Ceo = command.Ceo;
                company.Name = command.Name;
                company.Email = command.Email;
                company.Phone = command.Phone;
                company.UrlImage = ImageUpload.UploadImageCompany(command.Logo, true, company.UrlImage);
                company.Province = command.Province;
                company.Municipality = command.Municipality;
                company.Address = command.Address;
                company.WebSite = command.WebSite;
                company.Facebook = command.Facebook;
                company.Instagram = command.Instagram;
                company.Twitter = command.Twitter;

                await _companyRepository.UpdateAsync(company, company.Id);
                
                response.IsSuccess = true;
                return response;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un error tratando de actualizar la empresa.");
            }
        }

    }
}
