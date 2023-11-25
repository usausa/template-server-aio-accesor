namespace Template.Web.Areas.Api.Models;

#pragma warning disable CA1819
public class ItemListResponse
{
    [Required]
    public ItemEntity[] Entries { get; set; } = default!;
}
#pragma warning restore CA1819
