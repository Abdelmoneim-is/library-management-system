using AutoMapper;
using LibraryApplication.Dtos.Borrowings;
using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Profiles
{
    public class BorrowingProfile : Profile
    {
        public BorrowingProfile()
        {
            CreateMap<Borrowing, BorrowingDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title));
        }
    }
}
