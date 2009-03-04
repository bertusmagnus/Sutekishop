using Suteki.Shop.HtmlHelpers;

namespace Suteki.Shop
{
	public class ViewPage<T> : MvcContrib.FluentHtml.ModelViewPage<T> where T : class 
	{
		public ViewPage() : base(new LowercaseFirstCharacterOfNameBehaviour())
		{
			
		}
	}
}