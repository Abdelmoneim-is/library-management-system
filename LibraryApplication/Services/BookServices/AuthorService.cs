using AutoMapper;
using LibraryApi.Common;
using LibraryApplication.Common;
using LibraryApplication.Contracts.BooksContracts;
using LibraryApplication.Dtos.Books.Authors;
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
    public class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuthorService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<AuthorDto>> CreateAsync(CreateAuthorDto dto, CancellationToken ct = default)
        {
            var repo = _unitOfWork.GetGenericRepository<BookAuthor, int>();

            if (await repo.CountAsync(new AuthorByNameSpecification(dto.FullName), ct) > 0)
                return Result<AuthorDto>.Fail(Errors.Confilict("Author.Conflict", $"An author with the same name '{dto.FullName}' already exists."));

            var author = _mapper.Map<BookAuthor>(dto);
            author.CreatedAt = DateTime.UtcNow;
            author.UpdatedAt = DateTime.UtcNow;

            repo.Add(author);
            await _unitOfWork.SaveChanges(ct);

            var resultDto = _mapper.Map<AuthorDto>(author);
            return Result<AuthorDto>.Ok(resultDto);
        }

        public async Task<Result> DeleteAuthorAsync(int id, CancellationToken ct = default)
        {
            var authorRepo = _unitOfWork.GetGenericRepository<BookAuthor, int>();
            var bookRepo = _unitOfWork.GetGenericRepository<Books, int>();

            var author = await authorRepo.GetByIdAsync(id, ct);
            if (author is null)
                return Result.Fail(Errors.NotFound("Author.NotFound", $"Author With Id {id} Is Not Found"));

            if (await bookRepo.CountAsync(new BooksByAuthorSpecification(id), ct) > 0)
                return Result.Fail(Errors.Confilict("Author.HasBooks", "Cannot delete this author because they have associated books."));

            authorRepo.Delete(author);
            await _unitOfWork.SaveChanges(ct);

            return Result.Ok();
        }

        public async Task<Result<PaginatedResult<AuthorDto>>> GetAllAsync(AuthorQueryParams queryParams, CancellationToken ct = default)
        {
            var repo = _unitOfWork.GetGenericRepository<BookAuthor, int>();

            var authors = await repo.GetAllAsync(new AuthorsSpecifications(queryParams), ct);
            var authorDtos = _mapper.Map<IReadOnlyList<AuthorDto>>(authors);

            var totalAuthors = await repo.CountAsync(new AuthorsCount(queryParams), ct);

            var paginated = new PaginatedResult<AuthorDto>(queryParams.PageSize, queryParams.PageIndex, totalAuthors, authorDtos);
            return Result<PaginatedResult<AuthorDto>>.Ok(paginated);
        }

        public async Task<Result<AuthorDto>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var repo = _unitOfWork.GetGenericRepository<BookAuthor, int>();
            var author = await repo.GetByIdAsync(id, ct);

            if (author is null)
                return Result<AuthorDto>.Fail(Errors.NotFound("Author.NotFound", $"Author With Id {id} Is Not Found"));

            var authorDto = _mapper.Map<AuthorDto>(author);
            return Result<AuthorDto>.Ok(authorDto);
        }

        public async Task<Result> UpdateAsync(int id, UpdateAuthorDto dto, CancellationToken ct = default)
        {
            var authorRepo = _unitOfWork.GetGenericRepository<BookAuthor, int>();

            var author = await authorRepo.GetByIdAsync(id, ct);
            if (author is null)
                return Result.Fail(Errors.NotFound("Author.NotFound", $"Author With Id {id} Is Not Found"));

            if (await authorRepo.CountAsync(new AuthorByNameSpecification(dto.FullName, id), ct) > 0)
                return Result.Fail(Errors.Confilict("Author.Conflict", $"Another author with the name '{dto.FullName}' already exists."));

            _mapper.Map(dto, author);
            author.UpdatedAt = DateTime.UtcNow;

            authorRepo.Update(author);
            await _unitOfWork.SaveChanges(ct);

            return Result.Ok();
        }
    }
}
