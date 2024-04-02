using System.IO.Compression;

namespace MAOToolkit.Extensions
{
    public static class ZipArchiveExtensions
    {
        public static void CreateEntryFromAny(this ZipArchive archive, string sourcePath, string entryName = "")
        {
            var fileName = Path.GetFileName(sourcePath);
            if (File.GetAttributes(sourcePath).HasFlag(FileAttributes.Directory))
            {
                archive.CreateEntryFromDirectory(sourcePath, Path.Combine(entryName, fileName));
            }
            else
            {
                // Causes an error for ReadOnly files.
                //archive.CreateEntryFromFile(sourcePath, Path.Combine(entryName, fileName), CompressionLevel.Optimal);

                var entry = archive.CreateEntry(Path.Combine(entryName, fileName), CompressionLevel.Optimal);
                entry.LastWriteTime = File.GetLastWriteTime(sourcePath);

                using (var fileStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var entryStream = entry.Open())
                {
                    fileStream.CopyTo(entryStream);
                }
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