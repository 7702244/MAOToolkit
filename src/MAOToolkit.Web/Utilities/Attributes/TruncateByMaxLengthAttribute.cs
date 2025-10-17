using System.ComponentModel.DataAnnotations;

namespace MAOToolkit.Utilities.Attributes;

/// <summary>
/// An attribute indicating that the value of the string
/// should be automatically truncated to the length
/// specified by the <see cref="MaxLengthAttribute"/> attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class TruncateByMaxLengthAttribute : Attribute;