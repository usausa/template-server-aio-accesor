namespace Template.Models.View;

using System.Diagnostics.CodeAnalysis;

using Template.Models.Entity;

public class ReportView : ItemEntity
{
    [AllowNull]
    public string CategoryName { get; set; }
}
