using System;
using System.IO;

namespace Suteki.Shop
{
    public partial class Image
    {
        public static string GetExtendedName(string path, ImageNameExtension imageNameExtension)
        {
            string extension = Enum.GetName(typeof(ImageNameExtension), imageNameExtension).ToLower();
            return string.Format("{0}.{1}{2}",
                Path.Combine(
                    Path.GetDirectoryName(path),
                    Path.GetFileNameWithoutExtension(path)),
                extension,
                Path.GetExtension(path));

        }

        public string FileNameAsString
        {
            get
            {
                return this.FileName.ToString() + ".jpg";
            }
        }

        public string ThumbFileName
        {
            get
            {
                return GetExtendedName(this.FileNameAsString, ImageNameExtension.Thumb);
            }
        }

        public string MainFileName
        {
            get
            {
                return GetExtendedName(this.FileNameAsString, ImageNameExtension.Main);
            }
        }
    }

    public enum ImageNameExtension
    {
        Thumb,
        Main
    }
}
