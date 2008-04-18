using System;
using NUnit.Framework;
using Suteki.Shop.Services;
using System.IO;
using System.Reflection;

namespace Suteki.Shop.Tests.Services
{
    [TestFixture]
    public class ImageServiceTests
    {
        ImageService imageService;

        [Test]
        public void CreateSizedImages_ShouldCreateImagesOfTheCorrectSize()
        {
            Image image = new Image { FileName = new Guid("46af1390-4cff-4741-a1d1-3c87b425bac9") };

            string imageFolderPath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "TestImages");

            imageService = new ImageService(
                new ImageFileService(imageFolderPath),
                300, 300, 100, 100);

            string jpgTestPath = Path.Combine(imageFolderPath, image.FileNameAsString);
            string jpgMainPath = Path.Combine(imageFolderPath, image.MainFileName);
            string jpgThumbPath = Path.Combine(imageFolderPath, image.ThumbFileName);

            imageService.CreateSizedImages(image);

            Assert.IsTrue(File.Exists(jpgMainPath));
            Assert.IsTrue(File.Exists(jpgThumbPath));

            File.Delete(jpgMainPath);
            File.Delete(jpgThumbPath);
        }

        public void GuidGenerator()
        {
            Console.WriteLine(Guid.NewGuid().ToString());
        }
    }
}
