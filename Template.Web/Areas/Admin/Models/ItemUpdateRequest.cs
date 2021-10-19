namespace Template.Web.Areas.Admin.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    public class DataUpdateRequestEntry
    {
        [AllowNull]
        public string Code { get; set; }

        public int Value { get; set; }
    }

    public class ItemUpdateRequest
    {
        [Required]
        [AllowNull]
        public DataUpdateRequestEntry[] Entries { get; set; }
    }
}
