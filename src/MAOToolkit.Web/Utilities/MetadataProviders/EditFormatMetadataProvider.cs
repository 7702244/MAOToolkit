using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace MAOToolkit.Utilities.MetadataProviders;

public class EditFormatMetadataProvider : IDisplayMetadataProvider
{
    public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
    {
        var editFormatAttribute = context.Attributes.OfType<EditFormatAttribute>().FirstOrDefault();
        if (editFormatAttribute is not null && !String.IsNullOrEmpty(editFormatAttribute.DataFormatString))
        {
            context.DisplayMetadata.EditFormatString = editFormatAttribute.DataFormatString;
        }
    }
}

/// <summary>
///     Allows overriding EditFormatString for a given field.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class EditFormatAttribute : Attribute
{
    /// <summary>
    ///     Gets or sets the format string
    /// </summary>
    public string? DataFormatString { get; set; }
}