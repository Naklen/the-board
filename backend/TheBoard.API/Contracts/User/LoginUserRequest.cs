using System.ComponentModel.DataAnnotations;

namespace TheBoard.API.Contracts.User;

public record LoginUserRequest(
    [Required] string Email,
    [Required] string Password);