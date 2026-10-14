using LibraryApi.Common;
using LibraryApplication.Dtos.Wishlist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Contracts.BooksContracts
{
    public interface IWishlistService
    {
        Task<Result> AddAsync(string userId, int bookId, CancellationToken ct = default);
        Task<Result> RemoveAsync(string userId, int bookId, CancellationToken ct = default);
        Task<Result<IReadOnlyList<WishistItemDto>>> GetMyWishlistAsync(string userId, CancellationToken ct = default);
        Task<Result<bool>> IsInWishlistAsync(string userId, int bookId, CancellationToken ct = default);
        Task<Result<IReadOnlyList<int>>> GetMyWishlistBookIdsAsync(string userId, CancellationToken ct = default);
    }
}
