using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LibraryApi.Common
{
    public sealed  record Errors (string code , string description , ErrorType errorType)
    {
        public static Errors Failure(string code = "General.Failure", string description = "General Failure Has Occured") => new Errors(code, description, ErrorType.Failure);
        public static Errors Validation(string code = "General.Validation", string description = "General Validation Has Occured") => new Errors(code, description, ErrorType.Validation);
        public static Errors NotFound(string code = "General.NotFound", string description = "Resource Not Found") => new Errors(code, description, ErrorType.NotFound);
        public static Errors Confilict(string code = "General.Confilict", string description = "General Confilict Has Occured") => new Errors(code, description, ErrorType.Confilict);
        public static Errors Unauthorized(string code = "General.Unauthorized", string description = "Access Denied Due To Bad Request") => new Errors(code, description, ErrorType.Unauhorized);
        public static Errors Forbidden(string code = "General.Forbidden", string description = "This Operation Is Forbidden") => new Errors(code, description, ErrorType.Forbidden);
        public static Errors InvalidCredentials(string code = "General.InvalidCredentials", string description = "Provided Credentials Are Invalid") => new Errors(code, description, ErrorType.InvalidCredentials);
    }

    public enum ErrorType
    {
        Failure = 0,
        Validation = 1,
        NotFound = 2,
        Confilict = 3,
        Unauhorized = 4,
        Forbidden = 5,
        InvalidCredentials = 6
    }
}
