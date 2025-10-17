using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MAOToolkit.Utilities.ModelBinders;

public class DecimalModelBinder : IModelBinder
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

public class DecimalInvariantProvider : IModelBinderProvider
{
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
    public static void AddDecimalInvariantProvider(this MvcOptions option)
    {
        option.ModelBinderProviders.Insert(0, new DecimalInvariantProvider());
    }
}