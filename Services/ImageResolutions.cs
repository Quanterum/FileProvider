namespace FileProvider.Services
{
    /// <summary>
    ///     Класс для упрощения работы с разрешениями изображений.
    /// </summary>
    public partial class ImageResolutions
    {
        private int Width { get; }
        private int Height { get; }

        /// <summary>
        ///     Конструктор для установки размеров изображения.
        /// </summary>
        /// <param name="width">Ширина изображения.</param>
        /// <param name="height">Высота изображения.</param>
        public ImageResolutions(int width, int height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        ///     Метод для извлечения разрешения изображения, если не заданы параметры, вернет '1920*1080'.
        /// </summary>
        /// <returns>Возвращает кортеж, содержащий ширину и высоту, '(1920, 1080)'.</returns>
        public (int width, int height) GetResolution()
        {
            if (Width is 0 || Height is 0)
                return FHD_1920x1080;
            
            return (Width, Height);
        }
    }
    
    /// <summary>
    ///     Константы разрешения изображений.
    /// </summary>
    public partial class ImageResolutions
    {
        /// <summary>
        ///     Константа для разрешения 640*360
        /// </summary>
        public static readonly (int width, int height) nHD_640x360 = (640, 360);
        /// <summary>
        ///     Константа для разрешения 1280*720
        /// </summary>
        public static readonly (int width, int height) HD_1280x720 = (1280, 720);
        /// <summary>
        ///     Константа для разрешения 1920*1080
        /// </summary>
        public static readonly (int width, int height) FHD_1920x1080 = (1920, 1080);
    }
}
