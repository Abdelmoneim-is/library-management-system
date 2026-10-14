using LibraryApi.Common;
using LibraryApplication.Contracts.BooksContracts;
using LibraryApplication.Dtos.Wishlist;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryApi.Controllers.Books
{
    
    public class WishListController : APIBaseController
    {
        private readonly IWishlistService _wishlistService;

        public WishListController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        private string currentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<WishistItemDto>>> GetMyWishlist(CancellationToken ct)
        {
            var result = await _wishlistService.GetMyWishlistAsync(currentUserId, ct);
            return ToActionResult(result);
        }

        [HttpGet("{bookId:int}/exists")]
        public async Task<ActionResult<bool>> IsInWishList(int bookId , CancellationToken ct)
        {
            var result = await _wishlistService.IsInWishlistAsync(currentUserId, bookId, ct);
            return ToActionResult(result);
        }

        [HttpPost("{bookId:int}")]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status409Conflict)]

        public async Task<ActionResult> Add (int bookId , CancellationToken ct)
        {
            var result = await _wishlistService.AddAsync(currentUserId, bookId, ct);
            return ToActionResult(result , "Book added to your wishlist.");
        }

        [HttpDelete("{bookId:int}")]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status404NotFound)]

        public async Task<ActionResult> Remove(int bookId, CancellationToken ct)
        {
            var result = await _wishlistService.RemoveAsync(currentUserId, bookId, ct);
            return ToActionResult(result, "Book removed from your wishlist..");
        }

        [HttpGet("ids")]
        [ProducesResponseType(typeof(IReadOnlyList<int>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<int>>> GetMyWishlistBookIds(CancellationToken ct)
        {
            var result = await _wishlistService.GetMyWishlistBookIdsAsync(currentUserId, ct);
            return ToActionResult(result);
        }
    }
}
