using NUnit.Framework;
using Suteki.Shop.Services;
using System.IO;
using System.Reflection;
using Moq;

namespace Suteki.Shop.Tests.Services
{
    [TestFixture]
    public class ImageFileServiceTests
    {
        ImageFileService imageFileService;

        [Test]
        public void GetFullPath_ShouldReturnFullPage()
        {
            string imageFolderPath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "TestImages");
            string filename = "myfile.jpg";

            imageFileService = new Mock<ImageFileService>().Object;
            Mock.Get(imageFileService).Expect(ifs => ifs.GetImageFolderPath()).Returns(imageFolderPath);

            string path = imageFileService.GetFullPath(filename);

            string expectedPath = Path.Combine(imageFolderPath, filename);

            Assert.AreEqual(expectedPath, path);
        }
    }
}
