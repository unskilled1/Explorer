using Explorer.Interfaces.MessageBus;

namespace Explorer.Misc.MessageBus;

record RenameLastSelectedItem(ExplorerType Explorer) : IMessage;

record CutSelectedItems(ExplorerType Explorer) : IMessage;

record CopySelectedItems(ExplorerType Explorer) : IMessage;

record PasteSavedItems(ExplorerType Explorer) : IMessage;

record DeleteSelectedItems(ExplorerType Explorer) : IMessage;

record CopySelectedItemPath(ExplorerType Explorer) : IMessage;

record CreateNewFolder(ExplorerType Explorer) : IMessage;

record ShowFolderProperties(ExplorerType Explorer) : IMessage;

record SelectAllItems(ExplorerType Explorer) : IMessage;

record RemoveSelection(ExplorerType Explorer) : IMessage;

record RevertSelection(ExplorerType Explorer) : IMessage;