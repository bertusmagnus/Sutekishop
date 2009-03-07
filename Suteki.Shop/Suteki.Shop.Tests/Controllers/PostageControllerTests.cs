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
using Suteki.Shop.ViewData;

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
    	public void NewWithPost_ShouldAddNewPostage()
    	{
    		var postage = new Postage() { MaxWeight = 250, Price = (decimal)5.25, Name = "foo"};
    		postageController.New(postage)
    			.ReturnRedirectToRouteResult()
    			.ToAction("Index");

			postageRepository.AssertWasCalled(x=>x.InsertOnSubmit(postage));
    	}

    	[Test]
    	public void NewWithPost_ShouldRenderViewOnError()
    	{
    		postageController.ModelState.AddModelError("foo", "bar");
    		postageController.New(new Postage())
    			.ReturnsViewResult()
    			.ForView("Edit");

			postageRepository.AssertWasNotCalled(x=>x.InsertOnSubmit(Arg<Postage>.Is.Anything));
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
    	public void EditWithPost_ShouldRenderViewOnSuccessfulSave()
    	{
    		var postage = new Postage();
    		postageController.Edit(postage)
    			.ReturnsViewResult()
    			.WithModel<ScaffoldViewData<Postage>>()
    			.AssertNotNull(x => x.Message)
				.AssertAreSame(postage,x=>x.Item);
    	}

    	[Test]
    	public void EditWithPost_ShouldRenderViewOnError()
    	{
    		postageController.ModelState.AddModelError("foo", "Bar");
    		var postage = new Postage();
    		postageController.Edit(postage)
    			.ReturnsViewResult()
    			.WithModel<ScaffoldViewData<Postage>>()
    			.AssertNull(x => x.Message)
    			.AssertAreSame(postage, x => x.Item);
    	}

        [Test]
        public void MoveUp_ShouldMoveItemUp()
        {
            const int position = 4;

            var orderResult = MockRepository.GenerateMock<IOrderServiceWithPosition<Postage>>();

            orderableService.Stub(os => os.MoveItemAtPosition(position)).Return(orderResult);

            var postages = new List<Postage>();
            postageRepository.Expect(pr => pr.GetAll()).Return(postages.AsQueryable());

            postageController.MoveUp(position, 1);

            orderResult.AssertWasCalled(or => or.UpOne());
        }
    }
}
