using AutoMapper;
using MediSearch.Core.Application.Dtos.Account;
using MediSearch.Core.Application.Dtos.Company;
using MediSearch.Core.Application.Dtos.Product;
using MediSearch.Core.Application.Features.Account.Commands.Authenticate;
using MediSearch.Core.Application.Features.Account.Commands.RegisterClient;
using MediSearch.Core.Application.Features.Account.Commands.RegisterCompany;
using MediSearch.Core.Application.Features.Admin.Commands.EditProfile;
using MediSearch.Core.Application.Features.Admin.Commands.EditProfileCompany;
using MediSearch.Core.Application.Features.Admin.Commands.RegisterEmployee;
using MediSearch.Core.Application.Features.Home.Command.AddFavoriteCompany;
using MediSearch.Core.Application.Features.Home.Command.AddFavoriteProduct;
using MediSearch.Core.Application.Features.Product.Command.AddComment;
using MediSearch.Core.Application.Features.Product.Command.AddReply;
using MediSearch.Core.Application.Features.Product.Command.UpdateProduct;
using MediSearch.Core.Application.Features.Product.CreateProduct;
using MediSearch.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Mappings
{
	public class GeneralProfile : Profile
	{
		public GeneralProfile()
		{
			#region Account
			CreateMap<AuthenticationRequest, AuthenticateCommand>()
				.ReverseMap();

			CreateMap<RegisterRequest, RegisterClientCommand>()
				.ForMember(x => x.Image, opt => opt.Ignore())
				.ReverseMap()
				.ForMember(x => x.UrlImage, opt => opt.Ignore());

            CreateMap<RegisterCompanyRequest, RegisterCompanyCommand>()
                .ForMember(x => x.Image, opt => opt.Ignore())
				.ForMember(x => x.ImageLogo, opt => opt.Ignore())
				.ForMember(x => x.CompanyType, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(x => x.UrlImage, opt => opt.Ignore())
				.ForMember(x => x.UrlImageLogo, opt => opt.Ignore())
				.ForMember(x => x.CompanyTypeId, opt => opt.Ignore());

            CreateMap<RegisterEmployeeRequest, RegisterEmployeeCommand>()
                .ReverseMap();

            CreateMap<EditProfileCommand, EditProfileCommandRequest>()
                .ReverseMap()
                .ForMember(x => x.Id, opt => opt.Ignore());

            CreateMap<EditProfileCommand, UserDTO>()
                .ForMember(x => x.Role, opt => opt.Ignore())
                .ForMember(x => x.Email, opt => opt.Ignore())
                .ForMember(x => x.CompanyId, opt => opt.Ignore())
                .ForMember(x => x.PhoneNumber, opt => opt.Ignore())
                .ReverseMap();
            #endregion

            #region Product
            CreateMap<Product, CreateProductCommand>()
                .ForMember(x => x.Images, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(x => x.UrlImages, opt => opt.Ignore());

            CreateMap<Product, UpdateProductCommand>()
                .ForMember(x => x.Images, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(x => x.UrlImages, opt => opt.Ignore());

            CreateMap<Product, ProductDTO>()
                .ForMember(X => X.Comments, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(x => x.Comments, opt => opt.Ignore())
                .ForMember(x => x.Company, opt => opt.Ignore())
                .ForMember(x => x.CompanyId, opt => opt.Ignore())
                .ForMember(x => x.Created, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.LastModified, opt => opt.Ignore())
                .ForMember(x => x.LastModifiedBy, opt => opt.Ignore());
            #endregion

            #region Company
            CreateMap<Company, CompanyDTO>()
                .ForMember(x => x.IsFavorite, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(x => x.Ceo, opt => opt.Ignore())
                .ForMember(x => x.Email, opt => opt.Ignore())
                .ForMember(x => x.WebSite, opt => opt.Ignore())
                .ForMember(x => x.Facebook, opt => opt.Ignore())
                .ForMember(x => x.Instagram, opt => opt.Ignore())
                .ForMember(x => x.Twitter, opt => opt.Ignore())
                .ForMember(x => x.CompanyType, opt => opt.Ignore())
                .ForMember(x => x.CompanyTypeId, opt => opt.Ignore())
                .ForMember(x => x.CompanyUsers, opt => opt.Ignore())
                .ForMember(x => x.Products, opt => opt.Ignore())
                .ForMember(x => x.Created, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.LastModified, opt => opt.Ignore())
                .ForMember(x => x.LastModifiedBy, opt => opt.Ignore());
            
            CreateMap<Company, CompanyDetailsDTO>()
                .ForMember(x => x.Products, opt => opt.Ignore())
                .ForMember(x => x.IsFavorite, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(x => x.CompanyType, opt => opt.Ignore())
                .ForMember(x => x.CompanyTypeId, opt => opt.Ignore())
                .ForMember(x => x.CompanyUsers, opt => opt.Ignore())
                .ForMember(x => x.Products, opt => opt.Ignore())
                .ForMember(x => x.Created, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.LastModified, opt => opt.Ignore())
                .ForMember(x => x.LastModifiedBy, opt => opt.Ignore());

            CreateMap<EditProfileCompanyCommand, EditProfileCompanyCommandRequest>()
                .ReverseMap()
                .ForMember(x => x.Id, opt => opt.Ignore());
            #endregion

            #region Comment
            CreateMap<Comment, AddCommentCommand>()
                .ReverseMap()
                .ForMember(x => x.Created, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.LastModified, opt => opt.Ignore())
                .ForMember(x => x.LastModifiedBy, opt => opt.Ignore())
                .ForMember(x => x.Product, opt => opt.Ignore())
                .ForMember(x => x.Replies, opt => opt.Ignore());
            #endregion

            #region Reply
            CreateMap<Reply, AddReplyCommand>()
                .ReverseMap()
                .ForMember(x => x.Created, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.LastModified, opt => opt.Ignore())
                .ForMember(x => x.LastModifiedBy, opt => opt.Ignore())
                .ForMember(x => x.Comment, opt => opt.Ignore());
            #endregion

            #region Favorites
            CreateMap<FavoriteCompany, AddFavoriteCompanyCommand>()
                .ReverseMap()
                .ForMember(x => x.Company, opt => opt.Ignore())
                .ForMember(x => x.Created, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.LastModified, opt => opt.Ignore())
                .ForMember(x => x.LastModifiedBy, opt => opt.Ignore());

            CreateMap<FavoriteProduct, AddFavoriteProductCommand>()
                .ReverseMap()
                .ForMember(x => x.Product, opt => opt.Ignore())
                .ForMember(x => x.Created, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.LastModified, opt => opt.Ignore())
                .ForMember(x => x.LastModifiedBy, opt => opt.Ignore());
            #endregion
        }
    }
}
