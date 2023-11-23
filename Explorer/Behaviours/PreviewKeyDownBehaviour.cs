using Avalonia;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using System;
using System.Windows.Input;

namespace Explorer.Behaviours;

public class PreviewKeyDownBehavior : Behavior<InputElement>
{
    public static readonly StyledProperty<bool> HandleEventProperty
        = AvaloniaProperty.Register<PreviewKeyDownBehavior, bool>(nameof(HandleEvent));

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<PreviewKeyDownBehavior, ICommand?>(nameof(Command));

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<PreviewKeyDownBehavior, object?>(nameof(CommandParameter));

    public static readonly StyledProperty<Key?> DownKeyProperty = 
        AvaloniaProperty.Register<PreviewKeyDownBehavior, Key?>(nameof(DownKey));

    public static readonly StyledProperty<Type?> TargetSourceTypeProperty =
        AvaloniaProperty.Register<PreviewKeyDownBehavior, Type?>(nameof(TargetSourceType));


    public bool HandleEvent
    {
        get => GetValue(HandleEventProperty);
        set => SetValue(HandleEventProperty, value);
    }

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public Key? DownKey
    {
        get => GetValue(DownKeyProperty);
        set => SetValue(DownKeyProperty, value);
    }

    public Type? TargetSourceType
    {
        get => GetValue(TargetSourceTypeProperty);
        set => SetValue(TargetSourceTypeProperty, value);
    }


    protected override void OnAttached()
    {
        AssociatedObject!.KeyDown += KeyDown;
        base.OnAttached();
    }

    protected override void OnDetaching()
    {
        AssociatedObject!.KeyDown -= KeyDown;
        base.OnDetaching();
    }

    private void KeyDown(object? sender, KeyEventArgs e)
    {
        if (DownKey.GetValueOrDefault(e.Key) != e.Key)
            return;

        if (IsSet(TargetSourceTypeProperty) 
            && e.Source is { } 
            && TargetSourceType != e.Source.GetType())
            return;

        e.Handled = HandleEvent;

        if (Command is null)
            return;

        object? resolvedParameter = IsSet(CommandParameterProperty)
            ? CommandParameter
            : default;

        Command.Execute(resolvedParameter);
    }
}
