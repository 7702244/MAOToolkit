using System.IO.Compression;

namespace MAOToolkit.Extensions
{
    public static class ZipArchiveExtensions
    {
        public static void CreateEntryFromAny(this ZipArchive archive, string sourcePath, string entryName = "")
        {
            string fileName = Path.GetFileName(sourcePath);
            string entryFullName = Path.Combine(entryName, fileName);
            
            if (File.GetAttributes(sourcePath).HasFlag(FileAttributes.Directory))
            {
                archive.CreateEntryFromDirectory(sourcePath, entryFullName);
            }
            else
            {
                archive.GetEntry(entryFullName)?.Delete();
                
                archive.CreateEntryFromFile(sourcePath, entryFullName, CompressionLevel.Optimal);
            }
        }

        public static void CreateEntryFromDirectory(this ZipArchive archive, string sourceDirName, string entryName = "")
        {
            var files = Directory.EnumerateFileSystemEntries(sourceDirName);
            foreach (string file in files)
            {
                archive.CreateEntryFromAny(file, entryName);
            }
        }
    }
}