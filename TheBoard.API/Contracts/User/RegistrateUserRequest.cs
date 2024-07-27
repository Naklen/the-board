using System.ComponentModel.DataAnnotations;

namespace TheBoard.API.Contracts.User;

public record RegistrateUserRequest(
    [Required]string UserName,
    [Required]string Email,
    [Required]string Password);

