using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Explorer.Converters;

public class ItemExplorerToPathConverter : IMultiValueConverter
{
    private readonly object? _openFolderRes;
    private readonly object? _folderRes;
    private readonly object? _fileRes;

    public ItemExplorerToPathConverter()
    {
        _openFolderRes = Application.Current!.FindResource("OpenFolderPath");
        _folderRes = Application.Current!.FindResource("FolderPath");
        _fileRes = Application.Current!.FindResource("FilePath");
    }

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count == 2 &&
                    values[0] is bool isDirectory &&
                    values[1] is bool isExpanded)
        {
            if (!isDirectory)
                return _fileRes;
            else
                return isExpanded ? _openFolderRes : _folderRes;
        }

        return null;
    }
}
