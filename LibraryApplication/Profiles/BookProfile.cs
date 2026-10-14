using AutoMapper;
using LibraryApplication.Dtos.Books.BooksDto;
using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Profiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Books, BookDto>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.BookAuthor.FullName))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.BookCategory.Name))
                .ForMember(dest => dest.CoverImageURL, opt => opt.MapFrom<PictureURLResolver>());

            CreateMap<CreateBookDto, Books>();
            CreateMap<UpdateBookDto, Books>();

        }
    }
}
