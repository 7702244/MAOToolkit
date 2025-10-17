using System.ComponentModel.DataAnnotations;
using System.Reflection;
using MAOToolkit.Utilities.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MAOToolkit.Utilities.ModelBinders;

public class NormalizeStringModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        string? value = valueProviderResult.FirstValue;

        if (String.IsNullOrEmpty(value))
        {
            return Task.CompletedTask;
        }
        
        value = value.Trim();

        var metadata = bindingContext.ModelMetadata;
        if (!String.IsNullOrEmpty(metadata.PropertyName))
        {
            var propInfo = metadata.ContainerType?.GetProperty(metadata.PropertyName);
            if (propInfo?.IsDefined(typeof(TruncateByMaxLengthAttribute), false) == true)
            {
                var maxLengthAttr = propInfo.GetCustomAttribute<MaxLengthAttribute>();
                if (maxLengthAttr != null && value.Length > maxLengthAttr.Length)
                {
                    value = value.Substring(0, maxLengthAttr.Length);
                }
            }
        }

        bindingContext.Result = ModelBindingResult.Success(value);
        return Task.CompletedTask;
    }
}

public class NormalizeStringModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Metadata.UnderlyingOrModelType == typeof(string))
        {
            return new NormalizeStringModelBinder();
        }

        return null;
    }
}

public static class NormalizeStringModelBinderExtensions
{
    public static void AddNormalizeStringProvider(this MvcOptions option)
    {
        option.ModelBinderProviders.Insert(0, new NormalizeStringModelBinderProvider());
    }
}