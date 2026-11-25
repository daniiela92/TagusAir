namespace TagusAir.Helpers
{
    public class AirportLocation
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }

    // Coordinates from OurAirports (ourairports.com/data), released to the Public Domain.
    public static class AirportCoordinates
    {
        private static readonly Dictionary<string, AirportLocation> _airports = new Dictionary<string, AirportLocation>
        {
            { "LIS", new AirportLocation { Latitude = 38.7813, Longitude = -9.1359 } },    // Lisboa
            { "OPO", new AirportLocation { Latitude = 41.2481, Longitude = -8.6814 } },    // Porto
            { "FAO", new AirportLocation { Latitude = 37.0159, Longitude = -7.9709 } },    // Faro
            { "FNC", new AirportLocation { Latitude = 32.6978, Longitude = -16.7746 } },   // Funchal
            { "PDL", new AirportLocation { Latitude = 37.7412, Longitude = -25.6979 } },   // Ponta Delgada
            { "MAD", new AirportLocation { Latitude = 40.4934, Longitude = -3.5722 } },    // Madrid
            { "BCN", new AirportLocation { Latitude = 41.2971, Longitude = 2.0785 } },     // Barcelona
            { "AGP", new AirportLocation { Latitude = 36.6782, Longitude = -4.5002 } },    // Málaga
            { "CDG", new AirportLocation { Latitude = 49.0090, Longitude = 2.5541 } },     // Paris
            { "LHR", new AirportLocation { Latitude = 51.4707, Longitude = -0.4599 } },    // Londres Heathrow
            { "LGW", new AirportLocation { Latitude = 51.1487, Longitude = -0.1857 } },    // Londres Gatwick
            { "AMS", new AirportLocation { Latitude = 52.3086, Longitude = 4.7639 } },     // Amesterdão
            { "FRA", new AirportLocation { Latitude = 50.0267, Longitude = 8.5584 } },     // Frankfurt
            { "FCO", new AirportLocation { Latitude = 41.8045, Longitude = 12.2520 } },    // Roma
            { "BRU", new AirportLocation { Latitude = 50.9014, Longitude = 4.4844 } },     // Bruxelas
            { "DUB", new AirportLocation { Latitude = 53.4287, Longitude = -6.2621 } },    // Dublin
            { "GRU", new AirportLocation { Latitude = -23.4313, Longitude = -46.4700 } },  // São Paulo
            { "LAD", new AirportLocation { Latitude = -8.8584, Longitude = 13.2312 } }     // Luanda
        };

        public static AirportLocation GetAirportLocation(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }

            return _airports.TryGetValue(code.Trim().ToUpper(), out var location) ? location : null;
        }
    }
}