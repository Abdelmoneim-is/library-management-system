using LibraryApi.Common;
using LibraryApplication.Common;
using LibraryApplication.Common.BorrowingsQueryParams;
using LibraryApplication.Dtos.Borrowings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Contracts.BooksContracts
{
    public interface IBorrowingService
    {
        Task<Result<BorrowingDto>> BorrowBookAsync(string userId, CreateBrowingDto dto, CancellationToken ct = default);
        Task<Result> ReturnBookAsync(string userId, bool isAdmin, int borrowindId, CancellationToken ct = default);
        Task<Result<IReadOnlyList<BorrowingDto>>> GetMyLoansAsync(string userId, CancellationToken ct = default);
        Task<Result<PaginatedResult<BorrowingDto>>> GetAllBorrowingsAsync(BorrowingsQueryParams queryParams, CancellationToken ct = default); Task<Result> ExtendBorrowingAsync(string userId, int borrowingId, CancellationToken ct = default);
        Task<Result> PayFineAsync(int borrowingId, CancellationToken ct = default);
        Task<Result<IReadOnlyList<BorrowingDto>>> GetUnpaidFinesAsync(CancellationToken ct = default);
        Task<Result<IReadOnlyList<BorrowingDto>>> GetOverdueBorrowingsAsync(CancellationToken ct = default);
    }
}
