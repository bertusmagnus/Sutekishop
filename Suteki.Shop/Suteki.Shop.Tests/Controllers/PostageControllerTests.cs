using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web.Mvc;
using NUnit.Framework;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.TestHelpers;
using Suteki.Common.Validation;
using Suteki.Common.ViewData;
using Suteki.Shop.Controllers;
using System.Collections.Generic;
using Rhino.Mocks;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class PostageControllerTests
    {
        private PostageController postageController;

        private IRepository<Postage> postageRepository;
        private IOrderableService<Postage> orderableService;
        private IValidatingBinder validatingBinder;
        private IHttpContextService httpContextService;

        [SetUp]
        public void SetUp()
        {
            // you have to be an administrator to access the CMS controller
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("admin"), new[] { "Administrator" });

            postageRepository = MockRepository.GenerateStub<IRepository<Postage>>();
            orderableService = MockRepository.GenerateStub<IOrderableService<Postage>>();
            validatingBinder = new ValidatingBinder(new SimplePropertyBinder());
            httpContextService = MockRepository.GenerateStub<IHttpContextService>();

            postageController = new PostageController
            {
                Repository = postageRepository,
                OrderableService = orderableService,
                ValidatingBinder = validatingBinder,
                httpContextService = httpContextService
            };
        }

        [Test]
        public void Index_ShouldShowListOfPostages()
        {
            var postages = new List<Postage>();
            postageRepository.Expect(pr => pr.GetAll()).Return(postages.AsQueryable());

            postageController.Index(1)
                .ReturnsViewResult()
                .ForView("Index")
                .WithModel<ScaffoldViewData<Postage>>()
                .AssertNotNull(vd => vd.Items);
        }

        [Test]
        public void New_ShouldShowEditViewWithNewPostage()
        {
            postageController.New()
                .ReturnsViewResult()
                .ForView("Edit")
                .WithModel<ScaffoldViewData<Postage>>()
                .AssertNotNull(vd => vd.Item);
        }

        [Test]
        public void Edit_ShouldShowEditViewWithExistingPostage()
        {
            var postageId = 3;
            var postage = new Postage { PostageId = postageId };

            postageRepository.Expect(pr => pr.GetById(postageId)).Return(postage);

            postageController.Edit(postageId)
                .ReturnsViewResult()
                .ForView("Edit")
                .WithModel<ScaffoldViewData<Postage>>()
                .AssertAreSame(postage, vd => vd.Item);
        }

        [Test]
        public void Update_ShouldAddNewPostage()
        {
            var form = BuildMockPostageForm();
            form.Add("postageid", "0");

            Postage postage = null;

            postageRepository.Expect(pr => pr.InsertOnSubmit(Arg<Postage>.Is.Anything))
                .WhenCalled(invocation => { postage = invocation.Arguments[0] as Postage; });
            postageRepository.Expect(pr => pr.SubmitChanges());
            httpContextService.Expect(hcs => hcs.FormOrQuerystring).Return(form);

            postageController.Update(form)
                .ReturnRedirectToRouteResult()
                .ToAction("Index");

            Assert.AreEqual(form["name"], postage.Name);
            Assert.AreEqual(form["maxweight"], postage.MaxWeight.ToString());
            Assert.AreEqual(form["price"], postage.Price.ToString());

            postageRepository.VerifyAllExpectations();
        }

        private static FormCollection BuildMockPostageForm()
        {
            var form = new FormCollection
            {
                {"name", "A"}, 
                {"maxWeight", "250"}, 
                {"price", "5.25"}
            };

            return form;
        }

        [Test]
        public void Update_ShouldUpdateExistingPostage()
        {
            const int postageId = 4;
            var form = BuildMockPostageForm();
            form.Add("postageid", postageId.ToString());

            var postage = new Postage 
            {
                PostageId = postageId,
                Name = "old name",
                MaxWeight = 100,
                Price = 2.23M
            };

            postageRepository.Expect(pr => pr.GetById(postageId)).Return(postage);
            postageRepository.Expect(pr => pr.SubmitChanges());
            httpContextService.Expect(hcs => hcs.FormOrQuerystring).Return(form);

            postageController.Update(form)
                .ReturnRedirectToRouteResult()
                .ToAction("Index");

            Assert.AreEqual(form["name"], postage.Name);
            Assert.AreEqual(form["maxweight"], postage.MaxWeight.ToString());
            Assert.AreEqual(form["price"], postage.Price.ToString());

            postageRepository.VerifyAllExpectations();
        }

        [Test]
        public void MoveUp_ShouldMoveItemUp()
        {
            const int position = 4;

            var orderResult = MockRepository.GenerateMock<IOrderServiceWithPosition<Postage>>();

            orderableService.Expect(os => os.MoveItemAtPosition(position)).Return(orderResult);
            orderResult.Expect(or => or.UpOne());

            var postages = new List<Postage>();
            postageRepository.Expect(pr => pr.GetAll()).Return(postages.AsQueryable());

            postageController.MoveUp(position, 1);

            orderableService.VerifyAllExpectations();
        }
    }
}
