using LibraryApplication.Contracts.BooksContracts;
using LibraryApplication.Services.BookServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication
{
    public static class ApplicationServicesRegisteration
    {
        public static IServiceCollection ApplicationService(this IServiceCollection services)
        {
            services.AddAutoMapper(c => { }, typeof(ApplicationServicesRegisteration).Assembly);
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IAuthorService, AuthorService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IBorrowingService, BorrowingService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IWishlistService, WishlistService>();


            return services;
        }
    }
}
