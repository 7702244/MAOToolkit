using System.IO.Compression;

namespace MAOToolkit.Extensions;

public static class ZipArchiveExtensions
{
    /// <summary>
    /// Adds a file or directory to the archive.
    /// </summary>
    public static void CreateEntryFromAny(this ZipArchive archive, string sourcePath, string entryName = "", CompressionLevel compressionLevel = CompressionLevel.Optimal)
    {
        string fileName = Path.GetFileName(sourcePath);
        string entryFullName = Path.Combine(entryName, fileName);
            
        if (File.GetAttributes(sourcePath).HasFlag(FileAttributes.Directory))
        {
            archive.CreateEntryFromDirectory(sourcePath, entryFullName, compressionLevel);
        }
        else
        {
            archive.CreateEntryFromFile(sourcePath, entryFullName, compressionLevel);
        }
    }

    /// <summary>
    /// Adds a directory and its files to the archive.
    /// </summary>
    public static void CreateEntryFromDirectory(this ZipArchive archive, string sourceDirName, string entryName = "", CompressionLevel compressionLevel = CompressionLevel.Optimal)
    {
        var files = Directory.EnumerateFileSystemEntries(sourceDirName);
        foreach (string file in files)
        {
            archive.CreateEntryFromAny(file, entryName, compressionLevel);
        }
    }
}
