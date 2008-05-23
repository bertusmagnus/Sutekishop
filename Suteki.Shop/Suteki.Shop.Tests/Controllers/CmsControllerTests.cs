using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Moq;
using Suteki.Shop.Controllers;
using Suteki.Shop.Repositories;
using Suteki.Shop.ViewData;
using System.Collections.Specialized;
using System.Threading;
using System.Security.Principal;
using Suteki.Shop.Services;
using System.Web.Mvc;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class CmsControllerTests
    {
        CmsController cmsController;

        IRepository<Content> contentRepository;
        IOrderableService<Content> contentOrderableService;

        [SetUp]
        public void SetUp()
        {
            // you have to be an administrator to access the CMS controller
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("admin"), new string[] { "Administrator" });

            contentRepository = new Mock<IRepository<Content>>().Object;
            contentOrderableService = new Mock<IOrderableService<Content>>().Object;

            cmsController = new Mock<CmsController>(
                contentRepository, 
                contentOrderableService).Object;
        }

        [Test]
        public void Index_ShouldRenderIndexViewWithContent()
        {
            string urlName = "home";

            var contents = new List<Content>
            {
                new TextContent { UrlName = "Online Shop" },
                new TextContent { UrlName = "Home" },
                new ActionContent { Name = "Help Pages" }
            }.AsQueryable();

            Mock.Get(contentRepository).Expect(cr => cr.GetAll()).Returns(contents);

            cmsController.Index(urlName)
                .ReturnsRenderViewResult()
                .ForView("Index")
                .AssertAreSame<CmsViewData, TextContent>(
                    contents.OfType<TextContent>().Single(c => c.UrlName == "Home"), 
                    vd => vd.TextContent);
        }

        [Test]
        public void Add_ShouldShowContentEditViewWithDefaultContent()
        {
            int menuId = 1;

            cmsController.Add(menuId)
                .ReturnsRenderViewResult()
                .ForView("Edit")
                .AssertNotNull<CmsViewData, TextContent>(vd => vd.TextContent)
                .AssertAreEqual<CmsViewData, int>(menuId, vd => vd.Content.ParentContentId.Value);
        }

        [Test]
        public void Edit_ShouldDisplayEditViewWithExistingContent()
        {
            int contentId = 22;

            TextContent content = new TextContent { ContentId = contentId };
            Mock.Get(contentRepository).Expect(cr => cr.GetById(contentId)).Returns(content).Verifiable();

            cmsController.Edit(contentId)
                .ReturnsRenderViewResult()
                .ForView("Edit")
                .AssertAreEqual<CmsViewData, int>(contentId, vd => vd.Content.ContentId);

            Mock.Get(contentRepository).Verify();
        }

        [Test]
        public void Update_ShouldAddNewContent()
        {
            int contentId = 0;
            int menuId = 1;

            NameValueCollection form = CreateContentEditForm(menuId).ForTextContent();

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
        public void Update_ShouldAddNewMenu()
        {
            int contentId = 0;
            int menuId = 1;

            NameValueCollection form = CreateContentEditForm(menuId).ForMenuContent();

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
            NameValueCollection form = new NameValueCollection();
            form.Add("id", "0");
            form.Add("parentcontentid", menuId.ToString());
            form.Add("name", "myNewContent");
            Mock.Get(cmsController).ExpectGet(c => c.Form).Returns(form);
            return form;
        }

        private RenderViewResult TestUpdateAction(int contentId, int menuId, NameValueCollection form)
        {
            return cmsController.Update(contentId)
                .ReturnsRenderViewResult()
                .ForView("List");
                //.AssertNotNull<CmsViewData, Content>(vd => vd.Content)
                //.AssertAreEqual<CmsViewData, string>(form["name"], vd => vd.Content.Name)
                //.AssertAreEqual<CmsViewData, int>(menuId, vd => vd.Content.ParentContentId.Value);
        }

        [Test]
        public void Update_ShouldUpdateAnExistingTextContent()
        {
            int contentId = 22;
            int menuId = 1;

            NameValueCollection form = CreateContentEditForm(menuId).ForTextContent();

            TextContent content = new TextContent
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
            Menu mainMenu = new Menu();

            Mock.Get(contentRepository).Expect(mr => mr.GetById(1)).Returns(mainMenu);            

            cmsController.List(1)
                .ReturnsRenderViewResult()
                .ForView("List")
                .AssertAreSame<CmsViewData, Menu>(mainMenu, vd => vd.Menu);
        }

        [Test]
        public void NewMenu_ShouldShowMenuEditView()
        {
            int parentContentId = 1;

            cmsController.NewMenu(parentContentId)
                .ReturnsRenderViewResult()
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

    public static class RenderViewResultExtensionsForCmsTests
    {
        public static RenderViewResult ForText(this RenderViewResult renderViewResult, NameValueCollection form)
        {
            return renderViewResult;
                //.AssertAreEqual<CmsViewData, string>(form["text"], vd => vd.TextContent.Text);
        }
    }
}
