namespace Template.Web.Areas.Api.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    public class ItemUpdateRequestEntry
    {
        [AllowNull]
        public string Code { get; set; }

        public int Value { get; set; }
    }

    public class ItemUpdateRequest
    {
        [Required]
        [AllowNull]
        public ItemUpdateRequestEntry[] Entries { get; set; }
    }
}
