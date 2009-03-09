using System;
using System.Collections.Generic;
using System.Linq;
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
        public void Update_ShouldAddNewContent()
        {
            const int contentId = 0;
            const int menuId = 1;

            var form = CreateContentEditForm(menuId).ForTextContent();

            TextContent textContent = null;

            contentRepository.Expect(cr => cr.InsertOnSubmit(null))
                .IgnoreArguments()
                .WhenCalled(invocation => textContent = invocation.Arguments[0] as TextContent);

            TestUpdateAction(contentId, form);

            Assert.That(textContent, Is.Not.Null, "textContent is null");
            Assert.That(menuId, Is.EqualTo(menuId));
            Assert.That(textContent.Name, Is.EqualTo(form["name"]));
            Assert.That(textContent.Text, Is.EqualTo(form["text"]));

            contentRepository.AssertWasCalled(cr => cr.SubmitChanges());
        }

        [Test]
        public void Update_ShouldAllowHtmlText()
        {
            const int contentId = 0;
            const int menuId = 1;

            var form = CreateContentEditForm(menuId).ForTextContent();
            form["text"] = "<script></script>";

            TextContent textContent = null;

            contentRepository.Expect(cr => cr.InsertOnSubmit(null))
                .IgnoreArguments()
                .WhenCalled(invocation => textContent = invocation.Arguments[0] as TextContent);

            TestUpdateAction(contentId, form);

            Assert.That(textContent, Is.Not.Null, "textContent is null");
            Assert.That(menuId, Is.EqualTo(menuId));
            Assert.That(textContent.Name, Is.EqualTo(form["name"]));
            Assert.That(textContent.Text, Is.EqualTo(form["text"]));
            Console.WriteLine(textContent.Text);

            contentRepository.AssertWasCalled(cr => cr.SubmitChanges());
        }

        [Test]
        public void Update_ShouldAddNewMenu()
        {
            const int contentId = 0;
            const int menuId = 1;

            var form = CreateContentEditForm(menuId).ForMenuContent();

            Menu menu = null;

            contentRepository.Expect(cr => cr.InsertOnSubmit(null))
                .IgnoreArguments()
                .WhenCalled(invocation => menu = invocation.Arguments[0] as Menu);

            TestUpdateAction(contentId, form);

            Assert.That(menu, Is.Not.Null, "textContent is null");
            Assert.That(menuId, Is.EqualTo(menuId));
            Assert.That(menu.Name, Is.EqualTo(form["name"]));

            contentRepository.AssertWasCalled(cr => cr.SubmitChanges());
        }

        private static FormCollection CreateContentEditForm(int menuId)
        {
            var form = new FormCollection
            {
                {"id", "0"},
                {"parentcontentid", menuId.ToString()},
                {"name", "myNewContent"}
            };
            return form;
        }

        private void TestUpdateAction(int contentId, FormCollection form)
        {
            cmsController.Update(contentId, form)
                .ReturnsViewResult()
                .ForView("List");
        }

        [Test]
        public void Update_ShouldUpdateAnExistingTextContent()
        {
            const int contentId = 22;
            const int menuId = 1;

            var form = CreateContentEditForm(menuId).ForTextContent();

            var content = new TextContent
            {
                Name = "old name",
                Text = "old text"
            };

            contentRepository.Stub(cr => cr.GetById(contentId)).Return(content);

            TestUpdateAction(contentId, form);

            contentRepository.AssertWasCalled(cr => cr.SubmitChanges());
        }

        [Test]
        public void List_ShouldShowListOfExistingContent()
        {
            var mainMenu = new Menu();
            contentRepository.Expect(mr => mr.GetById(1)).Return(mainMenu);            

            cmsController.List(1)
                .ReturnsViewResult()
                .ForView("List")
                .WithModel<CmsViewData>()
                .AssertAreSame(mainMenu, vd => vd.Menu);
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
