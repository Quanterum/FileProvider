using System;
using System.Collections.Generic;
using System.Linq;
using FileProvider.Interfaces;
using FileProvider.Models;

namespace FileProvider.Services
{
    internal static class Extensions
    {
        internal static byte[] ConvertBase64ToByteArray(this string base64String) => 
            Convert.FromBase64String(base64String[(base64String.LastIndexOf(',') + 1)..]);

        internal static FileContent ToFileContentsDto(this IFileContents file) => new(file);

        internal static List<FileContent> ToFileContentsDto(this IEnumerable<IFileContents> files)
        {
            return files.Select(file => new FileContent(file)).ToList();
        }
    }
}
