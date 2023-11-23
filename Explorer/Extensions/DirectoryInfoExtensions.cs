using System.IO;

namespace Explorer.Extensions;

public static class DirectoryInfoExtensions
{
    /// <summary>
    /// Get size of directory
    /// </summary>
    /// <param name="directoryInfo"></param>
    /// <returns></returns>
    public static long Size(this DirectoryInfo directoryInfo)
    {
        long size = 0;
        var options = new EnumerationOptions { IgnoreInaccessible = true };

        foreach (var dir in directoryInfo.GetDirectories("*", options))
            size += dir.Size();

        foreach (var file in directoryInfo.GetFiles("*", options))
            size += file.Length;

        return size;
    }

}
