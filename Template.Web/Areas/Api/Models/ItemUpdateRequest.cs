namespace Template.Web.Areas.Api.Models;

public sealed class ItemUpdateRequestEntry
{
    public string Code { get; set; } = default!;

    public int Value { get; set; }
}

#pragma warning disable CA1819
public sealed class ItemUpdateRequest
{
    [Required]
    public ItemUpdateRequestEntry[] Entries { get; set; } = default!;
}
#pragma warning restore CA1819
