using LibraryApi.Common;
using LibraryApplication.Common;
using LibraryApplication.Dtos.Books.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Contracts.BooksContracts
{
    public interface ICategoryService
    {
        Task<Result<PaginatedResult<CategoryDto>>> GetAllCategoryAsync(CategoryQueryParams queryParams , CancellationToken ct = default);
        Task<Result<CategoryDto>> GetCategoryByIdAsync(int id, CancellationToken ct = default);
        Task<Result<CategoryDto>> CreateCategoryAsync(CreateCategoryDto categoryDto, CancellationToken ct = default);
        Task<Result> UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto, CancellationToken ct = default);
        Task<Result> DeleteCategoryAsync(int id , CancellationToken ct = default);
    }
}
