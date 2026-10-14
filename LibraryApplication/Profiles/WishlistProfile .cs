using AutoMapper;
using LibraryApplication.Dtos.Wishlist;
using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Profiles
{
    public class WishlistProfile : Profile
    {
        public WishlistProfile()
        {
            CreateMap<WishlistItem , WishistItemDto> ()
                .ForMember(d => d.Title , o => o.MapFrom(s => s.Book.Title))
                .ForMember(d => d.AuthorName , o => o.MapFrom(s => s.Book.BookAuthor.FullName))
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Book.BookCategory.Name))
                .ForMember(d => d.CoverImageURL, o => o.MapFrom(s => s.Book.CoverImageURL))
                .ForMember(d => d.AvailableCopies, o => o.MapFrom(s => s.Book.AvailableCopies))
                .ForMember(d => d.IsAvailable, o => o.MapFrom(s => s.Book.AvailableCopies > 0));
        }
    }
}
