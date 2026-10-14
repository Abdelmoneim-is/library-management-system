using AutoMapper;
using LibraryApi.Common;
using LibraryApplication.Common;
using LibraryApplication.Contracts.BooksContracts;
using LibraryApplication.Dtos.Books.Categories;
using LibraryApplication.Specifications;
using LibraryDomain.Contracts;
using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Services.BookServices
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<CategoryDto>> CreateCategoryAsync(CreateCategoryDto categoryDto, CancellationToken ct = default)
        {

            var repo = _unitOfWork.GetGenericRepository<BookCategory, int>();

            if (await repo.CountAsync(new CategoryByNameSpecification(categoryDto.Name), ct) > 0)
                return Result<CategoryDto>.Fail(Errors.Confilict("Category.Conflict", $"A category named '{categoryDto.Name}' already exists."));

            var data = _mapper.Map<BookCategory>(categoryDto);
            data.CreatedAt = DateTime.UtcNow;
            data.UpdatedAt = DateTime.UtcNow;

            repo.Add(data);
            await _unitOfWork.SaveChanges(ct);

            var resultDto = _mapper.Map<CategoryDto>(data);
            return Result<CategoryDto>.Ok(resultDto);
        }

        public async Task<Result> DeleteCategoryAsync(int id, CancellationToken ct = default)
        {
            var categoryRepo = _unitOfWork.GetGenericRepository<BookCategory, int>();
            var bookRepo = _unitOfWork.GetGenericRepository<Books, int>();

            var category = await categoryRepo.GetByIdAsync(id, ct);
            if (category is null)
                return Result.Fail(Errors.NotFound("Category.NotFound", $"Category With Id {id} Is Not Found"));

            if (await bookRepo.CountAsync(new BooksByCategorySpecification(id), ct) > 0)
                return Result.Fail(Errors.Confilict("Category.HasBooks", "Cannot delete this category because it contains books."));

            categoryRepo.Delete(category);
            await _unitOfWork.SaveChanges(ct);

            return Result.Ok();
        }

        public async Task<Result<PaginatedResult<CategoryDto>>> GetAllCategoryAsync(CategoryQueryParams queryParams , CancellationToken ct = default)
        {
            var repo = _unitOfWork.GetGenericRepository<BookCategory, int>();

            var categories = await repo.GetAllAsync(new CategoriesSpecifications(queryParams), ct);
            var data = _mapper.Map<IReadOnlyList<CategoryDto>>(categories);

            var totalCount = await repo.CountAsync(new CategoriesCount(queryParams), ct);

            var paginated = new PaginatedResult<CategoryDto>(queryParams.PageSize, queryParams.PageIndex, totalCount, data);
            return Result<PaginatedResult<CategoryDto>>.Ok(paginated);
        }

        public async Task<Result<CategoryDto>> GetCategoryByIdAsync(int id, CancellationToken ct = default)
        {
            var category = await _unitOfWork.GetGenericRepository<BookCategory, int>().GetByIdAsync(id, ct);
            if (category is null)
                return Result<CategoryDto>.Fail(Errors.NotFound("Category.NotFound", $"Category With Id {id} Is Not Found"));

            var data = _mapper.Map<CategoryDto>(category);
            return Result<CategoryDto>.Ok(data);
        }

        public async Task<Result> UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto, CancellationToken ct = default)
        {
            var categoryRepo = _unitOfWork.GetGenericRepository<BookCategory, int>();

            var category = await categoryRepo.GetByIdAsync(id, ct);
            if (category is null)
                return Result.Fail(Errors.NotFound("Category.NotFound", $"Category With Id {id} Is Not Found"));

            if (await categoryRepo.CountAsync(new CategoryByNameSpecification(categoryDto.Name, id), ct) > 0)
                return Result.Fail(Errors.Confilict("Category.Conflict", $"Another category with the name '{categoryDto.Name}' already exists."));

            _mapper.Map(categoryDto, category);
            category.UpdatedAt = DateTime.UtcNow;

            categoryRepo.Update(category);
            await _unitOfWork.SaveChanges(ct);

            return Result.Ok();
        }
    }
}
