using LibraryApi.Common;
using LibraryApplication.Common;
using LibraryApplication.Common.BorrowingsQueryParams;
using LibraryApplication.Contracts.BooksContracts;
using LibraryApplication.Dtos.Borrowings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryApi.Controllers.Books
{
    [Authorize]
    public class BorrowingsController : APIBaseController
    {
        private readonly IBorrowingService _borrowingService;

        public BorrowingsController(IBorrowingService borrowingService)
        {
            _borrowingService = borrowingService;
        }

        private string currentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        private bool isAdmin => User.IsInRole("Admin");

        [HttpPost]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<BorrowingDto>> Borrow (CreateBrowingDto dto, CancellationToken ct)
        {
            var result = await _borrowingService.BorrowBookAsync(currentUserId, dto , ct);
            return ToActionResult(result);
        }
        [HttpPost("{id:int}/return")]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status409Conflict)]

        public async Task<ActionResult> Return (int id , CancellationToken ct)
        {
            var result = await _borrowingService.ReturnBookAsync(currentUserId, isAdmin, id , ct);
            return ToActionResult(result , "Book returned successfully.");
        }

        [HttpGet("MyLoans")]
        public async Task<ActionResult<IReadOnlyList<BorrowingDto>>> MyLoans (CancellationToken ct)
        {
            var result = await _borrowingService.GetMyLoansAsync(currentUserId, ct);
            return ToActionResult(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/all")]
        public async Task<ActionResult<PaginatedResult<BorrowingDto>>> AllBorrowings(
            [FromQuery] BorrowingsQueryParams queryParams, CancellationToken ct)
        {
            var result = await _borrowingService.GetAllBorrowingsAsync(queryParams, ct);
            return ToActionResult(result);
        }

        [HttpPost("{id:int}/extend")]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Extend(int id, CancellationToken ct)
        {
            var result = await _borrowingService.ExtendBorrowingAsync(currentUserId, id, ct);
            return ToActionResult(result, "Loan extended successfully.");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/pay-fine")]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> PayFine(int id, CancellationToken ct)
        {
            var result = await _borrowingService.PayFineAsync(id, ct);
            return ToActionResult(result, "Fine paid successfully.");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/unpaid-fines")]
        public async Task<ActionResult<IReadOnlyList<BorrowingDto>>> UnpaidFines(CancellationToken ct)
        {
            var result = await _borrowingService.GetUnpaidFinesAsync(ct);
            return ToActionResult(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/overdue")]
        public async Task<ActionResult<IReadOnlyList<BorrowingDto>>> Overdue(CancellationToken ct)
        {
            var result = await _borrowingService.GetOverdueBorrowingsAsync(ct);
            return ToActionResult(result);
        }
    }
}
