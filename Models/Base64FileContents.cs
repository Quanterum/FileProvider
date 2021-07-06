using System;
using System.Diagnostics.CodeAnalysis;
using FileProvider.Interfaces;
using FileProvider.Services;

namespace FileProvider.Models
{
    /// <inheritdoc />
    public class Base64FileContents : IFileContents
    {
        [NotNull] private readonly string _base64;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="filename">Наименование файла.</param>
        /// <param name="base64">Файл, закодированный в base64.</param>
        public Base64FileContents(string filename, string base64)
        {
            FileName = filename ?? throw new NullReferenceException("'Filename' can't be null!");
            _base64 = base64 ?? throw new NullReferenceException("'Base64' can't be null!");
        }

        /// <inheritdoc />
        public string FileName { get; }

        /// <inheritdoc />
        public byte[] GetBytes()
        {
            return _base64.ConvertBase64ToByteArray();
        }
    }
}
