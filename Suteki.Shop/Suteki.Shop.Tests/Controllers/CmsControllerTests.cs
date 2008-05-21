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

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class CmsControllerTests
    {
        CmsController cmsController;

        IRepository<Content> contentRepository;
        IRepository<Menu> menuRepository;
        IOrderableService<Content> contentOrderableService;

        [SetUp]
        public void SetUp()
        {
            // you have to be an administrator to access the CMS controller
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("admin"), new string[] { "Administrator" });

            contentRepository = new Mock<IRepository<Content>>().Object;
            menuRepository = new Mock<IRepository<Menu>>().Object;
            contentOrderableService = new Mock<IOrderableService<Content>>().Object;

            cmsController = new Mock<CmsController>(
                contentRepository, 
                menuRepository,
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
                .AssertAreEqual<CmsViewData, int>(menuId, vd => vd.TextContent.MenuId);
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
                .AssertAreEqual<CmsViewData, int>(contentId, vd => vd.TextContent.ContentId);

            Mock.Get(contentRepository).Verify();
        }

        [Test]
        public void Update_ShouldAddNewContent()
        {
            int contentId = 0;
            int menuId = 1;

            NameValueCollection form = CreateTextContentEditForm(menuId);

            TextContent textContent = null;

            Mock.Get(contentRepository).Expect(cr => cr.InsertOnSubmit(It.IsAny<Content>()))
                .Callback<Content>(content => textContent = content as TextContent).Verifiable();
            Mock.Get(contentRepository).Expect(cr => cr.SubmitChanges()).Verifiable();

            TestUpdateAction(contentId, menuId, form);

            Assert.That(textContent, Is.Not.Null, "textContent is null");
            Assert.That(menuId, Is.EqualTo(menuId));
            Assert.That(textContent.Name, Is.EqualTo(form["name"]));
            Assert.That(textContent.Text, Is.EqualTo(form["text"]));

            Mock.Get(contentRepository).Verify();
        }

        private NameValueCollection CreateTextContentEditForm(int menuId)
        {
            NameValueCollection form = new NameValueCollection();
            form.Add("id", "0");
            form.Add("menuid", menuId.ToString());
            form.Add("name", "myNewContent");
            form.Add("text", "some content text");
            Mock.Get(cmsController).ExpectGet(c => c.Form).Returns(form);
            return form;
        }

        private void TestUpdateAction(int contentId, int menuId, NameValueCollection form)
        {
            cmsController.Update(contentId)
                .ReturnsRenderViewResult()
                .ForView("Index")
                .AssertNotNull<CmsViewData, TextContent>(vd => vd.TextContent)
                .AssertAreEqual<CmsViewData, string>(form["name"], vd => vd.TextContent.Name)
                .AssertAreEqual<CmsViewData, string>(form["text"], vd => vd.TextContent.Text)
                .AssertAreEqual<CmsViewData, int>(menuId, vd => vd.TextContent.MenuId);
        }

        [Test]
        public void Update_ShouldUpdateAnExistingTextContent()
        {
            int contentId = 22;
            int menuId = 1;

            NameValueCollection form = CreateTextContentEditForm(menuId);

            TextContent content = new TextContent
            {
                Name = "old name",
                Text = "old text"
            };

            Mock.Get(contentRepository).Expect(cr => cr.GetById(contentId)).Returns(content).Verifiable();
            Mock.Get(contentRepository).Expect(cr => cr.SubmitChanges()).Verifiable();

            TestUpdateAction(contentId, menuId, form);

            Mock.Get(contentRepository).Verify();
        }

        [Test]
        public void List_ShouldShowListOfExistingContent()
        {
            Menu mainMenu = new Menu();

            Mock.Get(menuRepository).Expect(mr => mr.GetById(1)).Returns(mainMenu);            

            cmsController.List()
                .ReturnsRenderViewResult()
                .ForView("List")
                .AssertAreSame<CmsViewData, Menu>(mainMenu, vd => vd.Menu);
        }
    }
}
