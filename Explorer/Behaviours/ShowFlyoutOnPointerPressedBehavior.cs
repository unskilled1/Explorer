using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace Explorer.Behaviours;

public class ShowFlyoutOnPointerPressedBehavior : Behavior<Control>
{
    protected override void OnAttached()
    {
        AssociatedObject!.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, 
            Avalonia.Interactivity.RoutingStrategies.Tunnel);
        base.OnAttached();
    }

    protected override void OnDetaching()
    {
        AssociatedObject!.RemoveHandler(InputElement.PointerPressedEvent, OnPointerPressed);
        base.OnDetaching();
    }

    private void OnPointerPressed(object? sender, PointerEventArgs e)
    {
        var props = e.GetCurrentPoint(AssociatedObject!).Properties;
        if (!props.IsRightButtonPressed)
            return;

        FlyoutBase.ShowAttachedFlyout(AssociatedObject!);

        //In case of inner child flyout -uncomment this
        //var childWithFlyout = AssociatedObject!.GetChildControlWithFlyout();
        //    if (childWithFlyout is { })
        //        FlyoutBase.ShowAttachedFlyout(childWithFlyout);
        }
}
