using Microsoft.AspNetCore.Mvc;

namespace MAOToolkit.Extensions;

public static class ControllerExtensions
{
    /// <summary>
    /// Creates a result for the specified view component.
    /// </summary>
    public static ViewComponentResult ViewComponent<TComponent>(this Controller controller)
    {
        return controller.ViewComponent(typeof(TComponent));
    }

    /// <summary>
    /// Creates a result for the specified view component.
    /// </summary>
    public static ViewComponentResult ViewComponent<TComponent>(this Controller controller, object arguments)
    {
        return controller.ViewComponent(typeof(TComponent), arguments);
    }
}
