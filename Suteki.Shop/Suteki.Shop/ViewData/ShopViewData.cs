using System;
using System.Collections.Generic;
using Suteki.Shop.Extensions;

namespace Suteki.Shop.ViewData
{
    public class ShopViewData : IMessageViewData, IErrorViewData
    {
        public string ErrorMessage { get; set; }
        public string Message { get; set; }
        
        public Category Category { get; set; }
        public IEnumerable<Category> Categories { get; set; }

        public Product Product { get; set; }
        public IEnumerable<Product> Products { get; set; }

        public IEnumerable<Role> Roles { get; set; }

        public User User { get; set; }
        public IEnumerable<User> Users { get; set; }

        public Basket Basket { get; set; }

        public Order Order { get; set; }

        public IEnumerable<Country> Countries { get; set; }
        public Country Country { get; set; }

        public IEnumerable<CardType> CardTypes { get; set; }

        public Postage Postage { get; set; }
        public IEnumerable<Postage> Postages { get; set; }

        // attempt at a fluent interface

        public ShopViewData WithErrorMessage(string errorMessage)
        {
            this.ErrorMessage = errorMessage;
            return this;
        }

        public ShopViewData WithMessage(string message)
        {
            this.Message = message;
            return this;
        }

        public ShopViewData WithCategory(Category category)
        {
            this.Category = category;
            return this;
        }

        public ShopViewData WithCategories(IEnumerable<Category> categories)
        {
            this.Categories = categories;
            return this;
        }

        public ShopViewData WithProduct(Product product)
        {
            this.Product = product;
            return this;
        }

        public ShopViewData WithProducts(IEnumerable<Product> products)
        {
            this.Products = products;
            return this;
        }

        public ShopViewData WithRoles(IEnumerable<Role> roles)
        {
            this.Roles = roles;
            return this;
        }

        public ShopViewData WithUser(User user)
        {
            this.User = user;
            return this;
        }

        public ShopViewData WithUsers(IEnumerable<User> users)
        {
            this.Users = users;
            return this;
        }

        public ShopViewData WithBasket(Basket basket)
        {
            this.Basket = basket;
            return this;
        }

        public ShopViewData WithOrder(Order order)
        {
            this.Order = order;
            return this;
        }

        public ShopViewData WithCountries(IEnumerable<Country> countries)
        {
            this.Countries = countries;
            return this;
        }

        public ShopViewData WithCountry(Country country)
        {
            this.Country = country;
            return this;
        }

        public ShopViewData WithCardTypes(IEnumerable<CardType> cardTypes)
        {
            this.CardTypes = cardTypes;
            return this;
        }

        public ShopViewData WithPostage(Postage postage)
        {
            this.Postage = postage;
            return this;
        }

        public ShopViewData WithPostages(IEnumerable<Postage> postages)
        {
            this.Postages = postages;
            return this;
        }
    }

    /// <summary>
    /// So you can write 
    /// View.Data.WithProducts(myProducts);
    /// </summary>
    public class View
    {
        public static ShopViewData Data { get { return new ShopViewData(); } }
    }
}
