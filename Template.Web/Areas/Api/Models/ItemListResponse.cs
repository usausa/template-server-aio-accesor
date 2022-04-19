namespace Template.Web.Areas.Api.Models;

public class ItemListResponse
{
    [Required]
    public ItemEntity[] Entries { get; set; } = default!;
}
