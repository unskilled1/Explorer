using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Explorer.Interfaces;
using Explorer.Misc;
using Explorer.Models;
using Explorer.Misc.MessageBus;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace Explorer.ViewModels;

public class BaseExplorerVM : ReactiveObject, IDisposable
{
    private List<BufferItemModel> _bufferItems = new();

    protected readonly Interfaces.MessageBus.IMessageBus _messageBus;

    protected string? _selectedPath;
    private bool _disposed;

    public string? SelectedPath
    {
        get => _selectedPath;
        set => this.RaiseAndSetIfChanged(ref _selectedPath, value);
    }

    public HierarchicalTreeDataGridSource<IExplorerItem> TreeSource { get; init; }

    public ReactiveCommand<Unit, Unit> ChangeSelectedPathCommand { get; }
    public ReactiveCommand<Unit, bool> CutSelectedItemsCommand { get; }
    public ReactiveCommand<Unit, bool> CopySelectedItemsCommand { get; }
    public ReactiveCommand<Unit, bool> PasteSavedItemsCommand { get; }
    public ReactiveCommand<Unit, bool> DeleteSelectedItemsCommand { get; }
    public ReactiveCommand<Unit, Unit> RenameSelectedItemCommand { get; }


    public BaseExplorerVM(string? selectedPath, Interfaces.MessageBus.IMessageBus messageBus)
    {
        _selectedPath = selectedPath;
        _messageBus = messageBus;

        ChangeSelectedPathCommand = ReactiveCommand.Create(ChangeSelectedPath);
        CutSelectedItemsCommand = ReactiveCommand.Create(CutSelectedItems);
        CopySelectedItemsCommand = ReactiveCommand.Create(CopySelectedItems);
        PasteSavedItemsCommand = ReactiveCommand.Create(PasteSavedItems);
        DeleteSelectedItemsCommand = ReactiveCommand.Create(DeleteSelectedItems);
        RenameSelectedItemCommand = ReactiveCommand.CreateFromTask(
            x => _messageBus.SendAsync(new RenameLastSelectedItem(ExplorerType.Explorer)));
    }

    private void ChangeSelectedPath()
    {
        var lastSelectedItem = TreeSource.RowSelection!.SelectedItems.LastOrDefault();
        if (lastSelectedItem is null)
            return;

        SelectedPath = lastSelectedItem.Path;
    }

    protected bool CutSelectedItems()
    {
        var selectedItems = TreeSource.RowSelection!.SelectedItems!.Where(x => !TreeSource.Items.Contains(x));
        if (!selectedItems.Any())
            return false;

        _bufferItems.Clear();
        foreach (var item in selectedItems)
        {
            var bufferModel = new BufferItemModel(Path: item!.Path,
                                                  IsFolder: item.IsDirectory,
                                                  IsCopy: false);
            _bufferItems.Add(bufferModel);
        }
        
        return true;
    }

    protected bool CopySelectedItems()
    {
        var selectedItems = TreeSource.RowSelection!.SelectedItems!.Where(x => !TreeSource.Items.Contains(x));
        if (!selectedItems.Any())
            return false;

        _bufferItems.Clear();
        foreach (var item in selectedItems)
        {
            var bufferModel = new BufferItemModel(Path: item!.Path,
                                                  IsFolder: item.IsDirectory,
                                                  IsCopy: true);
            _bufferItems.Add(bufferModel);
        }
        return true;
    }

    protected bool PasteSavedItems()
    {
        if (!_bufferItems.Any() || string.IsNullOrEmpty(SelectedPath))
            return false;

        var selectedItem = TreeSource.RowSelection!.SelectedItems!.Where(x => !TreeSource.Items.Contains(x)).FirstOrDefault();
        var directory = selectedItem?.Path ?? SelectedPath;
        foreach (var item in _bufferItems)
        {
            var path = item.Path;
            var itemExists = item.IsFolder 
                ? Directory.Exists(path) 
                : File.Exists(path);

            if (!itemExists)
                continue;

            try
            {
                if (item.IsFolder)
                {
                    var newPath = $"{directory}\\{Path.GetFileNameWithoutExtension(path)!}";
                    Directory.CreateDirectory(newPath);
                    RecursiveCopyFiles(path, newPath);

                    if (!item.IsCopy)
                        Directory.Delete(path, true);
                }
                else
                {
                    var newPath = $"{directory}\\{Path.GetFileName(path)}";
                    File.Copy(path, newPath, false);

                    if (!item.IsCopy)
                        File.Delete(path);
                }
            }
            catch (Exception exception)
            {
                // ignored
            }
        }

        return true;
    }

    private void RecursiveCopyFiles(string sourcePath, string targetPath)
    {
        var options = new EnumerationOptions
        {
            IgnoreInaccessible = true,
            AttributesToSkip = FileAttributes.Hidden | FileAttributes.System | FileAttributes.Temporary
        };

        foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", options))
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));

        foreach (var newPath in Directory.GetFiles(sourcePath, "*", options))
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), false);
    }

    protected bool DeleteSelectedItems()
    {
        var selectedItems = TreeSource.RowSelection!.SelectedItems!.Where(x => !TreeSource.Items.Contains(x));
        if (!selectedItems.Any())
            return false;

        foreach (var item in selectedItems)
            item!.Delete();

        return true;
    }

    protected async Task<bool> CopySelectedItemPathAsync()
    {
        var lastSelectedItem = TreeSource.RowSelection!.SelectedItems.LastOrDefault();
        if (lastSelectedItem is null)
            return false;

        var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (mainWindow is null)
            return false;

        try
        {
            await mainWindow.Clipboard!.SetTextAsync(lastSelectedItem.Path);
        }
        catch { return false; }

        return true;
    }

    private int Nfolder = 1;
    protected async Task<bool> CreateNewFolderAsync()
    {
        if (string.IsNullOrEmpty(SelectedPath))
            return false;

        DirectoryInfo? newDirectory = null;
        var newPath = $"{SelectedPath}\\New folder";
        try
        {
            if (!Directory.Exists(newPath)) { newDirectory = Directory.CreateDirectory(newPath); }
            else
            {
                newDirectory = Directory.CreateDirectory(newPath + " (" + Nfolder + ")");
                
                Nfolder++;
            }
        }
        catch { }

        if (newDirectory is null)
            return false;
        await Task.Delay(200);
        RemoveSelection();
       
        IExplorerItem? newItemModel = null;
        for (int i = 1; i < TreeSource.Rows.Count; i++)
        {
            if (TreeSource.Rows[i].Model is IExplorerItem item && item.Path == newPath)
            {
                newItemModel = item;
                TreeSource.RowSelection!.Select(new IndexPath(0, i - 1));
                break;
            }
        }

        if (newItemModel is null)
            return false;

        await _messageBus.SendAsync(new RenameLastSelectedItem(ExplorerType.Explorer));

        return true;
    }

    protected bool ShowFolderProperties()
    {
        throw new NotImplementedException();
    }

    protected bool SelectAllItems()
    {
        if (TreeSource.Rows.Count == 0)
            return false;

        for (int i = 1; i < TreeSource.Rows.Count; i++)
            TreeSource.RowSelection!.Select(new IndexPath(0, i - 1));

        return true;
    }

    protected bool RemoveSelection()
    {
        if (TreeSource.Rows.Count == 0)
            return false;

        for (int i = 1; i < TreeSource.Rows.Count; i++)
            TreeSource.RowSelection!.Deselect(new IndexPath(0, i - 1));

        return true;
    }

    protected bool RevertSelection()
    {
        if (TreeSource.Rows.Count == 0)
            return false;

        for (int i = 1; i < TreeSource.Rows.Count; i++)
        {
            var indexPath = new IndexPath(0, i - 1);
            if (TreeSource.RowSelection!.IsSelected(indexPath))
                TreeSource.RowSelection!.Deselect(indexPath);
            else
                TreeSource.RowSelection!.Select(indexPath);
        }

        return true;
    }

    public virtual void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;

        TreeSource!.Dispose();
    }
}
