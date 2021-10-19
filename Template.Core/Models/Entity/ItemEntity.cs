namespace Template.Models.Entity
{
    using System.Diagnostics.CodeAnalysis;

    public class ItemEntity
    {
        [AllowNull]
        public string Code { get; set; }

        [AllowNull]
        public string Category { get; set; }

        [AllowNull]
        public string Name { get; set; }

        public int Value { get; set; }
    }
}
