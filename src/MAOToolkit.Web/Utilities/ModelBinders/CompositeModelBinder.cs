using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MAOToolkit.Utilities.ModelBinders;

/// <summary>
/// A composite binder that combines several other binders and executes them sequentially for a single property.
/// </summary>
public class CompositeModelBinder : IModelBinder
{
    private readonly IModelBinder[] _binders;

    /// <summary>
    /// Creates a new instance that combines the passed binders.
    /// </summary>
    /// <param name="binders">A list of binders that need to be applied sequentially.</param>
    public CompositeModelBinder(params IModelBinder[] binders)
    {
        _binders = binders;
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        object? currentValue = null;
        bool isSet = false;

        foreach (var binder in _binders)
        {
            await binder.BindModelAsync(bindingContext);

            if (bindingContext.Result.IsModelSet)
            {
                currentValue = bindingContext.Result.Model;
                isSet = true;

                // Передаём результат следующему биндеру
                bindingContext.Model = currentValue;
            }
        }

        if (isSet)
            bindingContext.Result = ModelBindingResult.Success(currentValue);
    }
}

/// <summary>
/// A universal provider for combining multiple binders into a single pipeline.
/// Can be extended for different types.
/// </summary>
public class CompositeModelBinderProvider : IModelBinderProvider
{
    private readonly IReadOnlyList<IModelBinderProvider> _innerProviders;

    /// <summary>
    /// Creates a provider that combines the specified internal providers.
    /// </summary>
    public CompositeModelBinderProvider(params IModelBinderProvider[] innerProviders)
    {
        _innerProviders = innerProviders;
    }

    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        var binders = new List<IModelBinder>();

        foreach (var provider in _innerProviders)
        {
            var binder = provider.GetBinder(context);
            if (binder != null)
            {
                binders.Add(binder);
            }
        }

        // If none are suitable, we return null so that other providers can continue.
        if (binders.Count == 0)
            return null;

        return new CompositeModelBinder(binders.ToArray());
    }
}

public static class CompositeModelBinderExtensions
{
    public static void AddCompositeModelBinder(this MvcOptions option, params IModelBinderProvider[] providers)
    {
        option.ModelBinderProviders.Insert(0, new CompositeModelBinderProvider(providers));
    }
}