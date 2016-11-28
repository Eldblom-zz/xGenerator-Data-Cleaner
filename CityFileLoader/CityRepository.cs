using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CityFileLoader
{
    public class CityRepository
    {
        public CityRepository(string fileName)
        {
            ReadCities(fileName);
        }

        public City[] Cities { get; set; }

        public IGrouping<string, City>[] CitiesByCountry { get; set; }

        private void ReadCities(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    Cities = ReadCities(reader).ToArray();
                }
            }
            CitiesByCountry = Cities.GroupBy(c => c.CountryCode).ToArray();
            MarkLargeCitiesAsImportant();
        }

        public void MarkLargeCitiesAsImportant()
        {
            var index = 0;
            foreach (var cityGroup in CitiesByCountry)
            {
                Form1.SetStatus("Marking large important cities", index++, CitiesByCountry.Length);
                if (!cityGroup.Any(c => c.IsLarge))
                    continue;
                foreach (var city in cityGroup)
                {
                    city.IsImportant = city.IsLarge;
                }
            }
        }

        public void EnsureOneImportantCityPerCountry()
        {
            foreach (var countryCities in CitiesByCountry)
            {
                if (countryCities.Any(c => c.IsImportant))
                    continue;
                var citiesToSearch = countryCities.Where(c => c.IsPatched).ToArray();
                if (!citiesToSearch.Any())
                    citiesToSearch = countryCities.ToArray();
                var firstCity = citiesToSearch.OrderByDescending(c => c.Population).ThenBy(c => c.RegionCode).First();
                firstCity.IsImportant = true;
                if (firstCity.Population <= 1)
                    firstCity.Population = 10000;
            }
        }

        private IEnumerable<City> ReadCities(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var city = ReadCity(line);
                if (city != null)
                    yield return city;
            }
        }

        private City ReadCity(string line)
        {
            var city = City.FromCsv(line);
            return city.IsValid ? city : null;
        }

        public IEnumerable<City> GetImportantCities()
        {
            return Cities.Where(c => c.IsImportant);
        }
    }
}