using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using Explorer.Misc;
using Explorer.Misc.MessageBus;
using Explorer.ViewModels;
using System;
using System.Threading.Tasks;

namespace Explorer.Views;

public partial class ExplorerControl : ReactiveUserControl<ExplorerVM>
{
    public ExplorerControl()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        ViewModel!.MessageBus.Subscribe<RenameLastSelectedItem>(RenameLastSelectedItemCallback);
    }

    private Task RenameLastSelectedItemCallback(RenameLastSelectedItem call)
    {
        if (call.Explorer is not ExplorerType.Explorer)
            return Task.CompletedTask;

        var selectionArray = Tree.RowSelection!.SelectedIndex.ToArray();
        if (selectionArray is null || selectionArray.Length == 0)
            return Task.CompletedTask;

        var anchorArray = Tree.RowSelection!.AnchorIndex.ToArray();
        var lastIndex = anchorArray[^1] + 1;
        var cellTemplate = Tree.RowsPresenter!.TryGetElement(lastIndex)?.FindDescendantOfType<TreeDataGridTemplateCell>();

        if (cellTemplate is null)
            return Task.CompletedTask;

        var keyEvent = new KeyEventArgs
        {
            Source = this,
            Key = Key.F2,
            RoutedEvent = TreeDataGridTemplateCell.KeyDownEvent
        };

        cellTemplate.RaiseEvent(keyEvent);
        return Task.CompletedTask;
    }
}
