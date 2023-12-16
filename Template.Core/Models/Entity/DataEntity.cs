namespace Template.Models.Entity;

public sealed class DataEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public bool Flag { get; set; }

    public DateTime DateTime { get; set; }
}
