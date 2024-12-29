using System;

namespace ScriptRunner.Plugins.OrmLite.Attributes;

/// <summary>
///     Specifies that a property is a primary key for the associated table.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class PrimaryKeyAttribute : Attribute
{
    /// <summary>
    ///     Gets or sets a value indicating whether the primary key should auto-increment.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the primary key should auto-increment; otherwise, <c>false</c>.
    ///     Default value is <c>true</c>.
    /// </value>
    public bool AutoIncrement { get; set; } = true;
}