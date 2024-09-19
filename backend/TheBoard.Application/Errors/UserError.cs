namespace TheBoard.Application.Errors;

public class UserError(
    string message,
    string code,
    ErrorType type) : BaseError(message, code, type)
{
    public static UserError RegistrationWithExistedEmail(string email)
    {
        var error = new UserError("A user with same email already exists",
            "existed",
            ErrorType.Validation);
        error.WithMetadata("field", "email")
            .WithMetadata("attemptedValue", email);
        return error;
    }

    public static UserError RegistrationWithExistedUsername(string username)
    {
        var error = new UserError("A user with same username already exists",
            "existed",
            ErrorType.Validation);
        error.WithMetadata("field", "username")
            .WithMetadata("attemptedValue", username);
        return error;
    }

    public static UserError IsNotExist()
    {
        var error = new UserError("The user are not exist", "notExist", ErrorType.NotFound);
        return error;
    }
}
