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
        /// <summary>
        /// Enable the port number to be run by your username, if not running as admin:
        /// netsh http add urlacl url=http://+:27198/MetaWeblogTest.svc user=DOMAIN\user
        /// </summary>
        [Test, Explicit("Make sure you have access the correct namespace reservation before running this test")]
        public void Should_be_able_to_communicate_using_XmlRpc()
        {
//            var contentRepository = MockRepository.GenerateStub<IRepository<Content>>();
//            var contentOrderableService = MockRepository.GenerateStub<IOrderableService<Content>>();
//            var baseControllerService = MockRepository.GenerateStub<IBaseControllerService>();
//            var imageFileService = MockRepository.GenerateStub<IImageFileService>();
//            var userService = MockRepository.GenerateStub<IUserService>();
//
//            var metaWebLog = new MetaWeblog(
//                contentRepository,
//                contentOrderableService,
//                baseControllerService,
//                imageFileService,
//                userService);
//
//            var theSiteUrl = "http://my.excellent.shop/";
//            var url = "http://localhost:27198/MetaWeblogTest.svc";
//
//            baseControllerService.Stub(s => s.SiteUrl).Return(theSiteUrl);
//            userService.Stub(s => s.Authenticate(Arg<string>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
//            var user = new User
//            {
//                RoleId = Role.AdministratorId
//            };
//            userService.Stub(s => s.CurrentUser).Return(user);
//
//            using (new WindsorContainer()
//                .AddFacility<WcfFacility>()
//                .Register(
//                    Component.For<XmlRpcEndpointBehavior>(),
//                    Component.For<IMetaWeblog>().Instance(metaWebLog)
//                        .ActAs(new DefaultServiceModel()
//                            .AddBaseAddresses(url)
//                            .AddEndpoints(
//                                WcfEndpoint.ForContract<IMetaWeblog>()
//                                .BoundTo(new XmlRpcHttpBinding())
//                            )
//
//                        )
//                    )
//                )
//            {
//                var factory = new XmlRpcChannelFactory<IMetaWeblog>(new XmlRpcHttpBinding(), new EndpointAddress(url));
//                var client = factory.CreateChannel();
//
//                var posts = client.getUsersBlogs("", "mike", "m1ke");
//
//                Assert.That(posts.Length, Is.EqualTo(1));
//                Assert.That(posts[0].url, Is.EqualTo(theSiteUrl));
//
//
//            }
        }
        
    }
}