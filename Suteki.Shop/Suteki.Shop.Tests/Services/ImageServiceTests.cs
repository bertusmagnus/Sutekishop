using System;
using Moq;
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

            var imageFileService = new Mock<ImageFileService>().Object;
            Mock.Get(imageFileService).Expect(ifs => ifs.GetImageFolderPath()).Returns(imageFolderPath);

            imageService = new ImageService(imageFileService, 500, 500, 100, 100);

            string jpgTestPath = Path.Combine(imageFolderPath, image.FileNameAsString);
            string jpgMainPath = Path.Combine(imageFolderPath, image.MainFileName);
            string jpgThumbPath = Path.Combine(imageFolderPath, image.ThumbFileName);

            imageService.CreateSizedImages(image);

            Assert.IsTrue(File.Exists(jpgMainPath));
            Assert.IsTrue(File.Exists(jpgThumbPath));

            Console.WriteLine(jpgMainPath);

            //File.Delete(jpgMainPath);
            //File.Delete(jpgThumbPath);
        }

        public void GuidGenerator()
        {
            Console.WriteLine(Guid.NewGuid().ToString());
        }
    }
}
