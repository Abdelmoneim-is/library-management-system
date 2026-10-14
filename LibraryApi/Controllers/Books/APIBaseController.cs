using LibraryApi.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers.Books
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIBaseController : ControllerBase
    {
        public static ActionResult<T> ToActionResult<T>(Result<T> result)
        {
            if (result.IsSucess)
            {
                return new OkObjectResult(result.data);
            }

            return ToProblem(result.Errors);
        }

        public static ActionResult ToActionResult(Result result, string? successMessage = null)
        {
            if (result.IsSucess)
            {
                if (!string.IsNullOrEmpty(successMessage))
                    return new OkObjectResult(new { Message = successMessage });

                return new NoContentResult();
            }

            return ToProblem(result.Errors);
        }

        public static ObjectResult ToProblem(IReadOnlyList<Errors> errors)
        {
            if (errors is null || !errors.Any())
            {
                return new ObjectResult(new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Server.Error",
                    Detail = "An unexpected error occurred."
                })
                { StatusCode = StatusCodes.Status500InternalServerError };
            }

            var firstError = errors[0];
            var statusCode = firstError.errorType switch
            {
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.Unauhorized => StatusCodes.Status401Unauthorized,
                ErrorType.Confilict => StatusCodes.Status409Conflict,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = firstError.code,
                Detail = firstError.description,
                Extensions = { ["Errors"] = errors }
            };

            return new ObjectResult(problem) { StatusCode = statusCode };
        }
    }
}
