using LibraryApi.Common;
using LibraryApplication.Contracts.BooksContracts;
using LibraryApplication.Dtos.Reviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryApi.Controllers.Books
{

    public class ReviewsController : APIBaseController
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        [HttpGet("books/{bookId:int}/reviews")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IReadOnlyList<ReviewDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<ReviewDto>>> GetBookReviews(int bookId, CancellationToken ct)
        {
            var result = await _reviewService.GetBookReviewsAsync(bookId, ct);
            return ToActionResult(result);
        }

        [HttpPost("books/{bookId:int}/reviews")]
        [Authorize]
        [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ReviewDto>> AddReview (int bookId , [FromBody] CreateReviewDto dto , CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _reviewService.AddReviewAsync(userId, bookId, dto, ct);
            return ToActionResult(result);
        }

        [HttpPut("reviews/{reviewId:int}")]
        [Authorize]
        [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReviewDto>> UpdateReview (int reviewId , [FromBody] UpdateReviewDto dto , CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _reviewService.UpdateReviewAsync(userId , reviewId , dto, ct);
            return ToActionResult(result);
        }

        [HttpDelete("reviews/{reviewId:int}")]
        [Authorize]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> DeleteReview (int reviewId , CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var isAdmin = User.IsInRole("Admin");

            var result = await _reviewService.DeleteReviewAsync(userId, isAdmin, reviewId, ct);
            return ToActionResult(result);
        }


        [HttpGet("books/{bookId:int}/rating-summary")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(BookRatingSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookRatingSummaryDto>> GetBookRatingSummary(int bookId, CancellationToken ct)
        {
            var result = await _reviewService.GetBookRatingSummaryAsync(bookId, ct);
            return ToActionResult(result);
        }

        [HttpGet("books/{bookId:int}/reviews/mine")]
        [Authorize]
        [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReviewDto>> GetMyReview(int bookId, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _reviewService.GetMyReviewForBookAsync(userId, bookId, ct);
            return ToActionResult(result);
        }

    }
}
