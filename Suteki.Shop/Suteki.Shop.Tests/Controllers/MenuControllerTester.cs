using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.TestHelpers;
using Suteki.Shop.Controllers;
using Suteki.Shop.Tests.Repositories;
using Suteki.Shop.ViewData;

namespace Suteki.Shop.Tests.Controllers
{
	[TestFixture]
	public class MenuControllerTester
	{
		private MenuController controller;
		private IRepository<Menu> menuRepository;
		private IRepository<Category> categoryRepository;
		private IOrderableService<Content> orderableService;

		[SetUp]
		public void Setup()
		{
			categoryRepository = MockRepositoryBuilder.CreateCategoryRepository();
			menuRepository = MockRepository.GenerateStub<IRepository<Menu>>();
			orderableService = MockRepository.GenerateStub<IOrderableService<Content>>();
			controller = new MenuController(menuRepository, categoryRepository, orderableService);
		}

		[Test]
		public void Edit_should_render_view()
		{
			var menu = new Menu();
			menuRepository.Expect(x => x.GetAll()).Return(new List<Menu>().AsQueryable());
			menuRepository.Expect(x => x.GetById(3)).Return(menu);
			controller.Edit(3)
				.WithModel<CmsViewData>()
				.AssertAreSame(menu, x => x.Content);
				
		}

		[Test]
		public void New_should_render_view()
		{
			const int parentContentId = 1;

			var mainMenu = new Menu { ContentId = parentContentId };
			menuRepository.Expect(mr => mr.GetById(parentContentId)).Return(mainMenu);

			var menus = new List<Menu>().AsQueryable();
			menuRepository.Expect(cr => cr.GetAll()).Return(menus);

			controller.New(parentContentId)
				.ForView("Edit")
				.WithModel<CmsViewData>()
				.AssertNotNull<CmsViewData, Content>(vd => vd.Menu)
				.AssertAreEqual(parentContentId, vd => vd.Menu.ParentContentId.Value);

		}

		[Test]
		public void NewWithPost_should_save()
		{
			var menu = new Menu() { ParentContentId = 5 };

			controller.New(menu)
				.ReturnsRedirectToRouteResult()
				.ToController("Menu")
				.ToAction("List").WithRouteValue("id", menu.ParentContentId.ToString());


			menuRepository.AssertWasCalled(x => x.InsertOnSubmit(menu));

		}

		[Test]
		public void NewWithPost_should_render_edit_view_on_error()
		{
			controller.ModelState.AddModelError("foo", "bar");
			menuRepository.Expect(x => x.GetAll()).Return(new List<Menu>().AsQueryable());
			var menu = new Menu() { ParentContentId = 5 };
			controller.New(menu)
				.ReturnsViewResult()
				.ForView("Edit")
				.WithModel<CmsViewData>()
				.AssertAreSame(menu, x => x.Content);
		}

		[Test]
		public void List_ShouldShowListOfExistingContent() 
		{
			var mainMenu = new Menu();
			menuRepository.Expect(mr => mr.GetById(1)).Return(mainMenu);

			controller.List(1)
				.ReturnsViewResult()
				.WithModel<CmsViewData>()
				.AssertAreSame(mainMenu, vd => vd.Menu);
		}

	    [Test]
	    public void LeftMenu_should_return_tree_of_CategoryViewData()
	    {
	        var rootCategory = controller.LeftMenu()
	            .ReturnsViewResult()
	            .WithModel<CategoryViewData>();

	        Assert.That(rootCategory.Name, Is.EqualTo("root"));
            Assert.That(rootCategory.ChildCategories.Count, Is.EqualTo(2), "root has no children");

	        var one = rootCategory.ChildCategories[0];
	        var two = rootCategory.ChildCategories[1];
            Assert.That(one.Name, Is.EqualTo("one"));
            Assert.That(two.Name, Is.EqualTo("two"));

            Assert.That(one.ChildCategories.Count, Is.EqualTo(2), "one has no children");
	        var oneOne = one.ChildCategories[0];
	        var oneTwo = one.ChildCategories[1];
            Assert.That(oneOne.Name, Is.EqualTo("oneOne"));
            Assert.That(oneTwo.Name, Is.EqualTo("oneTwo"));

            Assert.That(oneTwo.ChildCategories.Count, Is.EqualTo(2), "oneTwo has no children");
	        var oneTwoOne = oneTwo.ChildCategories[0];
	        var oneTwoTwo = oneTwo.ChildCategories[1];
            Assert.That(oneTwoOne.Name, Is.EqualTo("oneTwoOne"));
            Assert.That(oneTwoTwo.Name, Is.EqualTo("oneTwoTwo"));
	    }
	}
}