using LibraryApi.Common;
using LibraryApplication.Common;
using LibraryApplication.Contracts.BooksContracts;
using LibraryApplication.Dtos.Books.BooksDto;
using LibraryApplication.Services.BookServices;
using LibraryDomain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers.Books
{
 
    public class BooksController : APIBaseController
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<BookDto>), StatusCodes.Status200OK)]
        public async Task <ActionResult<PaginatedResult<BookDto>>> GetAll([FromQuery] BooksQueryParams queryParams , CancellationToken ct)
        {
            var result = await _bookService.GetAllAsync(queryParams ,ct);
            return ToActionResult(result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDto>> GetById (int id , CancellationToken ct)
        {
            var result = await _bookService.GetByIdAsync(id , ct);
            return ToActionResult(result);
        }

        [Authorize(Roles = "Admin") ]
        [HttpPost]
        [ProducesResponseType(typeof(BookDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult<BookDto>> Create([FromBody] CreateBookDto dto , CancellationToken ct)
        {
            var result = await _bookService.CreateAsync(dto , ct);
            if (!result.IsSucess) return ToProblem(result.Errors);
            return CreatedAtAction(nameof(GetById), new { id = result.data.Id }, result.data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update (int id , UpdateBookDto dto , CancellationToken ct)
        {
            var result = await _bookService.UpdateAsync(id, dto, ct);
            return ToActionResult(result , "Book updated successfully.");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task <IActionResult> Delete (int id , CancellationToken ct)
        {
            var result = await _bookService.DeleteBookAsync(id, ct);
            return ToActionResult(result , "Book deleted successfully.");
        }
    }
}
