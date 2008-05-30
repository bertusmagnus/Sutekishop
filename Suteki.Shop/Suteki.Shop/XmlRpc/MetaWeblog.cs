using System;
using System.Linq;
using System.IO;
using CookComputing.XmlRpc;
using Suteki.Common.Extensions;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Shop.Repositories;
using System.Web.Security;
using Suteki.Shop.Services;
using System.Web;
using Castle.Windsor;

// This code is lifted straight on SubText http://sourceforge.net/projects/subtext

namespace Suteki.Shop.XmlRpc
{
    /// <summary>
    /// Implements the MetaBlog API.
    /// </summary>
    public class MetaWeblog : XmlRpcService, IMetaWeblog
    {
        private IRepository<Content> contentRepository;
        private IRepository<User> userRepository;
        private IOrderableService<Content> contentOrderableService;
        private IBaseControllerService baseControllerService;
        private IImageFileService imageFileService;

        public MetaWeblog(
            IRepository<Content> contentRepository,
            IRepository<User> userRepository,
            IOrderableService<Content> contentOrderableService,
            IBaseControllerService baseControllerService,
            IImageFileService imageFileService)
        {
            this.contentRepository = contentRepository;
            this.userRepository = userRepository;
            this.contentOrderableService = contentOrderableService;
            this.baseControllerService = baseControllerService;
            this.imageFileService = imageFileService;
        }

        public MetaWeblog(IWindsorContainer iocContainer) : this(
            iocContainer.Resolve<IRepository<Content>>(),
            iocContainer.Resolve<IRepository<User>>(),
            iocContainer.Resolve<IOrderableService<Content>>(),
            iocContainer.Resolve<IBaseControllerService>(),
            iocContainer.Resolve<IImageFileService>())
        {
        }

        public MetaWeblog()
        {
            // not using IoC container, manually creating
            var dataContext = new ShopDataContext();
            this.contentRepository = new Repository<Content>(dataContext);
            this.userRepository = new Repository<User>(dataContext);
            var categoryRepository = new Repository<Suteki.Shop.Category>(dataContext);
            this.contentOrderableService = new OrderableService<Content>(contentRepository);
            this.baseControllerService = new BaseControllerService(categoryRepository, contentRepository);
            this.imageFileService = new ImageFileService();
        }

//        public MetaWeblog()
//            : this(GetIocContainer())
//        {
//        }

        /// <summary>
        /// We want to get all our sevices from the IoC container.
        /// </summary>
        /// <returns></returns>
        private static IWindsorContainer GetIocContainer()
        {
            IContainerAccessor containerAccessor = HttpContext.Current.ApplicationInstance as IContainerAccessor;
            if (containerAccessor == null)
            {
                throw new XmlRpcFaultException(1,
                    "MetaWeblog's default constructor can only be used with an HttpApplication that implemenets IContainerAccessor");
            }

            return containerAccessor.Container;
        }

        private void ValidateUser(string username, string password)
        {
            User user = userRepository.GetAll().GetUser(username, EncryptPassword(password));

            if (user == null)
            {
                throw new XmlRpcFaultException(0, "Invalid email and/or password.");
            }

            if (!user.IsAdministrator)
            {
                throw new XmlRpcFaultException(0, "Email does not have permission to edit content.");
            }
        }

        public virtual string EncryptPassword(string password)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");
        }

        #region BlogApi Members

        public BlogInfo[] getUsersBlogs(string appKey, string username, string password)
        {
            ValidateUser(username, password);

            BlogInfo[] blogInfos = new BlogInfo[]
            {
                new BlogInfo
                {
                    blogid = "1",
                    blogName = "Suteki Shop",
                    url = baseControllerService.SiteUrl
                }
            };

            return blogInfos;
        }

        public bool deletePost(string appKey, string postid, string username, string password, [XmlRpcParameter(Description = "Where applicable, this specifies whether the blog should be republished after the post has been deleted.")] bool publish)
        {
            ValidateUser(username, password);
            int contentId = GetContentId(postid);

            Content content = contentRepository.GetById(contentId);

            contentRepository.DeleteOnSubmit(content);
            contentRepository.SubmitChanges();

            return false;
        }

        private static int GetContentId(string postid)
        {
            int contentId = 0;
            if (!int.TryParse(postid, out contentId))
            {
                throw new XmlRpcFaultException(1, "Invalid post id");
            }
            return contentId;
        }

        #endregion

        public bool editPost(string postid, string username, string password, Post post, bool publish)
        {
            ValidateUser(username, password);
            int contentId = GetContentId(postid);

            var content = contentRepository.GetById(contentId);
            var textContent = content as ITextContent;

            if (textContent == null)
            {
                throw new XmlRpcFaultException(1, "Invalid post id");
            }

            content.Name = post.title;
            textContent.Text = post.description;
            content.IsActive = publish;

            contentRepository.SubmitChanges();

            return false;

            //Entry entry = Entries.GetEntry(Int32.Parse(postid), PostConfig.None, true);
            //if (entry != null)
            //{
            //    entry.Author = info.Author;
            //    entry.Email = info.Email;
            //    entry.Body = post.description;
            //    entry.Title = post.title;
            //    entry.Description = string.Empty;

            //    entry.Categories.Clear();
            //    if (post.categories != null)
            //        entry.Categories.AddRange(post.categories);

            //    entry.PostType = PostType.BlogPost;
            //    entry.IsActive = publish;

            //    entry.DateModified = Config.CurrentBlog.TimeZone.Now;
            //    int[] categoryIds = { };
            //    if (entry.Categories.Count > 0)
            //    {
            //        categoryIds = Entries.GetCategoryIdsFromCategoryTitles(entry);
            //    }
            //    Entries.Update(entry);
            //    Entries.SetEntryCategoryList(entry.Id, categoryIds);

            //    if (entry.Enclosure == null)
            //    {
            //        if (!string.IsNullOrEmpty(post.enclosure.url))
            //        {
            //            Components.Enclosure enc = new Components.Enclosure();
            //            enc.Url = post.enclosure.url;
            //            enc.MimeType = post.enclosure.type;
            //            enc.Size = post.enclosure.length;
            //            enc.EntryId = entry.Id;
            //            Enclosures.Create(enc);
            //        }
            //    }
            //    else
            //    {
            //        if (!string.IsNullOrEmpty(post.enclosure.url))
            //        {
            //            Components.Enclosure enc = entry.Enclosure;
            //            enc.Url = post.enclosure.url;
            //            enc.MimeType = post.enclosure.type;
            //            enc.Size = post.enclosure.length;
            //            Enclosures.Update(enc);
            //        }
            //        else
            //        {
            //            Enclosures.Delete(entry.Enclosure.Id);
            //        }
            //    }
            //}
            //return false;
        }

        public Post getPost(string postid, string username, string password)
        {
            ValidateUser(username, password);
            int contentId = GetContentId(postid);

            var content = contentRepository.GetById(contentId);
            var textContent = content as ITextContent;

            if (textContent == null)
            {
                throw new XmlRpcFaultException(1, "invalid postid");
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

            //Framework.BlogInfo info = Config.CurrentBlog;
            //ValidateUser(username, password, info.AllowServiceAccess);

            //Entry entry = Entries.GetEntry(Int32.Parse(postid), PostConfig.None, true);
            //Post post = new Post();
            //post.link = entry.Url;
            //post.description = entry.Body;
            //post.dateCreated = entry.DateCreated;
            //post.postid = entry.Id;
            //post.title = entry.Title;
            //post.permalink = entry.FullyQualifiedUrl.ToString();
            //post.categories = new string[entry.Categories.Count];
            //entry.Categories.CopyTo(post.categories, 0);

            //return post;
        }

        private string GetPostUrl(Content content)
        {
            return "{0}cms/{1}".With(baseControllerService.SiteUrl, content.UrlName);
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

            //ICollection<Entry> ec = Entries.GetRecentPosts(numberOfPosts, PostType.BlogPost, PostConfig.IsActive, true);
            ////int i = 0;
            //int count = ec.Count;
            //Post[] posts = new Post[count];

            //int i = 0;
            //foreach (Entry entry in ec)
            //{
            //    Post post = new Post();
            //    post.dateCreated = entry.DateCreated;
            //    post.description = entry.Body;
            //    post.link = entry.Url;
            //    post.permalink = entry.FullyQualifiedUrl.ToString();
            //    post.title = entry.Title;
            //    post.postid = entry.Id.ToString(CultureInfo.InvariantCulture);
            //    post.userid = entry.Body.GetHashCode().ToString(CultureInfo.InvariantCulture);
            //    if (entry.Categories != null && entry.Categories.Count > 0)
            //    {
            //        post.categories = new string[entry.Categories.Count];
            //        entry.Categories.CopyTo(post.categories, 0);
            //    }
            //    if (entry.Enclosure != null)
            //    {
            //        post.enclosure.length = (int)entry.Enclosure.Size;
            //        post.enclosure.url = entry.Enclosure.Url;
            //        post.enclosure.type = entry.Enclosure.MimeType;
            //    }
            //    posts[i] = post;
            //    i++;
            //}
            //return posts;
        }

        public CategoryInfo[] getCategories(string blogid, string username, string password)
        {
            //Framework.BlogInfo info = Config.CurrentBlog;
            //ValidateUser(username, password, info.AllowServiceAccess);

            //ICollection<LinkCategory> lcc = Links.GetCategories(CategoryType.PostCollection, ActiveFilter.None);
            //if (lcc == null)
            //{
            //    throw new XmlRpcFaultException(0, "No categories exist");
            //}
            //CategoryInfo[] categories = new CategoryInfo[lcc.Count];
            //int i = 0;
            //foreach (LinkCategory linkCategory in lcc)
            //{
            //    CategoryInfo category = new CategoryInfo();
            //    category.categoryid = linkCategory.Id.ToString(CultureInfo.InvariantCulture);
            //    category.title = linkCategory.Title;
            //    category.htmlUrl = info.RootUrl + "Category/" + linkCategory.Id.ToString(CultureInfo.InvariantCulture) + ".aspx";
            //    category.rssUrl = info.RootUrl + "rss.aspx?catid=" + linkCategory.Id.ToString(CultureInfo.InvariantCulture);
            //    category.description = linkCategory.Title;

            //    categories[i] = category;
            //    i++;
            //}
            //return categories;

            return new CategoryInfo[0];
        }

        /// <summary>
        /// Creates a new post.  The publish boolean is used to determine whether the item 
        /// should be published or not.
        /// </summary>
        /// <param name="blogid">The blogid.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="post">The post.</param>
        /// <param name="publish">if set to <c>true</c> [publish].</param>
        /// <returns></returns>
        public string newPost(string blogid, string username, string password, Post post, bool publish)
        {
            ValidateUser(username, password);

            TextContent content = new TextContent
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

            //Entry entry = new Entry(PostType.BlogPost);
            //entry.Author = info.Author;
            //entry.Email = info.Email;
            //entry.Body = post.description;
            //entry.Title = post.title;
            //entry.Description = string.Empty;

            ////TODO: Figure out why this is here.
            ////		Probably means the poster forgot to set the date.
            //if (post.dateCreated.Year >= 2003)
            //{
            //    entry.DateCreated = post.dateCreated;
            //    entry.DateModified = post.dateCreated;
            //}
            //else
            //{
            //    entry.DateCreated = Config.CurrentBlog.TimeZone.Now;
            //    entry.DateModified = entry.DateCreated;
            //}

            //if (post.categories != null)
            //{
            //    entry.Categories.AddRange(post.categories);
            //}

            //entry.PostType = PostType.BlogPost;

            //entry.IsActive = publish;
            //entry.AllowComments = true;
            //entry.DisplayOnHomePage = true;
            //entry.IncludeInMainSyndication = true;
            //entry.IsAggregated = true;
            //entry.SyndicateDescriptionOnly = false;

            //int postID;
            //try
            //{
            //    postID = Entries.Create(entry);

            //    if (!string.IsNullOrEmpty(post.enclosure.url))
            //    {
            //        Components.Enclosure enc = new Components.Enclosure();
            //        enc.Url = post.enclosure.url;
            //        enc.MimeType = post.enclosure.type;
            //        enc.Size = post.enclosure.length;
            //        enc.EntryId = postID;
            //        Enclosures.Create(enc);
            //    }

            //    AddCommunityCredits(entry);
            //}
            //catch (Exception e)
            //{
            //    throw new XmlRpcFaultException(0, e.Message + " " + e.StackTrace);
            //}
            //if (postID < 0)
            //{
            //    throw new XmlRpcFaultException(0, "The post could not be added");
            //}
            //return postID.ToString(CultureInfo.InvariantCulture);

            return content.ContentId.ToString();
        }

        //private void AddCommunityCredits(Entry entry)
        //{
        //    string result = string.Empty;

        //    try
        //    {
        //        CommunityCreditNotification.AddCommunityCredits(entry);
        //    }
        //    catch (CommunityCreditNotificationException ex)
        //    {
        //        Log.WarnFormat("Community Credit ws returned the following response while notifying for the url {0}: {1}", entry.FullyQualifiedUrl.ToString(), ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error("Error while connecting to the Community Credit webservice", ex);
        //    }
        //}

        public mediaObjectInfo newMediaObject(object blogid, string username, string password, mediaObject mediaobject)
        {
            ValidateUser(username, password);

            try
            {
                //We don't validate the file because newMediaObject allows file to be overwritten
                //But we do check the directory and create if necessary
                //The media object's name can have extra folders appended so we check for this here too.
                string path = imageFileService.GetFullPath(mediaobject.name.Replace("/", "\\"));

                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }

                using (FileStream fileStream = new FileStream(path, FileMode.Create))
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                {
                    binaryWriter.Write(mediaobject.bits);
                }
            }
            //Any IO exceptions, we throw a new XmlRpcFault Exception
            catch (IOException exception)
            {
                throw new XmlRpcFaultException(0, exception.Message);
            }

            ////If all works, we return a mediaobjectinfo struct back holding the URL.
            mediaObjectInfo media;
            media.url = baseControllerService.SiteUrl + imageFileService.GetRelativeUrl(mediaobject.name);
            return media;
        }

        #region w.bloggar workarounds/nominal MT support - HACKS

        // w.bloggar is not correctly implementing metaWeblogAPI on its getRecentPost call, it wants 
        // an instance of blogger.getRecentPosts at various time. 
        // 
        // What works better with w.bloggar is to tell it to use MT settings. For w.bloggar users 
        // with metaWeblog configured, we'll throw a more explanatory exception than method not found.

        public struct BloggerPost
        {
            public string content;
            public DateTime dateCreated;
            public string postid;
            public string userid;
        }

        [XmlRpcMethod("blogger.getRecentPosts",
             Description = "Workaround for w.bloggar errors. Exists just to throw an exception explaining issue.")]
        public BloggerPost[] GetRecentPosts(string appKey, string blogid, string username,
            string password, int numberOfPosts)
        {
            throw new XmlRpcFaultException(0, "You are most likely getting this message because you are using w.bloggar or trying to access Blogger API support in .Text--only metaWeblog API is currently supported. If your issue is w.bloggar, read on.\n\nw.bloggar does not correctly implement the metaWeblog API.\n\nIt is trying to call blogger.getRecentPosts, which does not exist in the metaWeblog API. Contact w.bloggar and encourage them to fix this bug.\n\nIn the meantime, to workaround this, go to the Account Properties dialog and hit 'Reload Blogs List'. This should clear the issue temporarily on w.bloggars side.");
        }

        // we'll also add a couple structs and methods to give us nominal MT API-level support.
        // by doing this we'll allow w.bloggar to run against .Text using w.b's MT configuration.
        public struct MtCategory
        {
            public string categoryId;
            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public string categoryName;
            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public bool isPrimary;

            /// <summary>
            /// Initializes a new instance of the <see cref="MtCategory"/> class.
            /// </summary>
            /// <param name="category">The category.</param>
            public MtCategory(string category)
            {
                categoryId = category;
                categoryName = category;
                isPrimary = false;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="MtCategory"/> class.
            /// </summary>
            /// <param name="id">The id.</param>
            /// <param name="category">The category.</param>
            public MtCategory(string id, string category)
            {
                categoryId = id;
                categoryName = category;
                isPrimary = false;
            }
        }

        /// <summary>
        /// Represents a text filter returned by mt.supportedTextFilters.
        /// </summary>
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public struct MtTextFilter
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MtTextFilter"/> class.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="label">The label.</param>
            public MtTextFilter(string key, string label)
            {
                this.key = key;
                this.label = label;
            }
            public string key;
            public string label;
        }

        [XmlRpcMethod("mt.getCategoryList",
             Description = "Gets a list of active categories for a given blog as an array of MT category struct.")]
        public MtCategory[] GetCategoryList(string blogid, string username, string password)
        {
            //ValidateUser(username, password, Config.CurrentBlog.AllowServiceAccess);

            //ICollection<LinkCategory> lcc = Links.GetCategories(CategoryType.PostCollection, ActiveFilter.None);
            //if (lcc == null)
            //{
            //    throw new XmlRpcFaultException(0, "No categories exist");
            //}

            //MtCategory[] categories = new MtCategory[lcc.Count];
            //int i = 0;
            //foreach (LinkCategory linkCategory in lcc)
            //{
            //    MtCategory _category = new MtCategory(linkCategory.Id.ToString(CultureInfo.InvariantCulture), linkCategory.Title);
            //    categories[i] = _category;
            //    i++;
            //}
            //return categories;

            return null;
        }

        [XmlRpcMethod("mt.setPostCategories",
            Description = "Sets the categories for a given post.")]
        public bool SetPostCategories(string postid, string username, string password,
            MtCategory[] categories)
        {
            //ValidateUser(username, password, Config.CurrentBlog.AllowServiceAccess);

            //if (categories != null && categories.Length > 0)
            //{
            //    int postID = Int32.Parse(postid);

            //    ArrayList al = new ArrayList();


            //    for (int i = 0; i < categories.Length; i++)
            //    {
            //        al.Add(Int32.Parse(categories[i].categoryId));
            //    }

            //    if (al.Count > 0)
            //    {
            //        Entries.SetEntryCategoryList(postID, (int[])al.ToArray(typeof(int)));
            //    }
            //}

            return true;
        }

        [XmlRpcMethod("mt.getPostCategories",
             Description = "Sets the categories for a given post.")]
        public MtCategory[] GetPostCategories(string postid, string username, string password)
        {
            //ValidateUser(username, password, Config.CurrentBlog.AllowServiceAccess);

            //int postID = Int32.Parse(postid);
            //ICollection<Link> postCategories = Links.GetLinkCollectionByPostID(postID);
            //MtCategory[] categories = new MtCategory[postCategories.Count];
            //if (postCategories.Count > 0)
            //{
            //    // REFACTOR: Might prefer seeing a dictionary come back straight from the provider.
            //    // for now we'll build our own catid->catTitle lookup--we need it below bc collection
            //    // from post is going to be null for title.
            //    ICollection<LinkCategory> cats = Links.GetCategories(CategoryType.PostCollection, ActiveFilter.None);
            //    Hashtable catLookup = new Hashtable(cats.Count);
            //    foreach (LinkCategory currentCat in cats)
            //        catLookup.Add(currentCat.Id, currentCat.Title);

            //    int i = 0;
            //    foreach (Link link in postCategories)
            //    {
            //        MtCategory _category = new MtCategory(link.CategoryID.ToString(CultureInfo.InvariantCulture), (string)catLookup[link.CategoryID]);

            //        categories[i] = _category;
            //        i++;
            //    }
            //}

            //return categories;

            return null;
        }

        /// <summary>
        /// Retrieve information about the text formatting plugins supported by the server.
        /// </summary>
        /// <returns>
        /// an array of structs containing String key and String label. 
        /// key is the unique string identifying a text formatting plugin, 
        /// and label is the readable description to be displayed to a user. 
        /// key is the value that should be passed in the mt_convert_breaks 
        /// parameter to newPost and editPost.
        /// </returns>
        [XmlRpcMethod("mt.supportedTextFilters",
             Description = "Retrieve information about the text formatting plugins supported by the server.")]
        public MtTextFilter[] GetSupportedTextFilters()
        {
            return new MtTextFilter[] { new MtTextFilter("test", "test"), };
        }
        #endregion
    }
}
