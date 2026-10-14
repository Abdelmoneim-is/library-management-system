using AutoMapper;
using LibraryApi.Common;
using LibraryApplication.Common;
using LibraryApplication.Contracts.BooksContracts;
using LibraryApplication.Dtos.Books.BooksDto;
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
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<BookDto>> CreateAsync(CreateBookDto dto, CancellationToken ct = default)
        {
            var bookRepo = _unitOfWork.GetGenericRepository<Books, int>();
            var authorRepo = _unitOfWork.GetGenericRepository<BookAuthor, int>();
            var categoryRepo = _unitOfWork.GetGenericRepository<BookCategory, int>();

            if (await authorRepo.GetByIdAsync(dto.AuthorId, ct) is null)
                return Result<BookDto>.Fail(Errors.NotFound("Author.NotFound", $"Author With Id {dto.AuthorId} Is Not Found"));

            if (await categoryRepo.GetByIdAsync(dto.CategoryId, ct) is null)
                return Result<BookDto>.Fail(Errors.NotFound("Category.NotFound", $"Category With Id {dto.CategoryId} Is Not Found"));

            if (await bookRepo.CountAsync(new BookByIsbnSpecification(dto.ISBN), ct) > 0)
                return Result<BookDto>.Fail(Errors.Confilict("Book.Conflict", $"A book with ISBN '{dto.ISBN}' already exists."));

            var book = _mapper.Map<Books>(dto);
            book.AvailableCopies = dto.TotalCopies;
            book.CreatedAt = DateTime.UtcNow;
            book.UpdatedAt = DateTime.UtcNow;

            bookRepo.Add(book);
            await _unitOfWork.SaveChanges(ct);

            var resultDto = _mapper.Map<BookDto>(book);
            return Result<BookDto>.Ok(resultDto);
        }

        public async Task<Result> DeleteBookAsync(int id, CancellationToken ct = default)
        {
            var bookRepo = _unitOfWork.GetGenericRepository<Books, int>();
            var borrowingRepo = _unitOfWork.GetGenericRepository<Borrowing, int>();

            var book = await bookRepo.GetByIdAsync(id, ct);
            if (book is null)
                return Result.Fail(Errors.NotFound("Book.NotFound", $"Book With Id {id} Is Not Found"));

            if (await borrowingRepo.CountAsync(new BorrowingsByBookSpecification(id), ct) > 0)
                return Result.Fail(Errors.Confilict("Book.HasBorrowings",
                    "Cannot delete this book because it has borrowing history."));

            bookRepo.Delete(book);
            await _unitOfWork.SaveChanges(ct);

            return Result.Ok();
        }

        public async Task<Result<PaginatedResult<BookDto>>> GetAllAsync(BooksQueryParams queryParams, CancellationToken ct = default)
        {
            var repo = _unitOfWork.GetGenericRepository<Books, int>();

            var books = await repo.GetAllAsync(new BooksWithCategoryAndAuthor(queryParams), ct);
            var bookDtos = _mapper.Map<IReadOnlyList<BookDto>>(books);

            var totalCount = await repo.CountAsync(new BooksCount(queryParams), ct);

            var paginated = new PaginatedResult<BookDto>(queryParams.PageSize, queryParams.PageIndex, totalCount, bookDtos);
            return Result<PaginatedResult<BookDto>>.Ok(paginated);
        }

        public async Task<Result<BookDto>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var repo = _unitOfWork.GetGenericRepository<Books, int>();
            var book = await repo.GetByIdAsync(new BooksWithCategoryAndAuthor(id), ct);

            if (book is null)
                return Result<BookDto>.Fail(Errors.NotFound("Book.NotFound", $"Book With Id {id} Is Not Found"));

            var bookDto = _mapper.Map<BookDto>(book);

            var reviewRepo = _unitOfWork.GetGenericRepository<BookReview, int>();
            var reviews = await reviewRepo.GetAllAsync(new ReviewsByBookIdSpecification(id), ct);

            bookDto.ReviewsCount = reviews.Count;
            bookDto.AverageRating = reviews.Count > 0 ? Math.Round(reviews.Average(r => r.Rating), 1) : 0;

            return Result<BookDto>.Ok(bookDto);
        }

        public async Task<Result> UpdateAsync(int id, UpdateBookDto dto, CancellationToken ct = default)
        {
            var bookRepo = _unitOfWork.GetGenericRepository<Books, int>();
            var authorRepo = _unitOfWork.GetGenericRepository<BookAuthor, int>();
            var categoryRepo = _unitOfWork.GetGenericRepository<BookCategory, int>();

            var book = await bookRepo.GetByIdAsync(id, ct);
            if (book is null)
                return Result.Fail(Errors.NotFound("Book.NotFound", $"Book With Id {id} Is Not Found"));

            if (await authorRepo.GetByIdAsync(dto.AuthorId, ct) is null)
                return Result.Fail(Errors.NotFound("Author.NotFound", $"Author With Id {dto.AuthorId} Is Not Found"));

            if (await categoryRepo.GetByIdAsync(dto.CategoryId, ct) is null)
                return Result.Fail(Errors.NotFound("Category.NotFound", $"Category With Id {dto.CategoryId} Is Not Found"));

            if (await bookRepo.CountAsync(new BookByIsbnSpecification(dto.ISBN, id), ct) > 0)
                return Result.Fail(Errors.Confilict("Book.Conflict", $"Another book with ISBN '{dto.ISBN}' already exists."));

            var borrowedCopies = book.TotalCopies - book.AvailableCopies;
            var newAvailable = dto.TotalCopies - borrowedCopies;

            if (newAvailable < 0)
                return Result.Fail(Errors.Validation("Book.InvalidTotalCopies",
                    $"Total copies cannot be less than the {borrowedCopies} copies currently borrowed."));

            _mapper.Map(dto, book);
            book.AvailableCopies = newAvailable;
            book.UpdatedAt = DateTime.UtcNow;

            bookRepo.Update(book);
            await _unitOfWork.SaveChanges(ct);

            return Result.Ok();
        }
    }
}
