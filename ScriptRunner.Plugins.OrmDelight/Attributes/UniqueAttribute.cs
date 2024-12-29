using System;

namespace ScriptRunner.Plugins.OrmDelight.Attributes;

/// <summary>
///     Specifies that a property must have a unique value within its database context.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class UniqueAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UniqueAttribute" /> class with an optional error message.
    /// </summary>
    /// <param name="errorMessage">
    ///     The custom error message to use if validation fails. If not provided, defaults to "This
    ///     field must be unique."
    /// </param>
    public UniqueAttribute(string errorMessage = "This field must be unique.")
    {
        ErrorMessage = errorMessage;
    }

    /// <summary>
    ///     Gets or sets the custom error message if the uniqueness validation fails.
    /// </summary>
    /// <value>
    ///     The custom error message to display when validation fails. Defaults to "This field must be unique."
    /// </value>
    public string ErrorMessage { get; set; }
}