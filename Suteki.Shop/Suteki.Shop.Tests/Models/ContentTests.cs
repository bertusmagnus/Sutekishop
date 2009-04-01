using NUnit.Framework;
using Suteki.Shop.Models.Exceptions;

namespace Suteki.Shop.Tests.Models
{
    [TestFixture]
    public class ContentTests
    {
        Menu mainMenu;
        Menu subMenu;

        [SetUp]
        public void SetUp()
        {
            mainMenu = new Menu
            {
                ContentId = Menu.MainMenuId
            };

            subMenu = new Menu
            {
                Content1 = mainMenu,
                ParentContentId = Menu.MainMenuId
            };
        }

        [Test]
        public void TextContent_SubMenu_ShouldReturnItsSubMenu()
        {
            TextContent textContent = new TextContent
            {
                Content1 = subMenu
            };

            Assert.That(textContent.SubMenu, Is.SameAs(subMenu));
        }

        [Test]
        public void TextContentWithoutSubMenu_SubMenu_ShouldReturnNull()
        {

            TextContent textContent = new TextContent
            {
                Content1 = mainMenu
            };

            Assert.That(textContent.SubMenu, Is.Null);
        }

        [Test]
        public void Menu_SubMenu_ShouldReturnItselfIfItIsASubMenu()
        {
            Assert.That(subMenu.SubMenu, Is.SameAs(subMenu));
        }

        [Test]
        public void MainMenu_SubMenu_ShouldReturnNull()
        {
            Assert.That(mainMenu.SubMenu, Is.Null);
        }

        [Test]
        public void SubSubMenu_SubMenu_ShouldReturnSubMenu()
        {
            Menu subSubMenu = new Menu
            {
                Content1 = subMenu
            };

            Assert.That(subSubMenu.SubMenu, Is.SameAs(subMenu));
        }

		[Test]
        public void TextContentWithoutMenuShouldReturnNull()
        {
            var textContent = new TextContent
            {
                Text = "Hello World"
            };

            textContent.SubMenu.ShouldBeNull();
        }
    }
}
