using AutoMapper;
using LibraryApi.Common;
using LibraryApplication.Common;
using LibraryApplication.Common.BorrowingsQueryParams;
using LibraryApplication.Contracts.BooksContracts;
using LibraryApplication.Dtos.Borrowings;
using LibraryApplication.Specifications;
using LibraryDomain.Contracts;
using LibraryDomain.Entities;
using LibraryDomain.Exceptions; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Services.BookServices
{
    public class BorrowingService : IBorrowingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;

        public BorrowingService(IUnitOfWork unitOfWork , IMapper mapper , IIdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _identityService = identityService;
        }
        public async Task<Result<BorrowingDto>> BorrowBookAsync(string userId, CreateBrowingDto dto, CancellationToken ct = default)
        {
            var bookrepo = _unitOfWork.GetGenericRepository<Books, int>();
            var borrowingrepo = _unitOfWork.GetGenericRepository<Borrowing, int>();

            var book = await bookrepo.GetByIdAsync(dto.BookId, ct);
            if (book is null)
                return Result<BorrowingDto>.Fail(Errors.NotFound("Book.NotFound", $"Book With Id {dto.BookId} Is Not Found"));

            if (await borrowingrepo.CountAsync(new UnpaidFinesForUserSpec(userId), ct) > 0)
                return Result<BorrowingDto>.Fail(Errors.Confilict("Borrowing.UnpaidFines",
                    "You have unpaid fines. Please pay them at the library desk before borrowing new books."));

            if (book.AvailableCopies <= 0)
                return Result<BorrowingDto>.Fail(Errors.Confilict("Borrowing.NoCopyAvailable",
                    "No available copies of this book right now."));

            var activeBorrowings = await borrowingrepo.GetAllAsync(new ActiveBorrowingsForUserSpec(userId), ct);

            if (activeBorrowings.Count >= BorrowingConstants.MaxActiveBorrowingsPerUser)
                return Result<BorrowingDto>.Fail(Errors.Validation("Borrowing.LimitExceeded",
                    $"You can't borrow more than {BorrowingConstants.MaxActiveBorrowingsPerUser} books at the same time."));

            if (activeBorrowings.Any(b => b.IsOverDue))
                return Result<BorrowingDto>.Fail(Errors.Confilict("Borrowing.OverdueLoansExist",
                    "You have overdue books. Please return them before borrowing new books."));

            if (activeBorrowings.Any(b => b.BookId == dto.BookId))
                return Result<BorrowingDto>.Fail(Errors.Confilict("Borrowing.AlreadyBorrowed",
                    "You already have an active loan for this book."));

            var now = DateTime.UtcNow;
            var borrowing = new Borrowing
            {
                BookId = dto.BookId,
                UserId = userId,
                BorrowDate = now,
                DueDate = now.AddDays(BorrowingConstants.DefaultBorrowingDurationDays)
            };

            borrowingrepo.Add(borrowing);
            book.AvailableCopies -= 1;
            bookrepo.Update(book);

            try
            {
                await _unitOfWork.SaveChanges(ct);
            }
            catch (ConcurrencyException)
            {
                return Result<BorrowingDto>.Fail(Errors.Confilict("Borrowing.ConcurrencyError",
                    "Another user just borrowed the last copy. Please try again."));
            }

            borrowing.Book = book;
            var resultDto = _mapper.Map<BorrowingDto>(borrowing);
            return Result<BorrowingDto>.Ok(resultDto);
        }

        public async Task<Result> ExtendBorrowingAsync(string userId, int borrowingId, CancellationToken ct = default)
        {
            var borrowingRepo = _unitOfWork.GetGenericRepository<Borrowing, int>();
            var borrowing = await borrowingRepo.GetByIdAsync(borrowingId);

            if (borrowing is null)
                return Result.Fail(Errors.NotFound("Borrowing.NotFound", $"Borrowing With Id {borrowingId} Is Not Found"));

            if (borrowing.UserId != userId)
                return Result.Fail(Errors.Forbidden("Borrowing.Forbidden", "You can only extend your own loans."));

            if (borrowing.IsReturned)
                return Result.Fail(Errors.Confilict("Borrowing.AlreadyReturned", "Cannot extend a returned borrowing."));

            if (borrowing.IsOverDue)
                return Result.Fail(Errors.Confilict("Borrowing.Overdue", "Cannot extend an overdue borrowing. Please return the book."));

            if (await borrowingRepo.CountAsync(new UnpaidFinesForUserSpec(userId), ct) > 0)
                return Result.Fail(Errors.Confilict("Borrowing.UnpaidFines",
                    "You have unpaid fines. Please pay them before extending a loan."));

            if (borrowing.ExtensionCount >= BorrowingConstants.MaxExtensionCount)
                return Result.Fail(Errors.Confilict("Borrowing.MaxExtensionsReached", "You have reached the maximum number of extensions for this loan."));

            borrowing.DueDate = borrowing.DueDate.AddDays(BorrowingConstants.ExtensionDurationDays);
            borrowing.ExtensionCount += 1;

            borrowingRepo.Update(borrowing);
            await _unitOfWork.SaveChanges(ct);

            return Result.Ok();
        }

        public async Task<Result<PaginatedResult<BorrowingDto>>> GetAllBorrowingsAsync(
     BorrowingsQueryParams queryParams, CancellationToken ct = default)
        {
            var repo = _unitOfWork.GetGenericRepository<Borrowing, int>();

            var borrowings = await repo.GetAllAsync(new BorrowingsAdminSpecification(queryParams), ct);
            var dtos = _mapper.Map<IReadOnlyList<BorrowingDto>>(borrowings);

            await AttachUserInfoAsync(dtos, ct);

            var totalCount = await repo.CountAsync(new BorrowingsAdminCount(queryParams), ct);

            var paginated = new PaginatedResult<BorrowingDto>(
                queryParams.PageSize, queryParams.PageIndex, totalCount, dtos);

            return Result<PaginatedResult<BorrowingDto>>.Ok(paginated);
        }

        public async Task<Result<IReadOnlyList<BorrowingDto>>> GetMyLoansAsync(string userId, CancellationToken ct = default)
        {
            var spec = new BorrowingWithBookSpecification(userId);
            var repo = _unitOfWork.GetGenericRepository<Borrowing, int>();
            var borrowing = await repo.GetAllAsync(spec, ct);

            var dtos = _mapper.Map<IReadOnlyList<BorrowingDto>>(borrowing);
            return Result<IReadOnlyList<BorrowingDto>>.Ok(dtos);
        }

        public async Task<Result<IReadOnlyList<BorrowingDto>>> GetOverdueBorrowingsAsync(CancellationToken ct = default)
        {
            var borrowingRepo = _unitOfWork.GetGenericRepository<Borrowing, int>();
            var spec = new OverdueBorrowingsSpec();
            var borrowing = await borrowingRepo.GetAllAsync(spec, ct);

            var dtos = _mapper.Map<IReadOnlyList<BorrowingDto>>(borrowing);
            await AttachUserInfoAsync(dtos, ct);
            return Result<IReadOnlyList<BorrowingDto>>.Ok(dtos);
        }

        public async Task<Result<IReadOnlyList<BorrowingDto>>> GetUnpaidFinesAsync(CancellationToken ct = default)
        {
            var repo = _unitOfWork.GetGenericRepository<Borrowing, int>();

            var borrowings = await repo.GetAllAsync(new UnpaidFinesWithBookSpec(), ct);
            var dtos = _mapper.Map<IReadOnlyList<BorrowingDto>>(borrowings);

            await AttachUserInfoAsync(dtos, ct);

            return Result<IReadOnlyList<BorrowingDto>>.Ok(dtos);
        }

        public async Task<Result> PayFineAsync(int borrowingId, CancellationToken ct = default)
        {
            var borrowingRepo = _unitOfWork.GetGenericRepository<Borrowing, int>();
            var borrowing = await borrowingRepo.GetByIdAsync(borrowingId, ct);

            if (borrowing is null)
                return Result.Fail(Errors.NotFound("Borrowing.NotFound", $"Borrowing With Id {borrowingId} Is Not Found"));

            if (borrowing.FineAmount <= 0)
                return Result.Fail(Errors.Confilict("Borrowing.NoFine", "This borrowing has no fine."));

            if (borrowing.IsFinePaid)
                return Result.Fail(Errors.Confilict("Borrowing.FineAlreadyPaid", "Fine for this borrowing is already paid."));

            borrowing.IsFinePaid = true;
            borrowing.FinePaidAt = DateTime.UtcNow;   

            borrowingRepo.Update(borrowing);
            await _unitOfWork.SaveChanges(ct);

            return Result.Ok();
        }

        public async Task<Result> ReturnBookAsync(string userId, bool isAdmin, int borrowindId, CancellationToken ct = default)
        {
            var bookrepo = _unitOfWork.GetGenericRepository<Books, int>();
            var borrowingrepo = _unitOfWork.GetGenericRepository<Borrowing, int>();

            var borrowing = await borrowingrepo.GetByIdAsync(borrowindId);
            if (borrowing is null)
                return Result.Fail(Errors.NotFound("Borrowing.Not Found", $"Borrowing With Id {borrowindId} Is Not Found"));
            if (!isAdmin && borrowing.UserId != userId)
                return Result.Fail(Errors.Forbidden("Borrowing.Forbidden", "You can only return your own borrowed books."));
            if(borrowing.IsReturned)
                return Result.Fail(Errors.Confilict("Borrowing.AlreadyReturned", "This book has already been returned."));

            borrowing.ReturnDate = DateTime.UtcNow;

            var now = DateTime.UtcNow;
            borrowing.ReturnDate = now;

            if (now > borrowing.DueDate)
            {
                var overdueDays = (int)Math.Ceiling((now - borrowing.DueDate).TotalDays);

                borrowing.FineAmount = overdueDays * BorrowingConstants.DailyFineAmount;
                borrowing.IsFinePaid = false;
            }
            borrowingrepo.Update(borrowing);

            var book = await bookrepo.GetByIdAsync(borrowing.BookId);
            if (book is not null)
            {
                book.AvailableCopies += 1;
                bookrepo.Update(book);
            }

            await _unitOfWork.SaveChanges(ct);
            return Result.Ok();

        }

        private async Task AttachUserInfoAsync(IReadOnlyList<BorrowingDto> dtos, CancellationToken ct)
        {
            if (dtos.Count == 0) return;

            var userMap = await _identityService.GetUsersInfoByIdsAsync(dtos.Select(d => d.UserId), ct);

            foreach (var dto in dtos)
            {
                if (userMap.TryGetValue(dto.UserId, out var info))
                {
                    dto.UserName = info.UserName;
                    dto.DisplayName = info.DisplayName;
                }
            }
        }
    }
}
