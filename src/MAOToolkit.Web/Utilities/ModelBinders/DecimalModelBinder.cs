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

        if (!NormalizeSeparators(ref value)
            || !Decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedValue))
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

    private static bool NormalizeSeparators(ref string value)
    {
        int commaIndex = value.IndexOf(',');
        if (commaIndex < 0)
        {
            return true;
        }

        int dotIndex = value.IndexOf('.');
        if (dotIndex < 0)
        {
            // A single comma is the decimal separator. Multiple commas are ambiguous.
            if (commaIndex != value.LastIndexOf(','))
            {
                return false;
            }

            value = value.Replace(',', '.');
            return true;
        }

        // When both separators are present, dots are decimal separators and commas
        // are thousands separators. Commas after the decimal point are invalid.
        if (value.IndexOf(',', dotIndex) >= 0 || !HasValidThousandsGrouping(value.AsSpan(0, dotIndex)))
        {
            return false;
        }

        value = value.Replace(",", String.Empty);
        return true;
    }

    private static bool HasValidThousandsGrouping(ReadOnlySpan<char> integerPart)
    {
        int start = 0;
        while (start < integerPart.Length && !IsAsciiDigit(integerPart[start]))
        {
            start++;
        }

        int end = start;
        while (end < integerPart.Length && (IsAsciiDigit(integerPart[end]) || integerPart[end] == ','))
        {
            end++;
        }

        ReadOnlySpan<char> number = integerPart[start..end];
        int firstComma = number.IndexOf(',');
        if (firstComma < 0)
        {
            return true;
        }

        if (firstComma is < 1 or > 3)
        {
            return false;
        }

        int groupStart = 0;
        bool firstGroup = true;
        while (true)
        {
            int relativeCommaIndex = number[groupStart..].IndexOf(',');
            int groupEnd = relativeCommaIndex < 0 ? number.Length : groupStart + relativeCommaIndex;
            ReadOnlySpan<char> group = number[groupStart..groupEnd];
            int expectedLength = firstGroup ? firstComma : 3;
            if (group.Length != expectedLength)
            {
                return false;
            }

            foreach (char character in group)
            {
                if (!IsAsciiDigit(character))
                {
                    return false;
                }
            }

            if (relativeCommaIndex < 0)
            {
                break;
            }

            groupStart = groupEnd + 1;
            firstGroup = false;
        }

        return true;
    }

    private static bool IsAsciiDigit(char value) => value is >= '0' and <= '9';
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
