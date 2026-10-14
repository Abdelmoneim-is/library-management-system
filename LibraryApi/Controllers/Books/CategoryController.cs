using LibraryApi.Common;
using LibraryApplication.Common;
using LibraryApplication.Contracts.BooksContracts;
using LibraryApplication.Dtos.Books.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LibraryApi.Controllers.Books
{

    public class CategoryController : APIBaseController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<CategoryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResult<CategoryDto>>> GetAllCategories([FromQuery] CategoryQueryParams queryParams , CancellationToken ct)
        {
            var result = await _categoryService.GetAllCategoryAsync(queryParams ,ct);
            return ToActionResult(result);
        }


        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(int id , CancellationToken ct)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id, ct);
            return ToActionResult(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CategoryDto>> Create ([FromBody] CreateCategoryDto dto , CancellationToken ct)
        {
            var result = await _categoryService.CreateCategoryAsync(dto, ct);
            if (!result.IsSucess) return ToProblem(result.Errors);

            return CreatedAtAction(nameof(GetCategoryById), new { id = result.data.Id }, result.data);




        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update (int id ,[FromBody] UpdateCategoryDto dto , CancellationToken ct)
        {
            var result = await _categoryService.UpdateCategoryAsync(id , dto, ct);
            return ToActionResult(result, "Book Updated successfully.");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Errors), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _categoryService.DeleteCategoryAsync(id , ct);
            return ToActionResult(result, "Book deleted successfully.");
        }




    }
}
