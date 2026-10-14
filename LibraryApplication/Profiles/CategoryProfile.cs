using AutoMapper;
using LibraryApplication.Dtos.Books.Categories;
using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<BookCategory, CategoryDto>();
            CreateMap<CreateCategoryDto, BookCategory>();
            CreateMap<UpdateCategoryDto, BookCategory>();
        }
    }
}
