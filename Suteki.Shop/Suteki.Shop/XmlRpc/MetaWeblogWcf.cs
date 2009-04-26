using System;
using System.IO;
using System.Linq;
using Suteki.Common.Extensions;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Shop.Services;

namespace Suteki.Shop.XmlRpc
{
    public class MetaWeblogWcf : IMetaWeblog
    {
        private readonly IUserService userService;
        private readonly IRepository<Content> contentRepository;
        private readonly IBaseControllerService baseControllerService;
        private readonly IOrderableService<Content> contentOrderableService;
        private readonly IImageFileService imageFileService;

        public MetaWeblogWcf(
            IUserService userService, 
            IRepository<Content> contentRepository, 
            IBaseControllerService baseControllerService, 
            IOrderableService<Content> contentOrderableService, 
            IImageFileService imageFileService)
        {
            this.userService = userService;
            this.imageFileService = imageFileService;
            this.contentOrderableService = contentOrderableService;
            this.baseControllerService = baseControllerService;
            this.contentRepository = contentRepository;
        }

        private void ValidateUser(string username, string password)
        {
            if (!userService.Authenticate(username, password))
            {
                throw new ApplicationException("Invalid username or password");
            }

            // TODO: Check that user is an administrator
        }

        private static int GetContentId(string postid)
        {
            var contentId = 0;
            if (!int.TryParse(postid, out contentId))
            {
                throw new ApplicationException("Invalid post id");
            }
            return contentId;
        }

        private string GetPostUrl(IUrlNamed content)
        {
            return "{0}cms/{1}".With(baseControllerService.SiteUrl, content.UrlName);
        }

        #region Implementation of IMetaWeblog

        public bool editPost(string postid, string username, string password, Post post, bool publish)
        {
            ValidateUser(username, password);
            var contentId = GetContentId(postid);

            var content = contentRepository.GetById(contentId);
            var textContent = content as ITextContent;

            if (textContent == null)
            {
                throw new ApplicationException("Invalid post id");
            }

            content.Name = post.title;
            textContent.Text = post.description;
            content.IsActive = publish;

            contentRepository.SubmitChanges();

            return false;
        }

        public CategoryInfo[] getCategories(string blogid, string username, string password)
        {
            // Suteki shop doesn't support categories
            return new CategoryInfo[0];
        }

        public Post getPost(string postid, string username, string password)
        {
            ValidateUser(username, password);
            int contentId = GetContentId(postid);

            var content = contentRepository.GetById(contentId);
            var textContent = content as ITextContent;

            if (textContent == null)
            {
                throw new ApplicationException("invalid postid");
            }

            return new Post
            {
                link = GetPostUrl(content),
                description = textContent.Text,
                dateCreated = DateTime.Now,
                postid = content.ContentId,
                title = content.Name,
                permalink = "",
                categories = new string[0]
            };
        }

        public Post[] getRecentPosts(string blogid, string username, string password, int numberOfPosts)
        {
            ValidateUser(username, password);

            var posts = contentRepository
                .GetAll()
                .InOrder()
                .Where(c => c is ITextContent)
                .Select(content => new Post
                {
                    dateCreated = DateTime.Now,
                    description = ((ITextContent)content).Text,
                    title = content.Name,
                    postid = content.ContentId,
                    permalink = GetPostUrl(content)
                });

            return posts.ToArray();

        }

        public string newPost(string blogid, string username, string password, Post post, bool publish)
        {
            ValidateUser(username, password);

            var content = new TextContent
            {
                ParentContentId = 1,
                Position = contentOrderableService.NextPosition,
                ContentTypeId = ContentType.TextContentId,
                IsActive = publish,
                Name = post.title,
                Text = post.description
            };

            contentRepository.InsertOnSubmit(content);
            contentRepository.SubmitChanges();

            return content.ContentId.ToString();
        }

        public mediaObjectInfo newMediaObject(object blogid, string username, string password, mediaObject mediaobject)
        {
            ValidateUser(username, password);

            try
            {
                //We don't validate the file because newMediaObject allows file to be overwritten
                //But we do check the directory and create if necessary
                //The media object's name can have extra folders appended so we check for this here too.
                var path = imageFileService.GetFullPath(mediaobject.name.Replace("/", "\\"));

                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                using (var binaryWriter = new BinaryWriter(fileStream))
                {
                    binaryWriter.Write(mediaobject.bits);
                }
            }
            //Any IO exceptions, we throw a new XmlRpcFault Exception
            catch (IOException exception)
            {
                throw new ApplicationException(exception.Message);
            }

            ////If all works, we return a mediaobjectinfo struct back holding the URL.
            mediaObjectInfo media;
            media.url = baseControllerService.SiteUrl + imageFileService.GetRelativeUrl(mediaobject.name);
            return media;

        }

        public bool deletePost(string appKey, string postid, string username, string password, bool publish)
        {
            ValidateUser(username, password);
            var contentId = GetContentId(postid);

            var content = contentRepository.GetById(contentId);

            contentRepository.DeleteOnSubmit(content);
            contentRepository.SubmitChanges();

            return false;
        }

        public BlogInfo[] getUsersBlogs(string appKey, string username, string password)
        {
            ValidateUser(username, password);

            return new[]
            {
                new BlogInfo
                {
                    blogid = "1",
                    blogName = "Suteki Shop",
                    url = baseControllerService.SiteUrl
                }
            };
        }

        #endregion
    }
}