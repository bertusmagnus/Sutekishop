using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Moq;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.Validation;
using Suteki.Shop.Controllers;
using Suteki.Shop.Tests.TestHelpers;
using Suteki.Shop.ViewData;
using System.Collections.Specialized;
using System.Threading;
using System.Security.Principal;
using System.Web.Mvc;

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

            contentRepository = new Mock<IRepository<Content>>().Object;
            contentOrderableService = new Mock<IOrderableService<Content>>().Object;
            validatingBinder = new ValidatingBinder(new SimplePropertyBinder());

            cmsController = new Mock<CmsController>(
                contentRepository, 
                contentOrderableService,
                validatingBinder).Object;
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

            Mock.Get(contentRepository).Expect(cr => cr.GetAll()).Returns(contents);

            cmsController.Index(urlName)
                .ReturnsViewResult()
                .ForView("SubPage")
                .AssertAreSame<CmsViewData, ITextContent>(
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

            Mock.Get(contentRepository).Expect(cr => cr.GetAll()).Returns(contents);

            cmsController.Index(urlName)
                .ReturnsViewResult()
                .ForView("TopPage")
                .AssertAreSame<CmsViewData, ITextContent>(
                    contents.OfType<ITextContent>().First(), vd => vd.TextContent);

        }

        [Test]
        public void Add_ShouldShowContentEditViewWithDefaultContent()
        {
            const int menuId = 1;

            var menu = new Menu {ContentId = menuId};
            Mock.Get(contentRepository).Expect(cr => cr.GetById(menuId)).Returns(menu);

            var menus = new List<Content>().AsQueryable();
            Mock.Get(contentRepository).Expect(cr => cr.GetAll()).Returns(menus);

            cmsController.Add(menuId)
                .ReturnsViewResult()
                .ForView("Edit")
                .AssertNotNull<CmsViewData, ITextContent>(vd => vd.TextContent)
                .AssertAreEqual<CmsViewData, int>(menuId, vd => vd.Content.ParentContentId.Value);
        }

        [Test]
        public void Edit_ShouldDisplayEditViewWithExistingContent()
        {
            const int contentId = 22;

            var content = new TextContent { ContentId = contentId };
            Mock.Get(contentRepository).Expect(cr => cr.GetById(contentId)).Returns(content).Verifiable();

            var menus = new List<Content>().AsQueryable();
            Mock.Get(contentRepository).Expect(cr => cr.GetAll()).Returns(menus);

            cmsController.Edit(contentId)
                .ReturnsViewResult()
                .ForView("Edit")
                .AssertAreEqual<CmsViewData, int>(contentId, vd => vd.Content.ContentId)
                .AssertNotNull<CmsViewData, IEnumerable<Menu>>(vd => vd.Menus);

            Mock.Get(contentRepository).Verify();
        }

        [Test]
        public void Update_ShouldAddNewContent()
        {
            const int contentId = 0;
            const int menuId = 1;

            var form = CreateContentEditForm(menuId).ForTextContent();

            TextContent textContent = null;

            Mock.Get(contentRepository).Expect(cr => cr.InsertOnSubmit(It.IsAny<Content>()))
                .Callback<Content>(content => textContent = content as TextContent).Verifiable();
            Mock.Get(contentRepository).Expect(cr => cr.SubmitChanges()).Verifiable();

            TestUpdateAction(contentId, menuId, form).ForText(form);

            Assert.That(textContent, Is.Not.Null, "textContent is null");
            Assert.That(menuId, Is.EqualTo(menuId));
            Assert.That(textContent.Name, Is.EqualTo(form["name"]));
            Assert.That(textContent.Text, Is.EqualTo(form["text"]));

            Mock.Get(contentRepository).Verify();
        }

        [Test]
        public void Update_ShouldAllowHtmlText()
        {
            const int contentId = 0;
            const int menuId = 1;

            var form = CreateContentEditForm(menuId).ForTextContent();
            form["text"] = "<script></script>";

            TextContent textContent = null;

            Mock.Get(contentRepository).Expect(cr => cr.InsertOnSubmit(It.IsAny<Content>()))
                .Callback<Content>(content => textContent = content as TextContent).Verifiable();
            Mock.Get(contentRepository).Expect(cr => cr.SubmitChanges()).Verifiable();

            TestUpdateAction(contentId, menuId, form).ForText(form);

            Assert.That(textContent, Is.Not.Null, "textContent is null");
            Assert.That(menuId, Is.EqualTo(menuId));
            Assert.That(textContent.Name, Is.EqualTo(form["name"]));
            Assert.That(textContent.Text, Is.EqualTo(form["text"]));
            Console.WriteLine(textContent.Text);
            Mock.Get(contentRepository).Verify();
        }

        [Test]
        public void Update_ShouldAddNewMenu()
        {
            const int contentId = 0;
            const int menuId = 1;

            var form = CreateContentEditForm(menuId).ForMenuContent();

            Menu menu = null;

            Mock.Get(contentRepository).Expect(cr => cr.InsertOnSubmit(It.IsAny<Content>()))
                .Callback<Content>(content => menu = content as Menu).Verifiable();
            Mock.Get(contentRepository).Expect(cr => cr.SubmitChanges()).Verifiable();

            TestUpdateAction(contentId, menuId, form);

            Assert.That(menu, Is.Not.Null, "textContent is null");
            Assert.That(menuId, Is.EqualTo(menuId));
            Assert.That(menu.Name, Is.EqualTo(form["name"]));

            Mock.Get(contentRepository).Verify();
        }

        private NameValueCollection CreateContentEditForm(int menuId)
        {
            var form = new NameValueCollection
            {
                {"id", "0"},
                {"parentcontentid", menuId.ToString()},
                {"name", "myNewContent"}
            };
            Mock.Get(cmsController).ExpectGet(c => c.Form).Returns(form);
            return form;
        }

        private ViewResult TestUpdateAction(int contentId, int menuId, NameValueCollection form)
        {
            return cmsController.Update(contentId)
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

            Mock.Get(contentRepository).Expect(cr => cr.GetById(contentId)).Returns(content).Verifiable();
            Mock.Get(contentRepository).Expect(cr => cr.SubmitChanges()).Verifiable();

            TestUpdateAction(contentId, menuId, form).ForText(form);

            Mock.Get(contentRepository).Verify();
        }

        [Test]
        public void List_ShouldShowListOfExistingContent()
        {
            var mainMenu = new Menu();
            Mock.Get(contentRepository).Expect(mr => mr.GetById(1)).Returns(mainMenu);            

            cmsController.List(1)
                .ReturnsViewResult()
                .ForView("List")
                .AssertAreSame<CmsViewData, Menu>(mainMenu, vd => vd.Menu);
        }

        [Test]
        public void NewMenu_ShouldShowMenuEditView()
        {
            const int parentContentId = 1;

            var mainMenu = new Menu{ ContentId = parentContentId };
            Mock.Get(contentRepository).Expect(mr => mr.GetById(parentContentId)).Returns(mainMenu);

            var menus = new List<Content>().AsQueryable();
            Mock.Get(contentRepository).Expect(cr => cr.GetAll()).Returns(menus);

            cmsController.NewMenu(parentContentId)
                .ReturnsViewResult()
                .ForView("Edit")
                .AssertNotNull<CmsViewData, Content>(vd => vd.Menu)
                .AssertAreEqual<CmsViewData, int>(parentContentId, vd => vd.Menu.ParentContentId.Value);
        }
    }

    public static class CreateFormExtensions
    {
        public static NameValueCollection ForTextContent(this NameValueCollection form)
        {
            form.Add("contenttypeid", ContentType.TextContentId.ToString());
            form.Add("text", "some content text");
            return form;
        }

        public static NameValueCollection ForMenuContent(this NameValueCollection form)
        {
            form.Add("contenttypeid", ContentType.MenuId.ToString());
            return form;
        }
    }

    public static class ViewResultExtensionsForCmsTests
    {
        public static ViewResult ForText(this ViewResult ViewResult, NameValueCollection form)
        {
            return ViewResult;
                //.AssertAreEqual<CmsViewData, string>(form["text"], vd => vd.TextContent.Text);
        }
    }
}
