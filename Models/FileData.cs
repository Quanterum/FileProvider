using System;

namespace FileProvider.Models
{
    /// <summary>
    ///     Модель описывающая файл
    /// </summary>
    public class FileData
    {
        /// <summary>
        ///     Инициализация файла
        /// </summary>
        public FileData()
        {
        }

        /// <summary>
        ///     Создание файла
        /// </summary>
        /// <param name="fileHash">Хеш-сумма содержимого файла</param>
        /// <param name="fileUrl">Путь к файлу из внешней системы (URL отложенной загрузки файла)</param>
        /// <param name="fileName">Имя файла</param>
        /// <param name="mimeType">Тип MIME описывает содержимое файла</param>
        /// <param name="fileSize">Размер файла</param>
        public FileData(string fileHash, string fileUrl, string fileName, string mimeType, long fileSize)
        {
            FileHash = fileHash;
            FileUrl = fileUrl;
            FileName = fileName;
            MimeType = mimeType;
            FileSize = FileSizeToString(fileSize);
        }

        /// <summary>
        ///     Создание файла
        /// </summary>
        /// <param name="fileHash">Хеш-сумма содержимого файла</param>
        /// <param name="fileUrl">Путь к файлу из внешней системы (URL отложенной загрузки файла)</param>
        /// <param name="fileName">Имя файла</param>
        /// <param name="mimeType">Тип MIME описывает содержимое файла</param>
        /// <param name="fileSize">Размер файла</param>
        /// <param name="dateCreation">Дата создания файла</param>
        /// <param name="relativePath">Директория, в которой находится файл</param>
        public FileData(string fileHash, string fileUrl, string fileName, string mimeType, long fileSize,
            DateTime dateCreation, string relativePath) : this(fileHash, fileUrl, fileName, mimeType, fileSize)
        {
            DateCreation = dateCreation;
            RelativePath = relativePath;
        }

        /// <summary>
        ///     Хеш файла
        /// </summary>
        public string FileHash { get; set; }

        /// <summary>
        ///     Путь к файлу из внешней системы
        ///     (URL отложенной загрузки файла)
        /// </summary>
        public string FileUrl { get; set; }

        /// <summary>
        ///     Имя файла
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        ///     Тип MIME описывает содержимое файла
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        ///     Размер файла
        /// </summary>
        public string FileSize { get; set; }

        /// <summary>
        ///     Дата создания файла
        /// </summary>
        public DateTime DateCreation { get; set; } = DateTime.Now;

        /// <summary>
        ///     Директория, в которой находится файл
        /// </summary>
        public string RelativePath { get; set; }

        /// <summary>
        ///     Метод для переименования файла.
        /// </summary>
        /// <param name="fileName">Новое имя для файла.</param>
        public void Rename(string fileName)
        {
            FileName = fileName;
        }

        /// <summary>
        ///     Метод для установки дирректории
        /// </summary>
        /// <param name="relativePath">Относительный путь к файлу.</param>
        public void SetDirectory(string relativePath)
        {
            RelativePath = relativePath;
        }

        internal void SetFileSize(long filesize)
        {
            FileSize = FileSizeToString(filesize);
        }

        private static string FileSizeToString(long fileSize)
        {
            string[] sizes = {"B", "KB", "MB", "GB", "TB"};
            var order = 0;
            while (fileSize >= 1024 && order < sizes.Length - 1)
            {
                order++;
                fileSize /= 1024;
            }

            return string.Concat(fileSize, ' ', sizes[order]);
        }
    }
}
