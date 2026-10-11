using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MAOToolkit.Utilities.ModelBinders;

/// <summary>
/// Binds decimal values using invariant culture.
/// </summary>
public class DecimalModelBinder : IModelBinder
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

        // Remove unnecessary commas.
        value = value.Replace(",", String.Empty);

        if (!Decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedValue))
        {
            // Error
            bindingContext.ModelState.TryAddModelError(
                bindingContext.ModelName,
                "Could not parse decimal.");
            return Task.CompletedTask;
        }

        bindingContext.Result = ModelBindingResult.Success(parsedValue);
        return Task.CompletedTask;
    }
}

/// <summary>
/// Provides the invariant decimal model binder.
/// </summary>
public class DecimalInvariantProvider : IModelBinderProvider
{
    /// <summary>
    /// Returns a model binder when the model type is supported.
    /// </summary>
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Metadata.UnderlyingOrModelType == typeof(decimal))
        {
            return new DecimalModelBinder();
        }

        return null;
    }
}
public static class DecimalModelBinderExtensions
{
    /// <summary>
    /// Registers the invariant decimal model binder provider.
    /// </summary>
    public static void AddDecimalInvariantProvider(this MvcOptions option)
    {
        option.ModelBinderProviders.Insert(0, new DecimalInvariantProvider());
    }
}
