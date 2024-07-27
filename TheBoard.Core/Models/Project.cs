namespace TheBoard.Core.Models;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<User> Members { get; set; } = [];
    public List<Section> Sections { get; set; } = [];
    public List<Ticket> Tickets { get; set; } = [];
}
