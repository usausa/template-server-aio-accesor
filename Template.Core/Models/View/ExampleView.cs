namespace Template.Models.View
{
    using System.Diagnostics.CodeAnalysis;

    public class TemplateView
    {
        public int No { get; set; }

        [AllowNull]
        public string Name { get; set; }
    }
}
