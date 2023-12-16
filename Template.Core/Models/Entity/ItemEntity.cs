namespace Template.Models.Entity;

public abstract class ItemEntity
{
    public string Code { get; set; } = default!;

    public string Category { get; set; } = default!;

    public string Name { get; set; } = default!;

    public int Value { get; set; }
}
