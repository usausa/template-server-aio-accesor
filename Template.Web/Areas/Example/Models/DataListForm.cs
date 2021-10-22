namespace Template.Web.Areas.Example.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class DataListForm
    {
        [Range(1, Int32.MaxValue)]
        public int? Page { get; set; }

        public string? Name { get; set; }

        public DateTime? DateTimeFrom { get; set; }

        public DateTime? DateTimeTo { get; set; }
    }
}
