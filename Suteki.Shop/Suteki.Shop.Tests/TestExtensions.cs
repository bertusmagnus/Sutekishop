using NUnit.Framework;

namespace Suteki.Shop.Tests
{
	public static class TestExtensions
	{
		public static T ShouldNotBeNull<T>(this T obj)
		{
			Assert.IsNotNull(obj);
			return obj;
		}
	}
}