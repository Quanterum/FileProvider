using System;
using System.Diagnostics.CodeAnalysis;
using FileProvider.Interfaces;

namespace FileProvider.Models
{
    /// <inheritdoc />
    public class BytesFileContents : IFileContents
    {
        [NotNull] private readonly byte[] _bytes;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="filename">Наименование файла.</param>
        /// <param name="bytes">Файл, представленный в виде массива байт.</param>
        public BytesFileContents(string filename, byte[] bytes)
        {
            FileName = filename ?? throw new NullReferenceException("'Filename' can't be null!");
            _bytes = bytes ?? throw new NullReferenceException("'Bytes' can't be null!");
        }

        /// <inheritdoc />
        public string FileName { get; }

        /// <inheritdoc />
        public byte[] GetBytes()
        {
            return _bytes;
        }
    }
}
