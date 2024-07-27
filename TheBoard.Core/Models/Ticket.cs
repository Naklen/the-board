namespace TheBoard.Core.Models;

public class Ticket
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public Project? Project { get; set; }
    public Section? Section { get; set; }
    public List<User> Executors { get; set; } = [];
}
