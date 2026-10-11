using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MAOToolkit.Utilities.ModelBinders;

/// <summary>
/// Binds date and time values using the current culture.
/// </summary>
public class DateTimeModelBinder : IModelBinder
{
    /// <summary>
    /// Binds the request value to the model.
    /// </summary>
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        string? value = valueProviderResult.FirstValue;

        if (String.IsNullOrWhiteSpace(value))
        {
            return Task.CompletedTask;
        }

        if (!DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime parsedValue))
        {
            // Error
            bindingContext.ModelState.TryAddModelError(
                bindingContext.ModelName,
                "Could not parse datetime.");
            return Task.CompletedTask;
        }

        bindingContext.Result = ModelBindingResult.Success(parsedValue);
        return Task.CompletedTask;
    }
}

/// <summary>
/// Provides the date and time model binder.
/// </summary>
public class DateTimeCultureProvider : IModelBinderProvider
{
    /// <summary>
    /// Returns a model binder when the model type is supported.
    /// </summary>
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Metadata.UnderlyingOrModelType == typeof(DateTime))
        {
            return new DateTimeModelBinder();
        }

        return null;
    }
}
public static class DateTimeModelBinderExtensions
{
    /// <summary>
    /// Registers the date and time model binder provider.
    /// </summary>
    public static void AddDateTimeCultureProvider(this MvcOptions option)
    {
        option.ModelBinderProviders.Insert(0, new DateTimeCultureProvider());
    }
}
