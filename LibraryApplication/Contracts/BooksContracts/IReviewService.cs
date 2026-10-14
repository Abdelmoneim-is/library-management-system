using LibraryApi.Common;
using LibraryApplication.Dtos.Reviews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Contracts.BooksContracts
{
    public interface IReviewService
    {
        Task<Result<ReviewDto>> AddReviewAsync(string userId, int BookId, CreateReviewDto dto, CancellationToken ct = default);
        Task<Result<ReviewDto>> UpdateReviewAsync(string userId , int reviewId , UpdateReviewDto dto, CancellationToken ct = default);
        Task<Result<bool>> DeleteReviewAsync(string userId, bool isAdmin, int reviewId, CancellationToken ct = default);
        Task<Result<IReadOnlyList<ReviewDto>>> GetBookReviewsAsync(int bookId, CancellationToken ct = default);
        Task<Result<BookRatingSummaryDto>> GetBookRatingSummaryAsync(int bookId, CancellationToken ct = default);
        Task<Result<ReviewDto>> GetMyReviewForBookAsync(string userId, int bookId, CancellationToken ct = default);   
    }
}
