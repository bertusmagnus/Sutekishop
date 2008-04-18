using System;
using System.IO;

namespace Suteki.Shop.Services
{
    public class ImageFileService : Suteki.Shop.Services.IImageFileService
    {
        string imageFolderPath;

        public ImageFileService(string imageFolderPath)
        {
            this.imageFolderPath = imageFolderPath;
        }

        public string GetFullPath(string filename)
        {
            return Path.Combine(imageFolderPath, filename);
        }

        public string GetThumbPath(Image image)
        {
            return GetFullPath(image.ThumbFileName);
        }

        public string GetMainPath(Image image)
        {
            return GetFullPath(image.MainFileName);
        }
    }
}
