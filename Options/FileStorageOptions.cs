namespace FileProvider.Options
{
#nullable disable
    /// <summary>
    ///     Модель для настройки параметров файлового менеджера
    /// </summary>
    public class FileStorageOptions
    {
        /// <summary>
        ///     Путь для хранения пользовательских файлов
        /// </summary>
        public string StorageUserFilesPath { get; set; }

        /// <summary>
        ///     Путь для хранения системных файлов
        /// </summary>
        public string StorageDataFilesPath { get; set; }

        /// <summary>
        ///     Путь для хранения временных кэш-файлов
        /// </summary>
        public string CacheFilesPath { get; set; }

        /// <summary>
        ///     Включение/отключение кэширования изображений
        /// </summary>
        public bool IsCacheFilesAllowed { get; set; }

        /// <summary>
        ///     Объект, описывающий параметры хранилища S3
        /// </summary>
        public S3Options S3Options { get; set; }

        /// <summary>
        ///     Объект, описывающий параметры хранилища NFS
        /// </summary>
        public NFSOptions NFSOptions { get; set; }
    }

    /// <summary>
    ///     Объект, описывающий параметры хранилища S3
    /// </summary>
    public class S3Options
    {
        /// <summary>
        ///     Разрешена ли загрузка файлов в хранилище
        /// </summary>
        public bool IsUploadsAllowed { get; set; }

        /// <summary>
        ///     Разрешено ли скачивание из хранилища
        /// </summary>
        public bool IsDownloadsAllowed { get; set; }

        /// <summary>
        ///     URL сервера S3
        /// </summary>
        public string S3Url { get; set; }

        /// <summary>
        ///     Ключ доступа к серверу S3
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        ///     Секретный ключ доступа к серверу
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        ///     Название корзины на сервере S3
        /// </summary>
        public string BucketName { get; set; }
    }

    /// <summary>
    ///     Объект, описывающий параметры хранилища NFS
    /// </summary>
    public class NFSOptions
    {
        /// <summary>
        ///     Разрешена ли загрузка файлов в хранилище
        /// </summary>
        public bool IsUploadsAllowed { get; set; }

        /// <summary>
        ///     Разрешено ли скачивание из хранилища
        /// </summary>
        public bool IsDownloadsAllowed { get; set; }

        /// <summary>
        ///     Путь для сохранения файлов в хранилище NFS
        /// </summary>
        public string StoragePath { get; set; }
    }
}
