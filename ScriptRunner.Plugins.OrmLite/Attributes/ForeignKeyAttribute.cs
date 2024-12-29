using System;

namespace ScriptRunner.Plugins.OrmLite.Attributes;

/// <summary>
///     Specifies that a property is a foreign key referencing another table and column.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ForeignKeyAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ForeignKeyAttribute" /> class.
    /// </summary>
    /// <param name="referencedTable">The name of the table being referenced.</param>
    /// <param name="referencedColumn">The name of the column being referenced in the table.</param>
    public ForeignKeyAttribute(string referencedTable, string referencedColumn)
    {
        ReferencedTable = referencedTable;
        ReferencedColumn = referencedColumn;
    }

    /// <summary>
    ///     Gets the name of the referenced table.
    /// </summary>
    public string ReferencedTable { get; }

    /// <summary>
    ///     Gets the name of the referenced column in the referenced table.
    /// </summary>
    public string ReferencedColumn { get; }

    /// <summary>
    ///     Gets or sets the action to take when the referenced row is deleted.
    ///     Valid SQL actions include <c>CASCADE</c>, <c>SET NULL</c>, <c>SET DEFAULT</c>, or <c>NO ACTION</c>.
    /// </summary>
    public string? OnDelete { get; set; }

    /// <summary>
    ///     Gets or sets the action to take when the referenced row is updated.
    ///     Valid SQL actions include <c>CASCADE</c>, <c>SET NULL</c>, <c>SET DEFAULT</c>, or <c>NO ACTION</c>.
    /// </summary>
    public string? OnUpdate { get; set; }
}