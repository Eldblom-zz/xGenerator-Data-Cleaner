using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CityFileLoader
{
    public partial class Form1 : Form
    {
        private static DateTime _lastStatus = DateTime.MinValue;
        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            SetStatus("Loading cities");
            var cityRepository = new CityRepository(textBox1.Text);

            SetStatus("Loading alternate cities");
            var alternateCityRepository = new AlternateCityRepository(textBox2.Text);
            SetStatus("Patching cities");
            alternateCityRepository.Patch(cityRepository);

            SetStatus("Finding important cities");
            cityRepository.EnsureOneImportantCityPerCountry();

            SetStatus("Saving files");
            var importantCities = cityRepository.GetImportantCities().ToArray();
            var unpatched = importantCities.Where(c => !c.IsPatched).ToArray();
            var unpatchedCountries = cityRepository.CitiesByCountry.Where(g => !g.Any(c => c.IsPatched)).Select(g => g.Key).ToArray();
            LogMessage("Unpatched countries: " + string.Join(", ", unpatchedCountries));
            var unpatchedImportantCountries = cityRepository.CitiesByCountry.Where(g => !g.Any(c => c.IsImportant && c.IsPatched)).Select(g => g.Key).ToArray();
            LogMessage("Countries with no important patched cities: " + string.Join(", ", unpatchedImportantCountries));

            var service = new WriteToFileService(Path.GetDirectoryName(textBox1.Text));
            service.Write(cityRepository.GetImportantCities().Where(c => c.IsPatched), "ImportantCities");
            service.Write(unpatched, "Unpatched");
            SetStatus("Done...");
        }

        public static void SetStatus(string status, int statusIndex = 0, int statusCount = 0)
        {
            var form1 = ((Form1)Application.OpenForms[0]);
            form1.label1.Text = status;
            form1.progressBar1.Maximum = statusCount;
            form1.progressBar1.Value = statusIndex;
            DoEvents();
        }

        private static void DoEvents()
        {
            if ((DateTime.Now - _lastStatus).Seconds > 1)
            {
                _lastStatus = DateTime.Now;
                Application.DoEvents();
            }
        }

        public static void LogMessage(string message)
        {
            var form1 = ((Form1)Application.OpenForms[0]);
            form1.listBox1.Items.Add(message);
            form1.listBox1.SelectedIndex = form1.listBox1.Items.Count-1;
            DoEvents();
        }

    }
}