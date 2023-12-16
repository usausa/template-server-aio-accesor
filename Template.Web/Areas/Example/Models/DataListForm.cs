namespace Template.Web.Areas.Example.Models;

public sealed class DataListForm
{
    [Range(1, Int32.MaxValue)]
    public int? Page { get; set; }

    public string? Name { get; set; }

    public DateTime? DateTimeFrom { get; set; }

    public DateTime? DateTimeTo { get; set; }
}
