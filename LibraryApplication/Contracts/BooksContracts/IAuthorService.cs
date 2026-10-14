using LibraryApi.Common;
using LibraryApplication.Common;
using LibraryApplication.Dtos.Books.Authors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Contracts.BooksContracts
{
    public interface IAuthorService
    {
        Task<Result<PaginatedResult<AuthorDto>>> GetAllAsync(AuthorQueryParams queryParams , CancellationToken ct = default);
        Task<Result<AuthorDto>> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Result<AuthorDto>> CreateAsync(CreateAuthorDto dto, CancellationToken ct = default);
        Task<Result> UpdateAsync(int id, UpdateAuthorDto dto, CancellationToken ct = default);
        Task<Result> DeleteAuthorAsync(int id, CancellationToken ct = default);
    }
}
