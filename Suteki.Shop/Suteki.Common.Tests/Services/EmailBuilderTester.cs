using System;
using System.Collections.Generic;
using NUnit.Framework;
using Suteki.Common.Services;

namespace Suteki.Common.Tests.Services
{
	[TestFixture]
	public class EmailBuilderTester
	{
		IEmailBuilder builder;
		const string testView = "test";
		const string testViewWithPlaceholders = "testWithPlaceholders";

		[SetUp]
		public void Setup()
		{
			//For testing purposes, assume the views are Embedded Resources.
			//In reality, they'll be in the Views\Emails folder
			var properties = new Dictionary<string, object>();
			properties["resource.loader"] = "assembly";
			properties["assembly.resource.loader.class"] = "NVelocity.Runtime.Resource.Loader.AssemblyResourceLoader, NVelocity";
			properties["assembly.resource.loader.assembly"] = new List<string> { "Suteki.Common.Tests" };
			builder = new EmailBuilder(properties) { TemplatePath = "Suteki.Common.Tests.Services" };
		}

		[Test]
		public void Should_render_template()
		{
			var result = builder.GetEmailContent(testView, new Dictionary<string, object>());
			result.ShouldEqual("Test Email");
		}

		[Test]
		public void Should_throw_when_template_does_not_exist()
		{
			typeof(InvalidOperationException).ShouldBeThrownBy(() => 
				builder.GetEmailContent("foo", new Dictionary<string, object>())
			);
		}

		[Test]
		public void Should_merge_viewdata()
		{
			var viewdata = new Dictionary<string, object> { { "name", "Jeremy" } };
			var result = builder.GetEmailContent(testViewWithPlaceholders, viewdata);
			result.ShouldEqual("Hello Jeremy");
		}

		[Test]
		public void Throws_when_view_is_null()
		{
			typeof(ArgumentException).ShouldBeThrownBy(() => builder.GetEmailContent(null, new Dictionary<string, object>()));
		}

		[Test]
		public void Throws_when_viewdata_null()
		{
			typeof(ArgumentNullException).ShouldBeThrownBy(() => builder.GetEmailContent(testView, null));
		}

		[Test]
		public void Should_have_default_template_path()
		{
			var builder = new EmailBuilder();
			builder.TemplatePath.ShouldEqual("Views\\EmailTemplates");
		}
	}
}