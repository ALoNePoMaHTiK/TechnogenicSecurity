using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using TechnogenicSecurity.Models;

namespace TechnogenicSecurity
{
    public static class CatalogAdministrator
    {
        const string SUBSTANCES_PATH = "Catalogs\\Substances.json";

        public static ObservableCollection<Substance> getSubstances()
        { 
            ObservableCollection<Substance> substances = new ObservableCollection<Substance>();
            try
            {
                string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string json = File.ReadAllText(Path.Combine(dir,SUBSTANCES_PATH));
                substances = JsonSerializer.Deserialize<ObservableCollection<Substance>>(json);
            }
            catch (System.Exception e) { }
            return substances;
        }

        public static void saveSubstances(ObservableCollection<Substance> substances)
        {
            File.WriteAllText(SUBSTANCES_PATH, JsonSerializer.Serialize(substances, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                WriteIndented = true
            }));
        }
    }
}
