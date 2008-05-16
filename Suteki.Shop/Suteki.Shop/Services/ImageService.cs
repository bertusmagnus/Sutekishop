using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Suteki.Shop.Services
{
    public class ImageService : Suteki.Shop.Services.IImageService
    {
        ImageSize mainSize;
        ImageSize thumbnailSize;
        IImageFileService imageFileService;

        public ImageService(IImageFileService imageFileService, 
            int mainHeight, int mainWidth, int thumbHeight, int thumbWidth)
        {
            this.imageFileService = imageFileService;
            this.mainSize = new ImageSize(mainWidth, mainHeight);
            this.thumbnailSize = new ImageSize(thumbWidth, thumbHeight);
        }

        /// <summary>
        /// Creates a main and thumnail image from the given image of the taget size
        /// </summary>
        /// <param name="imagePath"></param>
        public void CreateSizedImages(Image image)
        {
            string imagePath = imageFileService.GetFullPath(image.FileNameAsString);
            using (System.Drawing.Image gdiImage = System.Drawing.Image.FromFile(imagePath))
            {
                CreateSizedImage(gdiImage, mainSize, Image.GetExtendedName(imagePath, ImageNameExtension.Main));
                CreateSizedImage(gdiImage, thumbnailSize, Image.GetExtendedName(imagePath, ImageNameExtension.Thumb));
            }
        }

        private void CreateSizedImage(System.Drawing.Image image, ImageSize targetSize, string targetPath)
        {
            ImageComparer imageComparer = new ImageComparer(image, targetSize);
            ImageSize newImageSize = null;

            if (imageComparer.IsLandscape)
            {
                newImageSize = imageComparer.LandscapeSize;
            }
            else
            {
                newImageSize = imageComparer.PortraitSize;
            }

            using (Bitmap bitmap = new Bitmap(newImageSize.Width, newImageSize.Height,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb))
            {
                bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(image,
                        new Rectangle(0, 0, newImageSize.Width, newImageSize.Height),
                        new Rectangle(0, 0, image.Width, image.Height),
                        GraphicsUnit.Pixel);
                }

                bitmap.Save(targetPath, ImageFormat.Jpeg);
            }
        }
    }

    public class ImageSize
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public ImageSize(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }
    }

    public class ImageComparer
    {
        readonly System.Drawing.Image sourceImage;
        readonly ImageSize targetSize;

        public ImageComparer(System.Drawing.Image sourceImage, ImageSize targetSize)
        {
            this.sourceImage = sourceImage;
            this.targetSize = targetSize;
        }

        public float WidthRatio { get { return (float)targetSize.Width / (float)sourceImage.Width; } }
        public float HeightRatio { get { return (float)targetSize.Height / (float)sourceImage.Height; } }
        public bool IsLandscape { get { return HeightRatio >= WidthRatio; } }
        
        public ImageSize LandscapeSize 
        { 
            get 
            { 
                return new ImageSize((int)(sourceImage.Width * WidthRatio), (int)(sourceImage.Height * WidthRatio)); 
            } 
        }

        public ImageSize PortraitSize
        {
            get
            {
                return new ImageSize((int)(sourceImage.Width * HeightRatio), (int)(sourceImage.Height * HeightRatio));
            }
        }
    }
}
