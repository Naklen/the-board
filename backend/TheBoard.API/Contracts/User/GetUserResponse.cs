namespace TheBoard.API.Contracts.User;

public record GetUserResponse(Guid Id, string Email, string Username);