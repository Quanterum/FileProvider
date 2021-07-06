using FileProvider.Interfaces;

namespace FileProvider.Models
{
    internal class FileContent
    {
        internal FileContent(IFileContents file)
        {
            FileName = file.FileName;
            Bytes = file.GetBytes();
        }

        internal string FileName { get; }

        internal byte[] Bytes { get; }
    }
}
