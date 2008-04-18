using System;
using NUnit.Framework;
using Suteki.Shop.Services;
using System.IO;
using System.Reflection;

namespace Suteki.Shop.Tests.Services
{
    [TestFixture]
    public class ImageFileServiceTests
    {
        ImageFileService imageFileService;

        [SetUp]
        public void SetUp()
        {
            string imageFolderPath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "TestImages");
            string filename = "myfile.jpg";

            imageFileService = new ImageFileService(imageFolderPath);

            string path = imageFileService.GetFullPath(filename);

            string expectedPath = Path.Combine(imageFolderPath, filename);

            Assert.AreEqual(expectedPath, path);
        }
    }
}
