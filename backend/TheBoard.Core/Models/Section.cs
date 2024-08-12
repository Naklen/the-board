namespace TheBoard.Core.Models;

public class Section
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Project? Project { get; set; }
    public List<Ticket> Tickets { get; set; } = [];
}
