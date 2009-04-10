using System;
using System.Collections.Generic;
using System.IO;
using Commons.Collections;
using NVelocity;
using NVelocity.App;
using NVelocity.Runtime;

namespace Suteki.Common.Services
{
	public class EmailBuilder : IEmailBuilder
	{
		static readonly IDictionary<string, object> defaultProperties;
		readonly VelocityEngine velocityEngine;
		string templatePath = "Views\\EmailTemplates";

		static EmailBuilder()
		{
			defaultProperties = new Dictionary<string, object>();

			string basePath = AppDomain.CurrentDomain.BaseDirectory;
			defaultProperties.Add(RuntimeConstants.RESOURCE_LOADER, "file");
			defaultProperties.Add(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, basePath);
		}

		public EmailBuilder() : this(defaultProperties)
		{
		}

		public EmailBuilder(IDictionary<string, object> nvelocityProperties)
		{
			var properties = new ExtendedProperties();

			foreach (var pair in nvelocityProperties)
			{
				properties.AddProperty(pair.Key, pair.Value);
			}

			velocityEngine = new VelocityEngine();
			velocityEngine.Init(properties);
		}

		public string TemplatePath
		{
			get { return templatePath; }
			set { templatePath = value; }
		}

		public string GetEmailContent(string templateName, IDictionary<string, object> viewdata)
		{
			return BuildEmail(templateName, viewdata);
		}

		/// <exception cref="ArgumentException"></exception>
		string BuildEmail(string templateName, IDictionary<string, object> viewdata)
		{
			if (viewdata == null)
			{
				throw new ArgumentNullException("viewData");
			}

			if (string.IsNullOrEmpty(templateName))
			{
				throw new ArgumentException("TemplateName");
			}

			var template = ResolveTemplate(templateName);

			var context = new VelocityContext();

			foreach (var key in viewdata.Keys)
			{
				context.Put(key, viewdata[key]);
			}

			using (var writer = new StringWriter())
			{
				template.Merge(context, writer);
				return writer.ToString();
			}
		}

		Template ResolveTemplate(string name)
		{
			name = Path.Combine(templatePath, name);

			if (!Path.HasExtension(name))
			{
				name += ".vm";
			}

			if (!velocityEngine.TemplateExists(name))
			{
				throw new InvalidOperationException(string.Format("Could not find a template named '{0}'", name));
			}

			return velocityEngine.GetTemplate(name);
		}
	}
}