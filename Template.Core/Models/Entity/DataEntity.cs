namespace Template.Models.Entity;

using System;

public class DataEntity
{
    public int Id { get; set; }

    [AllowNull]
    public string Name { get; set; }

    public bool Flag { get; set; }

    public DateTime DateTime { get; set; }
}
