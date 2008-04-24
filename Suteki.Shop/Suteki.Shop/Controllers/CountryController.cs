using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Suteki.Shop.Repositories;
using Suteki.Shop.ViewData;
using Suteki.Shop.Validation;
using Suteki.Shop.Services;
using System.Security.Permissions;

namespace Suteki.Shop.Controllers
{
    [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
    public class CountryController : ControllerBase
    {
        IRepository<Country> countryRepository;
        IOrderableService<Country> countryOrderableService;

        public CountryController(
            IRepository<Country> countryRepository, 
            IOrderableService<Country> countryOrderableService)
        {
            this.countryRepository = countryRepository;
            this.countryOrderableService = countryOrderableService;
        }

        public ActionResult Index()
        {
            return RenderIndexView();
        }

        private ActionResult RenderIndexView()
        {
            return RenderView("Index", View.Data.WithCountries(countryRepository.GetAll().InOrder()));
        }

        public ActionResult New()
        {
            Country country = new Country 
            { 
                Position = countryOrderableService.NextPosition,
                IsActive = true
            };

            return RenderView("Edit", View.Data.WithCountry(country));
        }

        public ActionResult Edit(int id)
        {
            Country country = countryRepository.GetById(id);
            return RenderView("Edit", View.Data.WithCountry(country));
        }

        public ActionResult Update(int countryId)
        {
            Country country = null;
            if (countryId == 0)
            {
                country = new Country();
            }
            else
            {
                country = countryRepository.GetById(countryId);
            }

            try
            {
                ValidatingBinder.UpdateFrom(country, Request.Form);
                country.IsActive = (this.ReadFromRequest("isactive") == "true");

                if (countryId == 0)
                {
                    countryRepository.InsertOnSubmit(country);
                }
                countryRepository.SubmitChanges();

                return RenderView("Edit", View.Data.WithCountry(country).WithMessage("This country has been saved"));
            }
            catch (ValidationException validationException)
            {
                return RenderView("Edit", View.Data.WithCountry(country).WithErrorMessage(validationException.Message));
            }
        }

        public ActionResult MoveUp(int id)
        {
            countryOrderableService.MoveItemAtPosition(id).UpOne();
            return RenderIndexView();
        }

        public ActionResult MoveDown(int id)
        {
            countryOrderableService.MoveItemAtPosition(id).DownOne();
            return RenderIndexView();
        }
    }
}
