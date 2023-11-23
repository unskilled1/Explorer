using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Explorer.Behaviours;

public class FocusOnLoadedBehavior : Behavior<Control>
{
    protected override void OnAttached()
    {
        AssociatedObject!.Loaded += OnLoaded;
        base.OnAttached();
    }

    protected override void OnDetaching()
    {
        AssociatedObject!.Loaded -= OnLoaded;
        base.OnDetaching();
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        AssociatedObject!.Focus(Avalonia.Input.NavigationMethod.Tab);
    }
}
