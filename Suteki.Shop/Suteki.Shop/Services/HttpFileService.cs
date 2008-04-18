using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using Suteki.Shop.Validation;

namespace Suteki.Shop.Services
{
    public class HttpFileService : IHttpFileService
    {
        IImageFileService imageFileService;
        IImageService imageService;

        public HttpFileService(IImageFileService imageFileService, IImageService imageService)
        {
            this.imageFileService = imageFileService;
            this.imageService = imageService;
        }

        public IEnumerable<Image> GetUploadedImages(HttpRequestBase request)
        {
            List<Image> images = new List<Image>();

            foreach (string inputTagName in request.Files)
            {
                HttpPostedFile file = request.Files[inputTagName];
                if (file.ContentLength > 0)
                {
                    // upload the image to filesystem
                    if (IsNotImage(file))
                    {
                        throw new ValidationException(string.Format("File '{0}' is not an image file (*.jpg)", file.FileName));
                    }

                    Image image = new Image
                    {
                        FileName = Guid.NewGuid(),
                        Description = Path.GetFileName(file.FileName)
                    };

                    file.SaveAs(imageFileService.GetFullPath(image.FileNameAsString));

                    // convert the image to main and thumb sizes
                    imageService.CreateSizedImages(image);

                    File.Delete(imageFileService.GetFullPath(image.FileNameAsString));

                    images.Add(image);
                }
            }

            return images;
        }

        private bool IsNotImage(HttpPostedFile file)
        {
            string extension = Path.GetExtension(file.FileName).ToLower();
            return (extension != ".jpg");
        }
    }
}
