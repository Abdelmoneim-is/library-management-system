using AutoMapper;
using LibraryApplication.Dtos.Books.BooksDto;
using LibraryDomain.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Profiles
{
    internal class PictureURLResolver : IValueResolver<Books, BookDto, string>
    {
        private readonly URLSettings settings;
        public PictureURLResolver(IOptions<URLSettings> options)
        {
            settings = options.Value;
        }
        public string Resolve(Books source, BookDto destination, string destMember, ResolutionContext context)
        {
            var baseURL = settings?.BaseURL?.TrimEnd('/') ?? string.Empty;

            // التأكد من أن رابط الصورة ليس فارغاً
            if (string.IsNullOrEmpty(source?.CoverImageURL))
            {
                return string.Empty;
            }

            var pathFile = source.CoverImageURL.TrimStart('/');
            return $"{baseURL}/Folder/{pathFile}";
        }


    }

    public class URLSettings
    {
        public string BaseURL { get; set; } 
    }
}
