using AutoMapper;
using LibraryApplication.Dtos.Reviews;
using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Profiles
{
    public class ReviewMappingProfile : Profile
    {
        public ReviewMappingProfile()
        {
            CreateMap<BookReview, ReviewDto>()
           .ForMember(d => d.UserName, opt => opt.Ignore())
           .ForMember(d => d.DisplayName, opt => opt.Ignore());
        }
       
    }
}
