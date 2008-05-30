using System;
using System.IO;
using System.Web;
using Suteki.Common.Extensions;

namespace Suteki.Shop.Services
{
    public class ImageFileService : Suteki.Shop.Services.IImageFileService
    {
        public virtual string GetImageFolderPath()
        {
            return HttpContext.Current.Server.MapPath("/ProductPhotos/");
        }

        public string GetFullPath(string filename)
        {
            return Path.Combine(GetImageFolderPath(), filename);
        }

        public string GetThumbPath(Image image)
        {
            return GetFullPath(image.ThumbFileName);
        }

        public string GetMainPath(Image image)
        {
            return GetFullPath(image.MainFileName);
        }

        public string GetRelativeUrl(string filename)
        {
            return "ProductPhotos/{0}".With(filename);
        }
    }
}
