using Avalonia;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using System;
using System.Windows.Input;

namespace Explorer.Behaviours;

public class PreviewDoubleTappedBehavior : Behavior<InputElement>
{
    public static readonly StyledProperty<bool> HandleEventProperty
        = AvaloniaProperty.Register<PreviewDoubleTappedBehavior, bool>(nameof(HandleEvent));

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<PreviewDoubleTappedBehavior, ICommand?>(nameof(Command));

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<PreviewDoubleTappedBehavior, object?>(nameof(CommandParameter));

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

    public Type? TargetSourceType
    {
        get => GetValue(TargetSourceTypeProperty);
        set => SetValue(TargetSourceTypeProperty, value);
    }


    protected override void OnAttached()
    {
        AssociatedObject!.DoubleTapped += DoubleTapped;
        base.OnAttached();
    }

    protected override void OnDetaching()
    {
        AssociatedObject!.DoubleTapped -= DoubleTapped;
        base.OnDetaching();
    }


    private void DoubleTapped(object? sender, TappedEventArgs e)
    {
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
