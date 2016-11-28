using System.Runtime.InteropServices;
using System.Text;

namespace CityFileLoader
{
    public class City
    {
        public static City FromCsv(string line)
        {
            var values = line.Split(',');
            return new City
                   {
                       CountryCode = Translate(values[0]),
                       AsciiName = values[1],
                       Name = values[2],
                       RegionCode = values[3],
                       Population = ParseDouble(values[4])??0,
                       Latitude = ParseDouble(values[5]),
                       Longitude = ParseDouble(values[6])
                   };
        }

        private static string Translate(string countryCode)
        {
            //Change Zaire to Democratic Republic of Congo
            if (countryCode == "zr")
                return "cd";
            return countryCode;
        }

        public double? Longitude { get; set; }

        public double? Latitude { get; set; }

        public double Population { get; set; }

        public string RegionCode { get; set; }

        public string Name { get; set; }

        public string AsciiName { get; set; }

        public string CountryCode { get; set; }

        public bool IsImportant { get; set; }

        public bool IsValid => Latitude != null && Longitude != null;
        public bool IsPatched { get; set; }

        public string TimeZone { get; set; }
        public string Id { get; set; }
        public bool IsLarge => Population > 25000;

        public static double? ParseDouble(string value)
        {
            double doubleValue;
            if (double.TryParse(value, out doubleValue))
                return doubleValue;
            return null;
        }

        public static int ParseInt(string value)
        {
            int intValue;
            return int.TryParse(value, out intValue) ? intValue : 0;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(Id);
            builder.Append('\t');
            builder.Append(CountryCode);
            builder.Append('\t');
            builder.Append(AsciiName);
            builder.Append('\t');
            builder.Append(Name);
            builder.Append('\t');
            builder.Append(RegionCode);
            builder.Append('\t');
            builder.Append(Population);
            builder.Append('\t');
            builder.Append(Latitude);
            builder.Append('\t');
            builder.Append(Longitude);
            builder.Append('\t');
            builder.Append(TimeZone);
            return builder.ToString();
        }
    }
}