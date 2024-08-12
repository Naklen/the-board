namespace TheBoard.Core.Models;

public class User
{
    public Guid Id { get; set; }    
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public List<Project> Projects { get; set; } = [];
    public List<Ticket> Tickets { get; set; } = [];
}