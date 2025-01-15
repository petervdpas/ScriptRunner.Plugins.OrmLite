using System;

namespace ScriptRunner.Plugins.OrmLite.Attributes;

/// <summary>
///     Specifies the table name for a database model.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class TableAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TableAttribute" /> class.
    /// </summary>
    /// <param name="name">The name of the database table.</param>
    public TableAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    ///     Gets the name of the database table.
    /// </summary>
    public string Name { get; }
}