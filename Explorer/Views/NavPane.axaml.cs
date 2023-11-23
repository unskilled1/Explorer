using Avalonia.ReactiveUI;
using Explorer.ViewModels;

namespace Explorer.Views;

public partial class QuickAccessControl : ReactiveUserControl<QAVM>
{
    public QuickAccessControl()
    {
        InitializeComponent();
    }
}
