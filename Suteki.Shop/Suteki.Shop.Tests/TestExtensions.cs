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

		public static T ShouldEqual<T>(this T actual, object expected)
		{
			Assert.AreEqual(expected, actual);
			return actual;
		}
	}
}