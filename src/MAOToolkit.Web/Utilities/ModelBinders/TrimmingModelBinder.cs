using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MAOToolkit.Utilities.ModelBinders;

public class TrimmingModelBinder : IModelBinder
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

        bindingContext.Result = ModelBindingResult.Success(value.Trim());
        return Task.CompletedTask;
    }
}

public class TrimmingModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Metadata.UnderlyingOrModelType == typeof(string))
        {
            return new TrimmingModelBinder();
        }

        return null;
    }
}

public static class TrimmingModelBinderExtensions
{
    public static void AddStringTrimmingProvider(this MvcOptions option)
    {
        option.ModelBinderProviders.Insert(0, new TrimmingModelBinderProvider());
    }
}