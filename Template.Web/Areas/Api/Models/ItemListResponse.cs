namespace Template.Web.Areas.Api.Models;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

using Template.Models.Entity;

public class ItemListResponse
{
    [Required]
    [AllowNull]
    public ItemEntity[] Entries { get; set; }
}
