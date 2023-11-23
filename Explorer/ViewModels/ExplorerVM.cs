using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Explorer.Interfaces;
using Explorer.Misc;
using Explorer.Misc.MessageBus;
using Explorer.Models;
using ReactiveUI;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Explorer.ViewModels;

public class ExplorerVM : BaseExplorerVM
{
    private readonly IDisposable _selectedPathSub;

    private IExplorerItem? _root;
    private bool _disposed;

    public Interfaces.MessageBus.IMessageBus MessageBus => _messageBus;

    public ExplorerVM(string? selectedPath, Interfaces.MessageBus.IMessageBus messageBus) 
        : base(selectedPath, messageBus)
    {
        TreeSource = new HierarchicalTreeDataGridSource<IExplorerItem>(Array.Empty<IExplorerItem>())
        {
            Columns =
            {
                new HierarchicalExpanderColumn<IExplorerItem>(
                    new TemplateColumn<IExplorerItem>(
                        header: "Имя",
                        cellTemplateResourceKey: "ItemNameCell",
                        cellEditingTemplateResourceKey: "ItemNameEditCell",
                        width: GridLength.Star,
                        options: new TemplateColumnOptions<IExplorerItem>()
                        {
                            BeginEditGestures = BeginEditGestures.F2,
                            CompareAscending = Comparisons.SortAscending(x => x.Name),
                            CompareDescending = Comparisons.SortDescending(x => x.Name),
                            IsTextSearchEnabled = true,
                            TextSearchValueSelector = x => x.Name
                        }),
                    childSelector: x => x.Children,
                    hasChildrenSelector: x => x.HasChildren && x.Path == _root!.Path,
                    isExpandedSelector: x => x.IsExpanded),

                new TextColumn<IExplorerItem, string>(
                    header: "Размер",
                    getter: x => x.StringSize,
                    options: new TextColumnOptions<IExplorerItem>()
                    {
                        CompareAscending = Comparisons.SortAscending(x => x.Size),
                        CompareDescending = Comparisons.SortDescending(x => x.Size),
                    }),

                new TextColumn<IExplorerItem, string>(
                    header: "Дата создания",
                    getter: x => x.Created.ToLocalTime().ToString("dd.MM.yyyy HH:mm.ss"),
                    options: new TextColumnOptions<IExplorerItem>()
                    {
                        CompareAscending = Comparisons.SortAscending(x => x.Created),
                        CompareDescending = Comparisons.SortDescending(x => x.Created),
                    }),

                new TextColumn<IExplorerItem, string>(
                    header: "Дата изменения",
                    getter: x => x.Modified.ToLocalTime().ToString("dd.MM.yyyy HH:mm.ss"),
                    options: new TextColumnOptions<IExplorerItem>()
                    {
                        CompareAscending = Comparisons.SortAscending(x => x.Modified),
                        CompareDescending = Comparisons.SortDescending(x => x.Modified),
                    })
            }
        };

        TreeSource.RowSelection!.SingleSelect = false;

        _selectedPathSub = this.WhenAnyValue(x => x.SelectedPath).Subscribe(SelectedPathHasChanged);

        _messageBus.Subscribe<CutSelectedItems>(CutSelectedItemsCallback);
        _messageBus.Subscribe<CopySelectedItems>(CopySelectedItemsCallback);
        _messageBus.Subscribe<PasteSavedItems>(PasteSavedItemsCallback);
        _messageBus.Subscribe<DeleteSelectedItems>(DeleteSelectedItemsCallback);
        _messageBus.Subscribe<CopySelectedItemPath>(CopySelectedItemPathCallback);
        _messageBus.Subscribe<CreateNewFolder>(CreateNewFolderCallback);
        _messageBus.Subscribe<ShowFolderProperties>(ShowFolderPropertiesCallback);
        _messageBus.Subscribe<SelectAllItems>(SelectAllItemsCallback);
        _messageBus.Subscribe<RemoveSelection>(RemoveSelectionCallback);
        _messageBus.Subscribe<RevertSelection>(RevertSelectionCallback);
    }

    private void SelectedPathHasChanged(string? selectedPath)
    {
        if (string.IsNullOrEmpty(selectedPath) || !Directory.Exists(selectedPath))
        {
            _selectedPath = _root?.Path;
            return;
        }

        _root?.Dispose();
        _root = new ExplorerItemModel(SelectedPath!, isDirectory: true, isRoot: true);
        TreeSource.Items = new[] { _root };
    }

    private Task CutSelectedItemsCallback(CutSelectedItems call)
    {
        if (call.Explorer is ExplorerType.Explorer)
            base.CutSelectedItems();

        return Task.CompletedTask;
    }

    private Task CopySelectedItemsCallback(CopySelectedItems call)
    {
        if (call.Explorer is ExplorerType.Explorer)
            base.CopySelectedItems();

        return Task.CompletedTask;
    }

    private Task PasteSavedItemsCallback(PasteSavedItems call)
    {
        if (call.Explorer is ExplorerType.Explorer)
            base.PasteSavedItems();

        return Task.CompletedTask;
    }

    private Task DeleteSelectedItemsCallback(DeleteSelectedItems call)
    {
        if (call.Explorer is ExplorerType.Explorer)
            base.DeleteSelectedItems();

        return Task.CompletedTask;
    }

    private Task CopySelectedItemPathCallback(CopySelectedItemPath call)
    {
        if (call.Explorer is ExplorerType.Explorer)
            return base.CopySelectedItemPathAsync();

        return Task.CompletedTask;
    }

    private Task CreateNewFolderCallback(CreateNewFolder call)
    {
        if (call.Explorer is ExplorerType.Explorer)
            return base.CreateNewFolderAsync();

        return Task.CompletedTask;
    }

    private Task ShowFolderPropertiesCallback(ShowFolderProperties call)
    {
        if (call.Explorer is ExplorerType.Explorer)
            base.ShowFolderProperties();

        return Task.CompletedTask;
    }

    private Task SelectAllItemsCallback(SelectAllItems call)
    {
        if (call.Explorer is ExplorerType.Explorer)
            base.SelectAllItems();

        return Task.CompletedTask;
    }

    private Task RemoveSelectionCallback(RemoveSelection call)
    {
        if (call.Explorer is ExplorerType.Explorer)
            base.RemoveSelection();

        return Task.CompletedTask;
    }

    private Task RevertSelectionCallback(RevertSelection call)
    {
        if (call.Explorer is ExplorerType.Explorer)
            base.RevertSelection();

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;

        base.Dispose();

        _selectedPathSub.Dispose();
        _root?.Dispose();
    }
}
