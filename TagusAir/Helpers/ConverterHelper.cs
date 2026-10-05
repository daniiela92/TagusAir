using TagusAir.Data.Entities;
using TagusAir.Models;

namespace TagusAir.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public Airplane ToAirplane(AirplaneViewModel model, string path, bool isNew)
        {
            return new Airplane
            {
                Id = isNew ? 0 : model.Id,
                Brand = model.Brand,
                Model = model.Model,
                EconomySeats = model.EconomySeats,
                BusinessSeats = model.BusinessSeats,
                IsActive = model.IsActive,
                ImageUrl = path
            };
        }

        public AirplaneViewModel ToAirplaneViewModel(Airplane airplane)
        {
            return new AirplaneViewModel
            {
                Id = airplane.Id,
                Brand = airplane.Brand,
                Model = airplane.Model,
                EconomySeats = airplane.EconomySeats,
                BusinessSeats = airplane.BusinessSeats,
                IsActive = airplane.IsActive,
                ImageUrl = airplane.ImageUrl
            };
        }

        public Country ToCountry(CountryViewModel model, string path, bool isNew)
        {
            return new Country
            {
                Id = isNew ? 0 : model.Id,
                Name = model.Name,
                FlagImageUrl = path
            };
        }

        public CountryViewModel ToCountryViewModel(Country country)
        {
            return new CountryViewModel
            {
                Id = country.Id,
                Name = country.Name,
                FlagImageUrl = country.FlagImageUrl
            };
        }
    }
}
