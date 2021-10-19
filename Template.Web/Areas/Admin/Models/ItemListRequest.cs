namespace Template.Web.Areas.Admin.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    public class ItemListRequest
    {
        [Required]
        [AllowNull]
        public string Category { get; set; }
    }
}
