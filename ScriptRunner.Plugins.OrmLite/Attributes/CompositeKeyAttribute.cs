using System;

namespace ScriptRunner.Plugins.OrmLite.Attributes;

/// <summary>
///     Specifies that a class has a composite key formed by multiple columns.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class CompositeKeyAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CompositeKeyAttribute" /> class.
    /// </summary>
    /// <param name="columns">The names of the columns that form the composite key.</param>
    /// <exception cref="ArgumentException">Thrown if no column names are specified.</exception>
    public CompositeKeyAttribute(params string[] columns)
    {
        if (columns == null || columns.Length == 0)
            throw new ArgumentException("At least one column must be specified for a composite key.", nameof(columns));

        Columns = columns;
    }

    /// <summary>
    ///     Gets the names of the columns that form the composite key.
    /// </summary>
    /// <value>
    ///     An array of column names that define the composite key.
    /// </value>
    public string[] Columns { get; }
}