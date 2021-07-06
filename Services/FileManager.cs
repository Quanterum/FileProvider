using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using FileProvider.Interfaces;
using FileProvider.Models;
using FileProvider.Options;
using Microsoft.Extensions.Options;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace FileProvider.Services
{
    /// <inheritdoc />
    public class FileManager<TFileData> : IFileManager<TFileData> where TFileData : FileData, new()
    {
        private readonly FileStorageOptions _options;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="options">Модель для настройки параметров файлового менеджера</param>
        public FileManager(IOptions<FileStorageOptions> options)
        {
            _options = options.Value;
        }

        /// <inheritdoc />
        public async Task<TFileData> UploadFileAsync(IFileContents fileContents)
        {
            return await UploadAsync(fileContents.ToFileContentsDto());
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TFileData>> UploadFilesAsync(IEnumerable<IFileContents> filesContents)
        {
            List<TFileData> response = new();
            foreach (var fileContent in filesContents)
            {
                response.Add(await UploadAsync(fileContent.ToFileContentsDto()));
            }
            return response;
        }

        /// <inheritdoc />
        public async Task<Stream> DownloadFileAsync(string hash)
        {
            if (_options.S3Options.IsDownloadsAllowed)
            {
                var stream = await DownloadFileFromS3Async(hash);
                if (stream is not null)
                    return stream;
            }

            if (_options.NFSOptions.IsDownloadsAllowed)
                return DownloadFileFromNfs(hash);

            return new FileStream("wwwroot/Uploads/no-image.png", FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        /// <inheritdoc />
        public async Task<Stream> DownloadImageAsync(string hash, (int width, int height)? resolution = null,
            string mimeType = "image/jpeg")
        {
            var customResolution = resolution ?? ImageResolutions.HD_1280x720;
            // Поиск файла в кэше
            var result = CheckFileInCache(hash, customResolution, out var stream);
            if (result) return stream;

            if (_options.S3Options.IsDownloadsAllowed)
                stream = await DownloadFileFromS3Async(hash);

            if (_options.NFSOptions.IsDownloadsAllowed && stream is null)
                stream = DownloadFileFromNfs(hash);

            var response = await MutateImage(stream, hash, customResolution, mimeType);

            return response ?? new FileStream("wwwroot/Uploads/no-image.png", FileMode.Open, FileAccess.Read,
                FileShare.Read);
        }


        #region private

        private async Task<TFileData> UploadAsync(FileContent file)
        {
            if (!MimeTypes.TryGetMimeType(file.FileName, out var mimeType))
                throw new FormatException(
                    $"Ошибка, имя файла '{file.FileName}' не содержит известное расширение! (прим. 'image.jpg')");
            if (file.Bytes is null) return null;

            SaveTempFileFromBytesArray(file.Bytes, file.FileName, out var tempFileName);

            var md5 = MD5.Create();
            await using var stream = File.OpenRead(tempFileName);
            var fileData = new TFileData
            {
                FileHash = BitConverter.ToString(await md5.ComputeHashAsync(stream)).Replace("-", string.Empty),
                FileUrl = "null",
                FileName = file.FileName,
                MimeType = mimeType
            };
            fileData.SetFileSize(stream.Length);
            
            stream.Close();

            if (_options.S3Options.IsUploadsAllowed)
                await UploadFileToS3Async(tempFileName, fileData);

            if (_options.NFSOptions.IsUploadsAllowed)
                UploadFileToNfs(tempFileName, fileData);

            File.Delete(tempFileName);
            return fileData;
        }

        private static void SaveTempFileFromBytesArray(byte[] bytes, string fileName, out string tempFileName)
        {
            var fileNameGuid = Guid.NewGuid();
            var expansion = fileName[fileName.LastIndexOf('.')..];
            File.WriteAllBytes(fileNameGuid + expansion, bytes);
            tempFileName = fileNameGuid + expansion;
        }

        #region NFS Storage

        private void UploadFileToNfs(string tempFileName, FileData fileData)
        {
            // Вычисляю пути на основании хеша
            var dir = fileData.FileHash.GetStorageDirectory(_options);
            if (File.Exists(dir.AbsoluteFilePath))
                return;

            // Помещаю файл в хранилище
            File.Copy(tempFileName, dir.AbsoluteFilePath);
            fileData.SetDirectory(dir.RelativeFilePath);
        }

        private Stream DownloadFileFromNfs(string hash)
        {
            // Вычисляю пути к файлу в хранилище
            var directory = hash.GetStorageDirectory(_options);
            if (File.Exists(directory.AbsoluteFilePath))
                return new FileStream(directory.AbsoluteFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            Log.Warning("Искомый файл не найден по пути: " + directory.AbsoluteFilePath);
            return new FileStream("wwwroot/Uploads/no-image.png", FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        #endregion

        #region S3 Storage

        private async Task UploadFileToS3Async(string tempFileName, FileData fileData)
        {
            try
            {
                var options = _options.S3Options;
                var amazonS3Config = new AmazonS3Config {ServiceURL = options.S3Url};
                var awsCredentials = new BasicAWSCredentials(options.AccessKey, options.SecretKey);
                using var s3Client = new AmazonS3Client(awsCredentials, amazonS3Config);

                var request = new PutObjectRequest
                {
                    BucketName = options.BucketName,
                    CannedACL = S3CannedACL.PublicReadWrite,
                    Key = Path.Combine(_options.StorageUserFilesPath, fileData.FileHash),
                    ContentType = fileData.MimeType,
                    InputStream = new FileStream(tempFileName, FileMode.Open, FileAccess.Read, FileShare.Read)
                };

                var result = await s3Client.PutObjectAsync(request);
                Log.Information(string.Join(Environment.NewLine, result));
            }
            catch (Exception exception)
            {
                Log.Information(exception, "Ошибка во время загрузки файла в S3 хранилище:");
            }
        }

        private async Task<Stream> DownloadFileFromS3Async(string hash)
        {
            try
            {
                var options = _options.S3Options;
                var amazonS3Config = new AmazonS3Config {ServiceURL = options.S3Url};
                var awsCredentials = new BasicAWSCredentials(options.AccessKey, options.SecretKey);
                using var s3Client = new AmazonS3Client(awsCredentials, amazonS3Config);

                var request = new GetObjectRequest
                {
                    BucketName = options.BucketName,
                    Key = Path.Combine(_options.StorageUserFilesPath, hash)
                };
                var result = await s3Client.GetObjectAsync(request);
                if (result.HttpStatusCode == HttpStatusCode.OK)
                    return result.ResponseStream;

                Log.Error($"Произошла ошибка при получении файла из S3: StatusCode = {result.HttpStatusCode}");
            }
            catch (Exception exception)
            {
                Log.Information(exception, "Ошибка во время получения файла из S3 хранилища:");
            }

            return null;
        }

        #endregion

        #region Work with image

        private bool CheckFileInCache(string hash, (int width, int height) resolution, out Stream fileStream)
        {
            fileStream = null;
            var (width, height) = resolution;
            if (!_options.IsCacheFilesAllowed)
                return false;

            // Вычисление директории и проверка на существование файла.
            var filePath = DirectoryManager.GetCacheDirectory(hash, _options.CacheFilesPath, width, height);
            if (!File.Exists(filePath.AbsoluteFilePath)) return false;

            fileStream = new FileStream(filePath.AbsoluteFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return true;
        }

        private async Task<Stream> MutateImage(Stream sourceImage, string imageHash, (int width, int height) resolution,
            string mimeType)
        {
            if (!_options.IsCacheFilesAllowed || mimeType is "image/svg+xml") return sourceImage;

            var (width, height) = resolution;
            using var image = await Image.LoadAsync(sourceImage);
            ResizeAndCropImage(image, width, height);

            var path = DirectoryManager.GetCacheDirectory(imageHash, _options.CacheFilesPath, width, height);
            await image.SaveAsync(path.AbsoluteFilePath,
                mimeType is "image/png" ? new PngEncoder() : new JpegEncoder());
            return new FileStream(path.AbsoluteFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        private void ResizeAndCropImage(Image image, int width, int height)
        {
            var originalWidth = image.Width;
            var originalHeight = image.Height;

            var widthRatio = (double) originalWidth / width;
            var heightRatio = (double) originalHeight / height;

            var ratio = widthRatio < heightRatio ? widthRatio : heightRatio;

            var newWidth = Convert.ToInt32(originalWidth / ratio);
            var newHeight = Convert.ToInt32(originalHeight / ratio);

            var rectanglePointX = (newWidth - width) / 2;
            var rectanglePointY = (newHeight - height) / 2;

            image.Mutate(x =>
                x.Resize(newWidth, newHeight)
                    .Crop(new Rectangle(rectanglePointX, rectanglePointY, width, height)));
        }

        /*
nHD	        640×360	    16:9	
FWVGA	    848×480	    16:9
qHD	        960×540     16:9
HD 720p	    1280×720	16:9
WXGA++	    1600×900	16:9
Full HD     1920×1080	16:9
QWXGA	    2048×1152	16:9
WQXGA       2560×1440	16:9
WQXGA	    3200×1800	16:9
4K UHD      3840×2160	16:9
8K UHD      7680×4320	16:9
QVGA	    320×240	    4:3
VGA	        640×480	    4:3
SVGA	    800×600     4:3
XGA	        1024×768	4:3
XGA+	    1152×864	4:3
SXGA+	    1400×1050	4:3
UXGA	    1600×1200	4:3
QXGA	    2048×1536	4:3
QUXGA	    3200×2400	4:3
HUXGA	    6400×4800	4:3
         */

        #endregion

        #endregion
    }
}
