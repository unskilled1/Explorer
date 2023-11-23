using Explorer.Misc;
using Explorer.Misc.MessageBus;
using ReactiveUI;
using System.Reactive;

namespace Explorer.ViewModels.TabBar;

public class IndexFeedVM : ReactiveObject
{
    private readonly Interfaces.MessageBus.IMessageBus _messageBus;

    public ReactiveCommand<Unit, Unit> CopySelectedItemsCommand { get; }
    public ReactiveCommand<Unit, Unit> PasteSavedItemsCommand { get; }
    public ReactiveCommand<Unit, Unit> CutSelectedItemsCommand { get; }
    public ReactiveCommand<Unit, Unit> CopySelectedItemPathCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteSelectedItemsCommand { get; }
    public ReactiveCommand<Unit, Unit> RenameSelectedItemCommand { get; }
    public ReactiveCommand<Unit, Unit> CreateNewFolderCommand { get; }
    public ReactiveCommand<Unit, Unit> ShowFolderPropertiesCommand { get; }
    public ReactiveCommand<Unit, Unit> SelectAllItemsCommand { get; }
    public ReactiveCommand<Unit, Unit> RemoveSelectionCommand { get; }
    public ReactiveCommand<Unit, Unit> RevertSelectionCommand { get; }


    public IndexFeedVM(Interfaces.MessageBus.IMessageBus messageBus)
    {
        _messageBus = messageBus;

        CopySelectedItemsCommand = ReactiveCommand.CreateFromTask(
            x => _messageBus.SendAsync(new CopySelectedItems(ExplorerType.Explorer)));
        PasteSavedItemsCommand = ReactiveCommand.CreateFromTask(
            x => _messageBus.SendAsync(new PasteSavedItems(ExplorerType.Explorer)));
        CutSelectedItemsCommand = ReactiveCommand.CreateFromTask(
            x => _messageBus.SendAsync(new CutSelectedItems(ExplorerType.Explorer)));
        CopySelectedItemPathCommand = ReactiveCommand.CreateFromTask(
            async x => await _messageBus.SendAsync(new CopySelectedItemPath(ExplorerType.Explorer)));
        DeleteSelectedItemsCommand = ReactiveCommand.CreateFromTask(
            x => _messageBus.SendAsync(new DeleteSelectedItems(ExplorerType.Explorer)));
        RenameSelectedItemCommand = ReactiveCommand.CreateFromTask(
            x => _messageBus.SendAsync(new RenameLastSelectedItem(ExplorerType.Explorer)));
        CreateNewFolderCommand = ReactiveCommand.CreateFromTask(
            x => _messageBus.SendAsync(new CreateNewFolder(ExplorerType.Explorer)));
        ShowFolderPropertiesCommand = ReactiveCommand.CreateFromTask(
            x => _messageBus.SendAsync(new ShowFolderProperties(ExplorerType.Explorer)));
        SelectAllItemsCommand = ReactiveCommand.CreateFromTask(
            x => _messageBus.SendAsync(new SelectAllItems(ExplorerType.Explorer)));
        RemoveSelectionCommand = ReactiveCommand.CreateFromTask(
            x => _messageBus.SendAsync(new RemoveSelection(ExplorerType.Explorer)));
        RevertSelectionCommand = ReactiveCommand.CreateFromTask(
            x => _messageBus.SendAsync(new RevertSelection(ExplorerType.Explorer)));
    }
}
