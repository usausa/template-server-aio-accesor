namespace Template.Web.Areas.Admin.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    using Template.Models.Entity;

    public class ItemListResponse
    {
        [Required]
        [AllowNull]
        public ItemEntity[] Entries { get; set; }
    }
}
