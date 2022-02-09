namespace Template.Web.Areas.Api.Models;

public class ItemListResponse
{
    [Required]
    [AllowNull]
    public ItemEntity[] Entries { get; set; }
}
