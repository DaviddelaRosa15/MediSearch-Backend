using MediatR;
using MediSearch.Core.Application.Interfaces.Services;

namespace MediSearch.Core.Application.Features.Admin.Queries.GetProfile
{
    public class GetProfileQuery : IRequest<GetProfileQueryResponse>
    {
        public string Id { get; set; }
    }

    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, GetProfileQueryResponse>
    {
        private readonly IAccountService _accountService;
        public GetProfileQueryHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<GetProfileQueryResponse> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _accountService.GetUsersById(request.Id);
            if(user == null)
            {
                return null;
            }

            GetProfileQueryResponse response = new()
            {
                Address = user.Address,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Municipality = user.Municipality,
                Province = user.Province,
                UrlImage = user.UrlImage
            };

            return response;
        }
    }
}
