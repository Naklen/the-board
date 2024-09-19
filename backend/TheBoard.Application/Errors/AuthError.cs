namespace TheBoard.Application.Errors;

public class AuthError(
    string message,
    string code,
    ErrorType type) : BaseError(message, code, type) 
{
    public static AuthError InvalidRefreshToken()
    {
        var error = new AuthError("Invalid refresh token", "invalidRefreshToken", ErrorType.Auth);        
        return error;
    }

    public static AuthError LoginToNotExistedUser(string field, string value)
    {
        var error = new AuthError($"No users with this {field}", "notExist", ErrorType.Auth);
        error.WithMetadata("field", field)
            .WithMetadata("attemptedValue", value);
        return error;
    }

    public static AuthError WrongPassword()
    {
        var error = new AuthError("Wrong password", "wrongPassword", ErrorType.Auth);        
        return error;
    }

    public static AuthError SessionIsNotExist()
    {
        var error = new AuthError("Session is not exist", "sessionIsNotExist", ErrorType.Auth);
        return error;
    }    
}