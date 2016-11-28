using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CityFileLoader
{
    public class AlternateCityRepository
    {
        private Dictionary<string, AlternateCity[]> _citiesByCountry;

        public AlternateCityRepository(string fileName)
        {
            ReadCities(fileName);
        }

        public List<AlternateCity> Cities { get; set; }

        private void ReadCities(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    Cities = ReadCities(reader).ToList();
                }
            }
            _citiesByCountry = Cities.GroupBy(c => c.CountryCode).ToDictionary(g => g.Key, g => g.ToArray());
        }

        private IEnumerable<AlternateCity> ReadCities(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var city = ReadCity(line);
                if (city != null)
                    yield return city;
            }
        }

        private AlternateCity ReadCity(string line)
        {
            return AlternateCity.FromCsv(line);
        }

        public void Patch(CityRepository cities)
        {
            var countryIndex = 0;
            foreach (var countryGroup in cities.CitiesByCountry)
            {
                var citiesToPatch = countryGroup.Where(c => c.IsImportant).ToArray();
                if (!citiesToPatch.Any())
                    citiesToPatch = countryGroup.ToArray();

                var cityIndex = 0;
                var patched = 0;
                foreach (var city in citiesToPatch)
                {
                    Form1.SetStatus($"Patching country {countryGroup.Key} ({countryIndex} of {cities.CitiesByCountry.Length}): {cityIndex} of {citiesToPatch.Length} cities", cityIndex, citiesToPatch.Length);
                    var alternateCity = MatchCity(city);
                    if (alternateCity != null)
                    {
                        PatchCity(city, alternateCity);
                        patched++;
                    }
                    else if (city.IsImportant)
                    {
                        Form1.LogMessage($"Missed important city: {city.CountryCode.ToUpperInvariant()} {city.Name}");
                    }
                    cityIndex++;
                }
                countryIndex++;
                Form1.LogMessage($"Matched '{countryGroup.Key.ToUpperInvariant()}': Success rate: {((double)patched / citiesToPatch.Length * 100).ToString("F1")}%, {citiesToPatch.Length - patched} of {citiesToPatch.Length} missed");
            };

        }

        private AlternateCity MatchCity(City city)
        {
            if (!_citiesByCountry.ContainsKey(city.CountryCode.ToLowerInvariant()))
                return null;
            var countryCities = _citiesByCountry[city.CountryCode.ToLowerInvariant()];
            var matchedCity = countryCities.FirstOrDefault(a => MatchCities(city, a));
            if (matchedCity == null && countryCities.Length < 5)
            {
                matchedCity = countryCities.OrderByDescending(c => c.Population).FirstOrDefault();
                if (matchedCity != null)
                    matchedCity.Population = null;
            }
            return matchedCity;
        }

        private static bool MatchCities(City city, AlternateCity alternate)
        {
            if (alternate.AsciiName.Equals(city.AsciiName, StringComparison.InvariantCultureIgnoreCase))
                return true;
            if (alternate.Name.Equals(city.Name, StringComparison.InvariantCultureIgnoreCase))
                return true;
            if (alternate.AlternateNames.Select(n => n.ToLowerInvariant()).Any(n => n == city.AsciiName.ToLowerInvariant() || n == city.Name.ToLowerInvariant()))
                return true;
            return false;
        }

        private void PatchCity(City city, AlternateCity alternateCity)
        {
            city.Id = alternateCity.GeoNameId;
            city.TimeZone = alternateCity.TimeZone;
            if (alternateCity.Population.HasValue && alternateCity.Population > city.Population)
                city.Population = alternateCity.Population.Value;
            city.IsPatched = true;
        }
    }
}