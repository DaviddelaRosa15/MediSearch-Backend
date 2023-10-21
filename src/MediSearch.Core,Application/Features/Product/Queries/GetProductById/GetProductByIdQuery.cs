using AutoMapper;
using MediatR;
using MediSearch.Core.Application.Dtos.Comment;
using MediSearch.Core.Application.Dtos.Product;
using MediSearch.Core.Application.Dtos.Reply;
using MediSearch.Core.Application.Interfaces.Repositories;
using MediSearch.Core.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Features.Product.Queries.GetProductById
{
    public class GetProductByIdQuery : IRequest<ProductDTO>
    {
        public string Id { get; set; }
    }

    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDTO>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;
        private readonly ICompanyRepository _companyRepository;
        private readonly ICompanyUserRepository _companyUserRepository;
        private readonly ICommentRepository _commentRepository;

        public GetProductByIdQueryHandler(IProductRepository productRepository, IMapper mapper, IAccountService accountService, ICompanyRepository companyRepository, ICompanyUserRepository companyUserRepository, ICommentRepository commentRepository)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _accountService = accountService;
            _companyRepository = companyRepository;
            _companyUserRepository = companyUserRepository;
            _commentRepository = commentRepository;
        }

        public async Task<ProductDTO> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            List<CommentDTO> resultDTO = new();
            var product = await _productRepository.GetByIdAsync(request.Id);
            var response = _mapper.Map<ProductDTO>(product);
            
            if (response != null)
            {
                var comments = await _commentRepository.GetCommentsByProduct(response.Id);

                if (comments != null && comments.Count != 0)
                {
                    foreach (var item in comments)
                    {
                        List<ReplyDTO> list = new();
                        string name = "";
                        var user = await _accountService.GetUsersById(item.UserId);
                        var result = await _companyUserRepository.GetByUserAsync(item.UserId);
                        if (result != null)
                        {
                            var company = await _companyRepository.GetByIdAsync(result.CompanyId);
                            name = company.Name;
                        }

                        CommentDTO comment = new CommentDTO();
                        comment.Id = item.Id;
                        comment.Content = item.Content;
                        comment.OwnerName = result == null ? $"{user.FirstName} {user.LastName}" : $"{user.FirstName} {user.LastName}({name})";
                        comment.OwnerImage = user.UrlImage;


                        if (item.Replies != null && item.Replies.Count != 0)
                        {
                            foreach (var rep in item.Replies)
                            {
                                user = await _accountService.GetUsersById(rep.UserId);
                                result = await _companyUserRepository.GetByUserAsync(rep.UserId);
                                if (result != null)
                                {
                                    var company = await _companyRepository.GetByIdAsync(result.CompanyId);
                                    name = company.Name;
                                }

                                ReplyDTO dto = new();
                                dto.Id = rep.Id;
                                dto.Content = rep.Content;
                                dto.OwnerName = result == null ? $"{user.FirstName} {user.LastName}" : $"{user.FirstName} {user.LastName}({name})";
                                dto.OwnerImage = user.UrlImage;

                                list.Add(dto);
                            }
                        }
                        comment.Replies = list;

                        resultDTO.Add(comment);
                    }
                }

                response.Comments = resultDTO;
            }

            return response;
        }

    }
}
