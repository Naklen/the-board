namespace TheBoard.Application.Errors;

public class BaseError : Error
{
    public BaseError(string message, string code, ErrorType type) : base(message)
    {
            WithMetadata("code", code).
            WithMetadata("type", type);
    }
}
