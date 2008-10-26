using System.Linq;
using NUnit.Framework;
using Moq;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.ViewData;
using Suteki.Shop.Controllers;
using System.Collections.Generic;
using System.Collections.Specialized;
using Suteki.Shop.Tests.TestHelpers;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class PostageControllerTests
    {
        PostageController postageController;

        IRepository<Postage> postageRepository;
        IOrderableService<Postage> orderableService;
        ControllerTestContext testContext;

        [SetUp]
        public void SetUp()
        {
            postageRepository = new Mock<IRepository<Postage>>().Object;
            orderableService = new Mock<IOrderableService<Postage>>().Object;

            postageController = new PostageController();
            postageController.Repository = postageRepository;
            postageController.OrderableService = orderableService;

            testContext = new ControllerTestContext(postageController);
        }

        [Test]
        public void Index_ShouldShowListOfPostages()
        {
            List<Postage> postages = new List<Postage>();
            Mock.Get(postageRepository).Expect(pr => pr.GetAll()).Returns(postages.AsQueryable());

            postageController.Index()
                .ReturnsViewResult()
                .ForView("Index")
                .AssertNotNull<ScaffoldViewData<Postage>, IEnumerable<Postage>>(vd => vd.Items);
        }

        [Test]
        public void New_ShouldShowEditViewWithNewPostage()
        {
            postageController.New()
                .ReturnsViewResult()
                .ForView("Edit")
                .AssertNotNull<ScaffoldViewData<Postage>, Postage>(vd => vd.Item);
        }

        [Test]
        public void Edit_ShouldShowEditViewWithExistingPostage()
        {
            int postageId = 3;
            Postage postage = new Postage { PostageId = postageId };

            Mock.Get(postageRepository).Expect(pr => pr.GetById(postageId)).Returns(postage);

            postageController.Edit(postageId)
                .ReturnsViewResult()
                .ForView("Edit")
                .AssertAreSame<ScaffoldViewData<Postage>, Postage>(postage, vd => vd.Item);
        }

        [Test]
        public void Update_ShouldAddNewPostage()
        {
            NameValueCollection form = BuildMockPostageForm();
            form.Add("postageid", "0");

            Postage postage = null;

            Mock.Get(postageRepository).Expect(pr => pr.InsertOnSubmit(It.IsAny<Postage>()))
                .Callback<Postage>(p => { postage = p; })
                .Verifiable();
            Mock.Get(postageRepository).Expect(pr => pr.SubmitChanges()).Verifiable();

            postageController.Update()
                .ReturnsViewResult()
                .ForView("Index");

            Assert.AreEqual(form["name"], postage.Name);
            Assert.AreEqual(form["maxweight"], postage.MaxWeight.ToString());
            Assert.AreEqual(form["price"], postage.Price.ToString());

            Mock.Get(postageRepository).Verify();
        }

        private NameValueCollection BuildMockPostageForm()
        {
            NameValueCollection form = new NameValueCollection();
            form.Add("name", "A");
            form.Add("maxWeight", "250");
            form.Add("price", "5.25");
            testContext.TestContext.RequestMock.ExpectGet(r => r.Form).Returns(() => form);
            return form;
        }

        [Test]
        public void Update_ShouldUpdateExistingPostage()
        {
            int postageId = 4;
            NameValueCollection form = BuildMockPostageForm();
            form.Add("postageid", postageId.ToString());

            Postage postage = new Postage 
            {
                PostageId = postageId,
                Name = "old name",
                MaxWeight = 100,
                Price = 2.23M
            };

            Mock.Get(postageRepository).Expect(pr => pr.GetById(postageId)).Returns(postage).Verifiable();
            Mock.Get(postageRepository).Expect(pr => pr.SubmitChanges()).Verifiable();

            postageController.Update()
                .ReturnsViewResult()
                .ForView("Index");

            Assert.AreEqual(form["name"], postage.Name);
            Assert.AreEqual(form["maxweight"], postage.MaxWeight.ToString());
            Assert.AreEqual(form["price"], postage.Price.ToString());

            Mock.Get(postageRepository).Verify();
        }

        [Test]
        public void MoveUp_ShouldMoveItemUp()
        {
            int position = 4;

            IOrderServiceWithPosition<Postage> orderResult = new Mock<IOrderServiceWithPosition<Postage>>().Object;

            Mock.Get(orderableService).Expect(os => os.MoveItemAtPosition(position))
                .Returns(orderResult).Verifiable();
            Mock.Get(orderResult).Expect(or => or.UpOne()).Verifiable();

            List<Postage> postages = new List<Postage>();
            Mock.Get(postageRepository).Expect(pr => pr.GetAll()).Returns(postages.AsQueryable());

            postageController.MoveUp(position);

            Mock.Get(orderableService).Verify();
        }
    }
}
