using LibraryApi.Common;
using LibraryApplication.Common;
using LibraryApplication.Contracts.BooksContracts;
using LibraryApplication.Dtos.Books.Authors;
using LibraryApplication.Services.BookServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers.Books
{

    public class AuthorsController : APIBaseController
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<AuthorDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResult<AuthorDto>>> GetAll([FromQuery] AuthorQueryParams queryParams , CancellationToken ct)
        {
            var result = await _authorService.GetAllAsync(queryParams ,ct);
            return ToActionResult(result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(AuthorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuthorDto>> GetById(int id, CancellationToken ct)
        {
            var result = await _authorService.GetByIdAsync(id, ct);
            return ToActionResult(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(AuthorDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthorDto>> Create([FromBody] CreateAuthorDto dto, CancellationToken ct)
        {
            var result = await _authorService.CreateAsync(dto, ct);

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
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAuthorDto dto, CancellationToken ct)
        {
            var result = await _authorService.UpdateAsync(id, dto, ct);
            return ToActionResult(result, "Book Updated successfully.");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _authorService.DeleteAuthorAsync(id, ct);
            return ToActionResult(result, "Book deleted successfully.");
        }
    }
}
