using AutoMapper;
using LibraryApplication.Dtos.Books.Authors;
using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Profiles
{
    public class AuthorProfile : Profile
    {
        public AuthorProfile()
        {
            CreateMap<BookAuthor, AuthorDto>();
            CreateMap<CreateAuthorDto, BookAuthor>();
            CreateMap<UpdateAuthorDto, BookAuthor>();
        }
    }
}
