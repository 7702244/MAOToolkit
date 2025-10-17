using Microsoft.AspNetCore.Mvc;

namespace MAOToolkit.Extensions;

public static class ControllerExtensions
{
    public static ViewComponentResult ViewComponent<TComponent>(this Controller controller)
    {
        return controller.ViewComponent(typeof(TComponent));
    }

    public static ViewComponentResult ViewComponent<TComponent>(this Controller controller, object arguments)
    {
        return controller.ViewComponent(typeof(TComponent), arguments);
    }
}