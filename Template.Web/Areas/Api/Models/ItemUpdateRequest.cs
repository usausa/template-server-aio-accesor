namespace Template.Web.Areas.Api.Models;

public class ItemUpdateRequestEntry
{
    public string Code { get; set; } = default!;

    public int Value { get; set; }
}

public class ItemUpdateRequest
{
    [Required]
    public ItemUpdateRequestEntry[] Entries { get; set; } = default!;
}
