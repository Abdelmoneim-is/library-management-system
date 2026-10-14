using AutoMapper;
using LibraryApi.Common;
using LibraryApplication.Contracts.BooksContracts;
using LibraryApplication.Dtos.Wishlist;
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
    public class WishlistService : IWishlistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WishlistService(IUnitOfWork unitOfWork , IMapper mapper )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result> AddAsync(string userId, int bookId, CancellationToken ct = default)
        {
            var bookRepo = _unitOfWork.GetGenericRepository<Books, int>();
            var wishlistRepo = _unitOfWork.GetGenericRepository<WishlistItem, int>();

            var book = await bookRepo.GetByIdAsync(bookId , ct);

            if (book is null)
                return Result.Fail(Errors.NotFound("Book.NotFound", $"Book With Id {bookId} Is Not Found"));

            var spec = new WishlistItemSpecification(userId, bookId );
            var existingCount = await wishlistRepo.CountAsync(spec, ct);
            if (existingCount > 0)
                return Result.Fail(Errors.Confilict("Wishlist.AlreadyExists", "This book is already in your wishlist."));

            wishlistRepo.Add(new WishlistItem
            {
                BookId = bookId,
                UserId = userId,
                AddedAt = DateTime.UtcNow
            });

            await _unitOfWork.SaveChanges(ct);
            return Result.Ok();
        }

        public async Task<Result<IReadOnlyList<WishistItemDto>>> GetMyWishlistAsync(string userId, CancellationToken ct = default)
        {
            var wishListRepo = _unitOfWork.GetGenericRepository<WishlistItem, int>();

            var spec = new WishlistForUserSpecification(userId);
            var wishList = await wishListRepo.GetAllAsync(spec , ct);

            var dto = _mapper.Map<IReadOnlyList< WishistItemDto>>(wishList);

            return Result<IReadOnlyList<WishistItemDto>>.Ok(dto);

        }

        public async Task<Result<IReadOnlyList<int>>> GetMyWishlistBookIdsAsync(string userId, CancellationToken ct = default)
        {
            var wishlistRepo = _unitOfWork.GetGenericRepository<WishlistItem, int>();

            var items = await wishlistRepo.GetAllAsync(new WishlistItemsByUserSpecification(userId), ct);
            IReadOnlyList<int> ids = items.Select(i => i.BookId).ToList();

            return Result<IReadOnlyList<int>>.Ok(ids);
        }

        public async Task<Result<bool>> IsInWishlistAsync(string userId, int bookId, CancellationToken ct = default)
        {
            var wishListRepo = _unitOfWork.GetGenericRepository<WishlistItem, int>();

            var spec = new WishlistItemSpecification(userId, bookId);

            var count = await wishListRepo.CountAsync(spec , ct);

            return Result<bool>.Ok(count > 0);

        }

        public async Task<Result> RemoveAsync(string userId, int bookId, CancellationToken ct = default)
        {
            var wishlistRepo = _unitOfWork.GetGenericRepository<WishlistItem, int>();

            var spec = new WishlistItemSpecification(userId, bookId );
            var item = await wishlistRepo.GetByIdAsync(spec, ct);
            if (item is null)
                return Result.Fail(Errors.NotFound("Wishlist.NotFound", "This book is not in your wishlist."));

            wishlistRepo.Delete(item);
            await _unitOfWork.SaveChanges(ct);
            return Result.Ok();
        }
    }
}
