namespace FileProvider.Interfaces
{
    /// <summary>
    ///     Модель, описывающая файл для загрузки на сервер.
    /// </summary>
    public interface IFileContents
    {
        /// <summary>
        ///     Имя файла с расширением, прим.: "image.jpg"
        /// </summary>
        string FileName { get; }

        /// <summary>
        ///     Метод получения массива байт
        /// </summary>
        /// <returns>Возвращает массив байт.</returns>
        byte[] GetBytes();
    }
}
