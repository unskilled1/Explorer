namespace Explorer.Extensions;

public static class LongExtensions
{
    private static readonly string[] _supportedSize = { "bytes", "Kb", "Mb", "Gb" };

    public static string FileSize(this long fileBytes)
    {
        int index = 0;

        while (fileBytes > 1024 && index < _supportedSize.Length)
        {
            fileBytes /= 1024;
            index++;
        }
        return $"{fileBytes} {_supportedSize[index]}";
    }
}
