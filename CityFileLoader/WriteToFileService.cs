using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CityFileLoader
{
    internal class WriteToFileService
    {
        private readonly string _destinationPath;

        public WriteToFileService(string destinationPath)
        {
            _destinationPath = destinationPath;
        }

        public void Write(IEnumerable<City> cities, string name)
        {
            var fileName = Path.Combine(_destinationPath, name + ".txt");
            using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                using (var fileWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    foreach (var city in cities)
                    {
                        fileWriter.Write(city.ToString());
                        fileWriter.Write("\r\n");
                    }
                }
            }
        }
    }
}