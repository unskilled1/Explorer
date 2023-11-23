using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

namespace Explorer.Extensions;

public static class VisualExtensions
{
    public static Control? GetChildControlWithFlyout(this Visual visual)
    {
        if (visual is not Control control)
            return null;

        var flyoutBase = FlyoutBase.GetAttachedFlyout(control);
        if (flyoutBase is { })
            return control;

        var children = control.GetVisualChildren();
        foreach (var child in children)
        {
            if (child.GetChildControlWithFlyout() is { } res)
                return res;
        }

        return null;
    }

}
