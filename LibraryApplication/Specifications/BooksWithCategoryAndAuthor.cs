using LibraryApplication.Common;
using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Specifications
{
    internal class BooksWithCategoryAndAuthor : BaseSpecification<Books , int>
    {
        public BooksWithCategoryAndAuthor(BooksQueryParams queryParams)
             : base(b => (!queryParams.AuthorId.HasValue || b.AuthorId == queryParams.AuthorId)
                 && (!queryParams.CategoryId.HasValue || b.CategoryId == queryParams.CategoryId)
                 && (string.IsNullOrWhiteSpace(queryParams.SearchValue)
                     || b.Title.ToLower().Contains(queryParams.SearchValue.ToLower())
                     || b.ISBN.ToLower().Contains(queryParams.SearchValue.ToLower())
                     || b.Publisher.ToLower().Contains(queryParams.SearchValue.ToLower())))
        {
            AddInclude(b => b.BookAuthor);
            AddInclude(b => b.BookCategory);

            switch (queryParams.Sorting)
            {
                case BookQuerySorting.DescindingName:
                    AddOrderByDescinding(b => b.Title);
                    break;
                case BookQuerySorting.AscendingName:
                    AddOrder(b => b.Title);
                    break;
                case BookQuerySorting.DescindingPublisherYear:
                    AddOrderByDescinding(b => b.PublishedYear);
                    break;
                case BookQuerySorting.AscendingPublisherYear:
                    AddOrder(b => b.PublishedYear);
                    break;
                default:
                    AddOrder(b => b.Id);
                    break;

            }

            ApplyPagination(queryParams.PageSize, queryParams.PageIndex);
        }


        public BooksWithCategoryAndAuthor(int id) : base(x => x.Id == id)
        {
            AddInclude(b => b.BookAuthor);
            AddInclude(b => b.BookCategory);
        }
    }
}
