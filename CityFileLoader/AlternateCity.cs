using System;
using System.Linq;

namespace CityFileLoader
{
    public class AlternateCity
    {
        public static AlternateCity FromCsv(string line)
        {
            if (line.StartsWith("#"))
                return null;
            var values = line.Split('\t');

            int index = 0;
            return new AlternateCity()
                   {
                       GeoNameId = values[index++],
                       Name = values[index++].ToLowerInvariant(),
                       AsciiName = values[index++].ToLowerInvariant(),
                       AlternateNames = values[index++].Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).Select(n => n.ToLowerInvariant()).ToArray(),
                       Latitude = City.ParseDouble(values[index++]),
                       Longitude = City.ParseDouble(values[index++]),
                       FeatureClass = values[index++],
                       FeatureCode = values[index++],
                       CountryCode = values[index++].ToLowerInvariant(),
                       OtherCountryCode = values[index++].Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries),
                       RegionCode = values[index++].ToLowerInvariant(),
                       Admin2 = values[index++],
                       Admin3 = values[index++],
                       Admin4 = values[index++],
                       Population = City.ParseDouble(values[index++]),
                       Elevation = City.ParseDouble(values[index++]),
                       DigitalElevationModel = values[index++],
                       TimeZone = values[index++],
                       ModificationDate = values[index]
                   };
        }

        public string ModificationDate { get; set; }

        public string TimeZone { get; set; }

        public string DigitalElevationModel { get; set; }

        public double? Elevation { get; set; }

        public double? Population { get; set; }

        public string Admin4 { get; set; }

        public string Admin3 { get; set; }

        public string Admin2 { get; set; }

        public string RegionCode { get; set; }

        public string[] OtherCountryCode { get; set; }

        public string CountryCode { get; set; }

        public string FeatureCode { get; set; }

        public string FeatureClass { get; set; }

        public double? Longitude { get; set; }

        public double? Latitude { get; set; }

        public string[] AlternateNames { get; set; }

        public string AsciiName { get; set; }

        public string Name { get; set; }

        public string GeoNameId { get; set; }
    }
}