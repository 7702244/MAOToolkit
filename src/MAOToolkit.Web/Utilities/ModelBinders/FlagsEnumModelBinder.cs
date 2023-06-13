using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MAOToolkit.Utilities.ModelBinders
{
    public class FlagsEnumModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext is null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            // Only accept enum values.
            if (!bindingContext.ModelMetadata.IsFlagsEnum)
                return Task.CompletedTask;

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            // Get the real enum type.
            var enumType = bindingContext.ModelType;
            enumType = Nullable.GetUnderlyingType(enumType) ?? enumType;

            // Each value self may contains a series of actual values, split it with comma.
            var strs = valueProviderResult.Values.SelectMany(s => s?.Split(',') ?? Array.Empty<string>());

            // Convert all items into enum items.
            var actualValues = strs.Select(valueString => Enum.Parse(enumType, valueString));

            // Merge to final result.
            var result = actualValues.Aggregate(0, (current, value) => current | Convert.ToInt32(value));

            // Convert to Enum object.
            var realResult = Enum.ToObject(enumType, result);

            // Result
            bindingContext.Result = ModelBindingResult.Success(realResult);
            return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     An <see cref="IModelBinderProvider" /> used to provider <see cref="FlagsEnumModelBinder" /> instances.
    /// </summary>
    public class FlagsEnumModelBinderProvider : IModelBinderProvider
    {
        /// <inheritdoc />
        /// <summary>
        ///     Creates a <see cref="IModelBinder" /> based on <see cref="ModelBinderProviderContext" />.
        /// </summary>
        /// <param name="context">The <see cref="ModelBinderProviderContext" />.</param>
        /// <returns>An <see cref="IModelBinder" />.</returns>
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.Metadata.IsFlagsEnum ? new FlagsEnumModelBinder() : null;
        }
    }

    public static class FlagsEnumModelBinderExtensions
    {
        public static void AddFlagsEnumProvider(this MvcOptions option)
        {
            option.ModelBinderProviders.Insert(0, new FlagsEnumModelBinderProvider());
        }
    }
}