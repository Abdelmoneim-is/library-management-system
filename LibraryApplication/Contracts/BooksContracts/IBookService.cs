using LibraryApi.Common;
using LibraryApplication.Common;
using LibraryApplication.Dtos.Books.BooksDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Contracts.BooksContracts
{
    public interface IBookService
    {
        Task<Result<PaginatedResult<BookDto>>> GetAllAsync(BooksQueryParams queryParams, CancellationToken ct = default);
        Task<Result<BookDto>> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Result<BookDto>> CreateAsync(CreateBookDto dto, CancellationToken ct = default);
        Task<Result> UpdateAsync(int id, UpdateBookDto dto, CancellationToken ct = default);
        Task<Result> DeleteBookAsync(int id, CancellationToken ct = default);
    }
}
