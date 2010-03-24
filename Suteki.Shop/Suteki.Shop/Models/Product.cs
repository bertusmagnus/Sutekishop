using System;
using System.Linq;
using System.Text.RegularExpressions;
using Suteki.Common;
using Suteki.Common.Repositories;
using Suteki.Common.Validation;
using Suteki.Shop.Repositories;
using Suteki.Shop.Models.ModelHelpers;

namespace Suteki.Shop
{
    public partial class Product : IOrderable, IActivatable, IUrlNamed
    {
        partial void OnNameChanging(string value)
        {
            value.Label("Name").IsRequired();
        }

        partial void OnNameChanged()
        {
            UrlName = Name.ToUrlFriendly();
        }

        partial void OnDescriptionChanging(string value)
        {
            value.Label("Description").IsRequired();
        }

        public bool HasMainImage
        {
            get
            {
                return this.ProductImages.Count > 0;
            }
        }

        public Image MainImage
        {
            get
            {
                if (HasMainImage) return this.ProductImages.InOrder().First().Image;
                return null;
            }
        }

        public bool HasSize
        {
            get
            {
                return this.Sizes.Active().Count() > 0;
            }
        }

        public Size DefaultSize
        {
            get
            {
                if (DefaultSizeMissing) throw new ApplicationException("Product has no default size");
                return this.Sizes[0];
            }
        }

        public bool DefaultSizeMissing
        {
            get
            {
                return Sizes.Count == 0;
            }
        }

        public string IsActiveAsString
        {
            get
            {
                if (IsActive) return string.Empty;
                return " Not Active";
            }
        }

        public string PlainTextDescription
        {
            get
            {
                // thanks to Phil Haack :)
                const string matchHtml = @"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>";
                const string matchUnwantedChars = @"[""\n\r]";
                return Regex.Replace(Regex.Replace(Description, matchHtml, ""), matchUnwantedChars, "");
            }
        }

        public static Product DefaultProduct(int parentCategory, int position)
		{
			var product = new Product 
			{
				ProductId = 0,
				Position = position
			};
			product.ProductCategories.Add(new ProductCategory() { CategoryId = parentCategory });
			return product;
		}
    }
}
