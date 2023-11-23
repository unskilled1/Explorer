using Avalonia;
using Avalonia.Controls;
using System;

namespace Explorer.Themes;

public sealed class ControlPreviewer : UserControl
{
    public static readonly StyledProperty<object?> KeyProperty = 
        AvaloniaProperty.Register<ControlPreviewer, object?>(nameof(Key));

    public object? Key
    {
        get => GetValue(KeyProperty);
        set => SetValue(KeyProperty, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (Name is null)
        {
            throw new NullReferenceException(nameof(Name));
        }

        var data = this.FindResource(Name);
        SetCurrentValue(KeyProperty, data);
        base.OnAttachedToVisualTree(e);
    }
}
