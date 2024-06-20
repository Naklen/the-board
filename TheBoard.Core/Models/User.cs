namespace TheBoard.Core.Models;

public class User
{
    public Guid Id { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public List<Project> Projects { get; set; }
    public List<Ticket> Tickets { get; set; }
}