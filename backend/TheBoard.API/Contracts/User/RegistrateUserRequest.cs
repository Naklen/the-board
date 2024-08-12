using System.ComponentModel.DataAnnotations;

namespace TheBoard.API.Contracts.User;

public record RegistrateUserRequest(
    [Required] string Username,
    [Required] string Email,
    [Required] string Password);