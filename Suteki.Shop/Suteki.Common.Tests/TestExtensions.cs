using System;
using NUnit.Framework;

namespace Suteki.Common.Tests
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

		public static Exception ShouldBeThrownBy(this Type exceptionType, TestDelegate testDelegate)
		{
			return Assert.Throws(exceptionType, testDelegate);
		}

		public static void ShouldBe<T>(this object actual)
		{
			Assert.IsInstanceOf<T>(actual);
		}

		public static void ShouldBeNull(this object actual)
		{
			Assert.IsNull(actual);
		}

		public static void ShouldBeTheSameAs(this object actual, object expected)
		{
			Assert.AreSame(expected, actual);
		}

		public static T CastTo<T>(this object source)
		{
			return (T)source;
		}

		public static void ShouldBeTrue(this bool source)
		{
			Assert.IsTrue(source);
		}

		public static void ShouldBeFalse(this bool source)
		{
			Assert.IsFalse(source);
		}
	}
}