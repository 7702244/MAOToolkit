using System.ComponentModel.DataAnnotations;
using System.Reflection;
using MAOToolkit.Utilities.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MAOToolkit.Utilities.ModelBinders;

/// <summary>
/// A model binder that automatically truncates a string to the length
/// specified in the <see cref="MaxLengthAttribute"/> attribute,
/// if the property is marked with the <see cref="TruncateByMaxLengthAttribute"/> attribute.
/// </summary>
public class TruncatingStringModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult == ValueProviderResult.None)
            return Task.CompletedTask;

        string? value = valueProviderResult.FirstValue;
        if (String.IsNullOrEmpty(value))
        {
            return Task.CompletedTask;
        }

        var metadata = bindingContext.ModelMetadata;
        if (String.IsNullOrEmpty(metadata.PropertyName))
        {
            return Task.CompletedTask;
        }
        
        var propInfo = metadata.ContainerType?.GetProperty(metadata.PropertyName);
        if (propInfo?.IsDefined(typeof(TruncateByMaxLengthAttribute), false) == true)
        {
            var maxLengthAttr = propInfo.GetCustomAttribute<MaxLengthAttribute>();
            if (maxLengthAttr != null && value.Length > maxLengthAttr.Length)
            {
                value = value.Substring(0, maxLengthAttr.Length);
            }
        }

        bindingContext.Result = ModelBindingResult.Success(value);
        return Task.CompletedTask;
    }
}

/// <summary>
/// Provider for binding strings with truncation by MaxLength.
/// Only works if the property has the attribute <see cref="TruncateByMaxLengthAttribute"/>.
/// </summary>
public class TruncatingStringModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.UnderlyingOrModelType == typeof(string)
            && !String.IsNullOrEmpty(context.Metadata.PropertyName))
        {
            var propInfo = context.Metadata.ContainerType?.GetProperty(context.Metadata.PropertyName);
            if (propInfo?.IsDefined(typeof(TruncateByMaxLengthAttribute), false) == true)
            {
                return new TruncatingStringModelBinder();
            }
        }

        return null;
    }
}

public static class TruncatingStringModelBinderExtensions
{
    public static void AddTruncatingStringProvider(this MvcOptions option)
    {
        option.ModelBinderProviders.Insert(0, new TruncatingStringModelBinderProvider());
    }
}