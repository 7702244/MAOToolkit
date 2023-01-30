using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MAOToolkit.Utilities.ModelBinders
{
    public class DateTimeModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext is null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            var value = valueProviderResult.FirstValue;

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

    public class DateTimeCultureProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.UnderlyingOrModelType == typeof(DateTime))
            {
                return new DateTimeModelBinder();
            }

            return null;
        }
    }

    public static class DateTimeModelBinderExtensions
    {
        public static void AddDateTimeCultureProvider(this MvcOptions option)
        {
            option.ModelBinderProviders.Insert(0, new DateTimeCultureProvider());
        }
    }
}