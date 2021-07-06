using System.IO;
using FileProvider.Options;

namespace FileProvider.Services
{
    internal static class DirectoryManager
    {
        internal static StorageDirectory GetStorageDirectory(this string fileHash, FileStorageOptions storageOptions)
        {
            var storagePath = storageOptions.NFSOptions.StoragePath;

            // Путь к хранилищу
            var actualDirectory = Path.Combine(storageOptions.StorageUserFilesPath,
                fileHash.Substring(0, 2), fileHash.Substring(2, 2));

            // Итоговая папка с проверкой на наличие
            var destDirectory = Path.Combine(storagePath, actualDirectory);
            if (!Directory.Exists(destDirectory)) Directory.CreateDirectory(destDirectory);

            // Относительный и полный пути к файлу
            var storageDirectory = new StorageDirectory
            {
                StoragePath = storagePath,
                RelativeFilePath = Path.Combine(actualDirectory, fileHash)
            };
            return storageDirectory;
        }

        internal static StorageDirectory GetCacheDirectory(string imageHash, string cacheFilesPath, int width,
            int height)
        {
            // Итоговая папка с проверкой на наличие
            var destDirectory = Path.Combine(cacheFilesPath, imageHash);
            if (!Directory.Exists(destDirectory))
                Directory.CreateDirectory(destDirectory);

            // Относительный и полный пути к файлу
            var storageDirectory = new StorageDirectory
            {
                StoragePath = cacheFilesPath,
                RelativeFilePath = Path.Combine(imageHash, $"{imageHash}_{width}x{height}")
            };
            return storageDirectory;
        }

        internal struct StorageDirectory
        {
            internal string StoragePath;

            internal string RelativeFilePath;

            internal string AbsoluteFilePath => Path.Combine(StoragePath, RelativeFilePath);
        }
    }
}
