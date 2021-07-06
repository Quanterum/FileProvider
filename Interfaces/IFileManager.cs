using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FileProvider.Models;

namespace FileProvider.Interfaces
{
    /// <summary>
    ///     Менеджер для работы с файлами.
    /// </summary>
    public interface IFileManager<TFileData> where TFileData : FileData
    {
        /// <summary>
        ///     Метод для сохранения файла из строки 'Base64' или массива байт.
        ///     <para>
        ///         Метод принимает класс, реализующий интерфейс <see cref="IFileContents" />
        ///         (<see cref="BytesFileContents" /> или <see cref="Base64FileContents" />)
        ///     </para>
        /// </summary>
        /// <param name="fileContents">Должен быть класс, реализующий интерфейс <see cref="IFileContents" /></param>
        /// <returns>Возвращает объект <see cref="FileData" />, соответствующий загруженному файлу.</returns>
        public Task<TFileData> UploadFileAsync(IFileContents fileContents);

        /// <summary>
        ///     Метод для множественного сохранения файлов из строк 'Base64' или массивов байт.
        ///     <para>
        ///         Метод принимает коллекцию классов, реализующих интерфейс <see cref="IFileContents" />
        ///         (<see cref="BytesFileContents" /> или <see cref="Base64FileContents" />)
        ///     </para>
        /// </summary>
        /// <param name="files">Коллекция классов, реализующих интерфейс <see cref="IFileContents" /></param>
        /// <returns>Возвращает коллекцию объектов <see cref="FileData" />, соответствующих загруженным файлам.</returns>
        public Task<IEnumerable<TFileData>> UploadFilesAsync(IEnumerable<IFileContents> files);

        /// <summary>
        ///     Метод для скачивания файла.
        /// </summary>
        /// <param name="hash">Хеш файла для скачивания</param>
        /// <returns>Возвращает <see cref="FileStream" /> с запрашиваемым файлом.</returns>
        public Task<Stream> DownloadFileAsync(string hash);

        /// <summary>
        ///     Метод для скачивания изображения с указанием необходимого размера.
        /// </summary>
        /// <param name="hash">Хеш файла для скачивания.</param>
        /// <param name="resolution">Необходимый размер изображения.</param>
        /// <param name="mimeType">Тип файла.</param>
        /// <returns>Возвращает <see cref="FileStream" /> с запрашиваемым изображением.</returns>
        public Task<Stream> DownloadImageAsync(string hash, (int width, int height)? resolution = null,
            string mimeType = "image/jpeg");
    }
}
