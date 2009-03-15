using System.ServiceModel;
using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.ServiceModel.Samples.XmlRpc;
using NUnit.Framework;
using Rhino.Mocks;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Shop.Services;
using Suteki.Shop.XmlRpc;

namespace Suteki.Shop.Tests.XmlRpc
{
    [TestFixture]
    public class MetaWeblogWcfSpike
    {
        private MetaWeblogWcf metaWeblog;

        private IRepository<Content> contentRepository;
        private IOrderableService<Content> contentOrderableService;
        private IBaseControllerService baseControllerService;
        private IImageFileService imageFileService;
        private IUserService userService;
        private IWindsorContainer container;

        private IMetaWeblog client;

        private const string theSiteUrl = "http://my.excellent.shop/";

        [SetUp]
        public void SetUp()
        {
            contentRepository = MockRepository.GenerateStub<IRepository<Content>>();
            contentOrderableService = MockRepository.GenerateStub<IOrderableService<Content>>();
            baseControllerService = MockRepository.GenerateStub<IBaseControllerService>();
            imageFileService = MockRepository.GenerateStub<IImageFileService>();
            userService = MockRepository.GenerateStub<IUserService>();

            metaWeblog = new MetaWeblogWcf(
                userService,
                contentRepository,
                baseControllerService,
                contentOrderableService,
                imageFileService);

            var url = "http://localhost:27198/MetaWeblogTest.svc";

            baseControllerService.Stub(s => s.SiteUrl).Return(theSiteUrl);
            userService.Stub(s => s.Authenticate(Arg<string>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var user = new User
            {
                RoleId = Role.AdministratorId
            };
            userService.Stub(s => s.CurrentUser).Return(user);


            container = new WindsorContainer()
                .AddFacility<WcfFacility>()
                .Register(
                    Component.For<XmlRpcEndpointBehavior>(),
                    Component.For<IMetaWeblog>().Instance(metaWeblog)
                        .ActAs(new DefaultServiceModel()
                            .AddBaseAddresses(url)
                            .AddEndpoints(
                                WcfEndpoint.ForContract<IMetaWeblog>()
                                .BoundTo(new XmlRpcHttpBinding())
                            )

                        )
                    );

            var factory = new XmlRpcChannelFactory<IMetaWeblog>(new XmlRpcHttpBinding(), new EndpointAddress(url));
            client = factory.CreateChannel();
        }

        [TearDown]
        public void TearDown()
        {
            container.Dispose();
        }

        /// <summary>
        /// Enable the port number to be run by your username, if not running as admin:
        /// netsh http add urlacl url=http://+:27198/MetaWeblogTest.svc user=DOMAIN\user
        /// </summary>
        [Test, Explicit("Make sure you have access the correct namespace reservation before running this test")]
        public void Should_be_able_to_get_blogs()
        {
            var posts = client.getUsersBlogs("", "mike", "m1ke");

            Assert.That(posts.Length, Is.EqualTo(1));
            Assert.That(posts[0].url, Is.EqualTo(theSiteUrl));
        }

        [Test, Explicit("Make sure you have access the correct namespace reservation before running this test")]
        public void Should_be_able_to_add_a_post()
        {
            Content content = null;

            contentRepository.Expect(r => r.InsertOnSubmit(null)).Callback((Content arg1) =>
            {
                content = arg1;
                return true;
            });

            var post = new Post
            {
                title = "the title",
                description = "the description"
            };

            client.newPost("1", "mike", "m1ke", post, true);

            var textContent = content as TextContent;
            if(textContent == null) Assert.Fail("content should be an instance of TextContent");

            Assert.That(textContent.Name, Is.EqualTo(post.title));
            Assert.That(textContent.Text, Is.EqualTo(post.description));
        }
    }
}