using TagusAir.Data.Entities;
using TagusAir.Models;

namespace TagusAir.Helpers
{
    public interface IConverterHelper
    {
        Airplane ToAirplane(AirplaneViewModel model, string path, bool isNew);

        AirplaneViewModel ToAirplaneViewModel(Airplane airplane);

        Country ToCountry(CountryViewModel model, string path, bool isNew);

        CountryViewModel ToCountryViewModel(Country country);
    }
}
