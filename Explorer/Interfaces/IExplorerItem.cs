using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;

namespace Explorer.Interfaces;

public interface IExplorerItem : IDisposable, IEditableObject
{
    /// <summary>
    /// Item full path
    /// </summary>
    string Path { get; }

    /// <summary>
    /// Item name
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Item new name created by control
    /// </summary>
    string? NewName { get; }

    /// <summary>
    /// Item size
    /// </summary>
    long Size { get; }

    /// <summary>
    /// Preety view of size property
    /// </summary>
    string? StringSize { get; }

    /// <summary>
    /// Last modified date
    /// </summary>
    DateTimeOffset Modified { get; }

    /// <summary>
    /// Creation date
    /// </summary>
    DateTimeOffset Created { get; }

    
    /// <summary>
    /// Has children flag
    /// </summary>
    bool HasChildren { get; }

    /// <summary>
    /// Item is expanded
    /// </summary>
    bool IsExpanded { get; set; }

    /// <summary>
    /// Item is hidden
    /// </summary>
    bool IsHidden { get; }

    /// <summary>
    /// Item is directory
    /// </summary>
    bool IsDirectory { get; }

    /// <summary>
    /// Inner child items
    /// </summary>
    IReadOnlyList<IExplorerItem>? Children { get; }

    bool Cut();
    bool Copy();
    bool Paste();
    bool Delete();
    bool Rename(string? newName);
}
