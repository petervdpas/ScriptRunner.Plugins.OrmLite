using System;

namespace ScriptRunner.Plugins.OrmLite.Attributes;

/// <summary>
///     Specifies that a property is required and must not be null or empty.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class RequiredAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="RequiredAttribute" /> class with an optional error message.
    /// </summary>
    /// <param name="errorMessage">
    ///     The custom error message to use if validation fails. If not provided, defaults to "This
    ///     field is required."
    /// </param>
    public RequiredAttribute(string errorMessage = "This field is required.")
    {
        ErrorMessage = errorMessage;
    }

    /// <summary>
    ///     Gets or sets the custom error message if the required field validation fails.
    /// </summary>
    /// <value>
    ///     The custom error message to display when validation fails. Defaults to "This field is required."
    /// </value>
    public string ErrorMessage { get; set; }
}