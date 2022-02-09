namespace Template.Web.Areas.Api.Models;

using System.ComponentModel.DataAnnotations;

public class ItemListResponse
{
    [Required]
    [AllowNull]
    public ItemEntity[] Entries { get; set; }
}
