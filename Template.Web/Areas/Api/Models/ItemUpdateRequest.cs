namespace Template.Web.Areas.Api.Models;

public class ItemUpdateRequestEntry
{
    public string Code { get; set; } = default!;

    public int Value { get; set; }
}

#pragma warning disable CA1819
public class ItemUpdateRequest
{
    [Required]
    public ItemUpdateRequestEntry[] Entries { get; set; } = default!;
}
#pragma warning restore CA1819
