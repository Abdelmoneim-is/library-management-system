using AutoMapper;
using LibraryApi.Common;
using LibraryApplication.Contracts.BooksContracts;
using LibraryApplication.Dtos.Reviews;
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
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork , IIdentityService identityService , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _mapper = mapper;
        }

        public async Task<Result< ReviewDto>> AddReviewAsync(string userId, int bookId, CreateReviewDto dto, CancellationToken ct = default)
        {
            var bookRepo = _unitOfWork.GetGenericRepository<Books, int>();
            var borrowingRepo = _unitOfWork.GetGenericRepository<Borrowing, int>();
            var reviewRepo = _unitOfWork.GetGenericRepository<BookReview, int>();
            
            var book = await bookRepo.GetByIdAsync(bookId , ct);
            if (book == null)
                return Result<ReviewDto>.Fail(Errors.NotFound("Book.Not Found", "The requested book was not found."));



            var borrowedCount = await borrowingRepo.CountAsync(new BorrowingWithBookSpecification(userId, bookId), ct);
            if (borrowedCount == 0)
                return Result<ReviewDto>.Fail(Errors.Forbidden("Review.NotAllowed", "You can only review books that you have previously borrowed."));

            var existingCount = await reviewRepo.CountAsync(new UserReviewForBookSpecification(userId, bookId), ct);
            if (existingCount > 0)
                return Result<ReviewDto>.Fail(Errors.Confilict("Review.AlreadyExists", "You have already submitted a review for this book."));

            var review = new BookReview()
            {
                BookId = bookId,
                UserId = userId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

             reviewRepo.Add(review);
            await _unitOfWork.SaveChanges(ct);

            var reviewDto = _mapper.Map<ReviewDto>(review);

            var userInfoMap = await _identityService.GetUsersInfoByIdsAsync(new[] { userId }, ct);
            if (userInfoMap.TryGetValue(userId, out var userInfo))
            {
                reviewDto.UserName = userInfo.UserName;
                reviewDto.DisplayName = userInfo.DisplayName;
            }
            return Result<ReviewDto>.Ok(reviewDto);


        }

        public async Task<Result<bool>> DeleteReviewAsync(string userId, bool isAdmin, int reviewId, CancellationToken ct = default)
        {
            var reviewRepo = _unitOfWork.GetGenericRepository<BookReview, int>();
            var userReview = await reviewRepo.GetByIdAsync(reviewId, ct);
            if (userReview == null)
                return Result<bool>.Fail(Errors.NotFound("Review.NotFound", "Review was not found."));
            if(userReview.UserId != userId && !isAdmin)
                return Result<bool>.Fail(Errors.Forbidden("Review.Forbidden", "You are not authorized to delete this review."));

            reviewRepo.Delete(userReview);
            await _unitOfWork.SaveChanges(ct);
            return Result<bool>.Ok(true);
        }

        public async Task<Result<BookRatingSummaryDto>> GetBookRatingSummaryAsync(int bookId, CancellationToken ct = default)
        {
            var bookRepo = _unitOfWork.GetGenericRepository<Books, int>();
            var book = await bookRepo.GetByIdAsync(bookId, ct);
            if (book == null)
            {
                return Result<BookRatingSummaryDto>.Fail(Errors.NotFound("Book.Not Found", "The requested book was not found."));
            }

            var reviewRepo = _unitOfWork.GetGenericRepository<BookReview, int>();
            var spec = new ReviewsByBookIdSpecification(bookId);
            var reviews = await reviewRepo.GetAllAsync(spec, ct);

            var summary = new BookRatingSummaryDto
            {
                ReviewsCount = reviews.Count,
                AverageRating = reviews.Count > 0 ? Math.Round(reviews.Average(r => r.Rating), 1) : 0
            };

            return Result<BookRatingSummaryDto>.Ok(summary);
        }

        public async Task<Result<IReadOnlyList<ReviewDto>>> GetBookReviewsAsync(int bookId, CancellationToken ct = default)
        {
            var reviewRepo = _unitOfWork.GetGenericRepository<BookReview, int>();
            var bookRepo = _unitOfWork.GetGenericRepository<Books, int>();

            var book = await bookRepo.GetByIdAsync(bookId, ct);
            if (book == null)
                return Result<IReadOnlyList<ReviewDto>>.Fail(Errors.NotFound("Book.NotFound", "The requested book was not found."));

            var reviewSpec = new ReviewsByBookIdSpecification(bookId);
            var review = await reviewRepo.GetAllAsync(reviewSpec, ct);
            var dto = _mapper.Map<IReadOnlyList<ReviewDto>>(review);
            if (!dto.Any())
                return Result<IReadOnlyList<ReviewDto>>.Ok(dto);

            var userIds = dto.Select(r => r.UserId);
            var userMap = await _identityService.GetUsersInfoByIdsAsync(userIds, ct);

            foreach (var dtos in dto)
            {
                if (userMap.TryGetValue(dtos.UserId, out var userInfo))
                {
                    dtos.UserName = userInfo.UserName;
                    dtos.DisplayName = userInfo.DisplayName;
                }
            }

            return Result<IReadOnlyList<ReviewDto>>.Ok(dto);
        }

        public async Task<Result<ReviewDto>> GetMyReviewForBookAsync(string userId, int bookId, CancellationToken ct = default)
        {
            var reviewRepo = _unitOfWork.GetGenericRepository<BookReview, int>();
            var review = await reviewRepo.GetByIdAsync(new UserReviewForBookSpecification(userId, bookId), ct);

            if (review is null)
                return Result<ReviewDto>.Fail(Errors.NotFound("Review.NotFound", "You have not reviewed this book yet."));

            var reviewDto = _mapper.Map<ReviewDto>(review);

            var userInfoMap = await _identityService.GetUsersInfoByIdsAsync(new[] { userId }, ct);
            if (userInfoMap.TryGetValue(userId, out var userInfo))
            {
                reviewDto.UserName = userInfo.UserName;
                reviewDto.DisplayName = userInfo.DisplayName;
            }

            return Result<ReviewDto>.Ok(reviewDto);
        }

        public async Task<Result<ReviewDto>> UpdateReviewAsync(string userId, int reviewId, UpdateReviewDto dto, CancellationToken ct = default)
        {
            var reviewRepo = _unitOfWork.GetGenericRepository<BookReview, int>();
            var review = await reviewRepo.GetByIdAsync(reviewId, ct);
            if(review == null)
                return Result<ReviewDto>.Fail(Errors.NotFound("Review.NotFound", "Review was not found."));

            if(review.UserId != userId)
                return Result<ReviewDto>.Fail(Errors.Forbidden("Review.Forbidden", "You are not authorized to update this review."));

            review.Rating = dto.Rating;
            review.Comment  = dto.Comment;
            review.UpdatedAt = DateTime.UtcNow;

           reviewRepo.Update(review );
            await _unitOfWork.SaveChanges(ct);

            var reviewDto = _mapper.Map<ReviewDto>(review);
            var userInfoMap = await _identityService.GetUsersInfoByIdsAsync(new[] { userId }, ct);
            if (userInfoMap.TryGetValue(userId, out var userInfo))
            {
                reviewDto.UserName = userInfo.UserName;
                reviewDto.DisplayName = userInfo.DisplayName;
            }

            return Result<ReviewDto>.Ok(reviewDto);


        }

       
    }
}
