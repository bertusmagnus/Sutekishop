using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NUnit.Framework;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.TestHelpers;
using Suteki.Common.Validation;
using Suteki.Shop.Controllers;
using Suteki.Shop.ViewData;
using System.Collections.Specialized;
using System.Threading;
using System.Security.Principal;
using System.Web.Mvc;
using Rhino.Mocks;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class CmsControllerTests
    {
        private CmsController cmsController;

        private IRepository<Content> contentRepository;
        private IOrderableService<Content> contentOrderableService;
        private IValidatingBinder validatingBinder;

        [SetUp]
        public void SetUp()
        {
            // you have to be an administrator to access the CMS controller
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("admin"), new string[] { "Administrator" });

            contentRepository = MockRepository.GenerateStub<IRepository<Content>>();
            contentOrderableService = MockRepository.GenerateStub<IOrderableService<Content>>();
            validatingBinder = new ValidatingBinder(new SimplePropertyBinder());

            cmsController = new CmsController(
                contentRepository, 
                contentOrderableService,
                validatingBinder);
        }

        [Test]
        public void Index_ShouldRenderIndexViewWithContent()
        {
            const string urlName = "home";

            var contents = new List<Content>
            {
                new TextContent { UrlName = "Home" },
                new ActionContent { Name = "Help Pages" }
            }.AsQueryable();

            contentRepository.Expect(cr => cr.GetAll()).Return(contents);

            cmsController.Index(urlName)
                .ReturnsViewResult()
                .ForView("SubPage")
                .WithModel<CmsViewData>()
                .AssertAreSame(
                    contents.OfType<ITextContent>().First(), 
                    vd => vd.TextContent);
        }

        [Test]
        public void Index_ShouldRenderTopContentWithTopPageView()
        {
            const string urlName = "home_page";

            var contents = new List<Content>
            {
                new TopContent { UrlName = "home_page" }
            }.AsQueryable();

            contentRepository.Expect(cr => cr.GetAll()).Return(contents);

            cmsController.Index(urlName)
                .ReturnsViewResult()
                .ForView("TopPage")
                .WithModel<CmsViewData>()
                .AssertAreSame(
                    contents.OfType<ITextContent>().First(), vd => vd.TextContent);

        }

        [Test]
        public void Add_ShouldShowContentEditViewWithDefaultContent()
        {
            const int menuId = 1;

            var menu = new Menu {ContentId = menuId};
            contentRepository.Expect(cr => cr.GetById(menuId)).Return(menu);

            var menus = new List<Content>().AsQueryable();
            contentRepository.Expect(cr => cr.GetAll()).Return(menus);

            cmsController.Add(menuId)
                .ReturnsViewResult()
                .ForView("Edit")
                .WithModel<CmsViewData>()
                .AssertNotNull(vd => vd.TextContent)
                .AssertAreEqual(menuId, vd => vd.Content.ParentContentId.Value);
        }

		[Test]
		public void AddWithPost_ShouldAddNewContent()
		{
			var content = new TextContent() { ParentContentId = 4 };
			cmsController.Add(content)
				.ReturnsRedirectToRouteResult()
				.ToController("Menu")
				.ToAction("List")
				.WithRouteValue("id", "4");

			contentRepository.AssertWasCalled(x => x.InsertOnSubmit(content));
		}


    	[Test]
    	public void AddWithPost_ShouldRenderViewWithError()
    	{
			contentRepository.Stub(cr => cr.GetAll()).Return(new List<Content>().AsQueryable());
    		cmsController.ModelState.AddModelError("foo", "bar");

    		var content = new TextContent();
			cmsController.Add(content)
				.ReturnsViewResult()
				.ForView("Edit")
				.WithModel<CmsViewData>()
				.AssertAreSame(content, x => x.Content);
    	}

    	[Test]
        public void Edit_ShouldDisplayEditViewWithExistingContent()
        {
            const int contentId = 22;

            var content = new TextContent { ContentId = contentId };
            contentRepository.Stub(cr => cr.GetById(contentId)).Return(content);

            var menus = new List<Content>().AsQueryable();
            contentRepository.Stub(cr => cr.GetAll()).Return(menus);

            cmsController.Edit(contentId)
                .ReturnsViewResult()
                .ForView("Edit")
                .WithModel<CmsViewData>()
                .AssertAreEqual(contentId, vd => vd.Content.ContentId)
                .AssertNotNull(vd => vd.Menus);
        }

    	[Test]
    	public void EditWithPost_ShouldRenderViewWithError()
    	{
			contentRepository.Stub(cr => cr.GetAll()).Return(new List<Content>().AsQueryable());
			cmsController.ModelState.AddModelError("foo", "bar");

			var content = new TextContent();
			cmsController.Edit(content)
				.ReturnsViewResult()
				.WithModel<CmsViewData>()
				.AssertAreSame(content, x => x.Content);
    	}

    	[Test]
    	public void EditWithPost_ShouldRedirectOnSuccessfulBinding()
    	{
    		var content = new TextContent {ParentContentId = 4};

			cmsController.Add(content)
				.ReturnsRedirectToRouteResult()
				.ToController("Menu")
				.ToAction("List")
				.WithRouteValue("id", "4");
    	}

    	[Test]
    	public void EditWithPost_should_work_for_TopContent()
    	{
			cmsController.Edit(new TopContent() { ParentContentId = 4});
    	}

    	[Test]
    	public void EditWithPost_ShouldAllowHtmlText()
    	{
			var content = new TextContent() { Text = HttpUtility.HtmlEncode("<script>"), ParentContentId = 4 };
			cmsController.Edit(content);
			content.Text.ShouldEqual("<script>");
    	}

		[Test]
		public void AddWithPost_ShouldAllowHtmlText() 
		{
			var content = new TextContent() { Text = HttpUtility.HtmlEncode("<script>"), ParentContentId = 4 };
			cmsController.Add(content);
			content.Text.ShouldEqual("<script>");
		}
    }

    public static class CreateFormExtensions
    {
        public static FormCollection ForTextContent(this FormCollection form)
        {
            form.Add("contenttypeid", ContentType.TextContentId.ToString());
            form.Add("text", "some content text");
            return form;
        }

        public static FormCollection ForMenuContent(this FormCollection form)
        {
            form.Add("contenttypeid", ContentType.MenuId.ToString());
            return form;
        }
    }
}
