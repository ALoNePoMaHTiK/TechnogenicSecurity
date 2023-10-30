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
        const string OBJECTS_TYPES_PATH = "Catalogs\\ObjectTypes.json";
        const string STORING_METHODS_PATH = "Catalogs\\StoringFlammableSubstancesMethods.json";

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

        public static ObservableCollection<StoringMethod> getStoringMethods()
        {
            ObservableCollection<StoringMethod> storingMethods = new ObservableCollection<StoringMethod>();
            try
            {
                string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string json = File.ReadAllText(Path.Combine(dir, STORING_METHODS_PATH));
                storingMethods = JsonSerializer.Deserialize<ObservableCollection<StoringMethod>>(json);
            }
            catch (System.Exception e) { }
            return storingMethods;
        }

        public static ObservableCollection<ObjectType> getNeigborObjects()
        {
            ObservableCollection<ObjectType> objects = new ObservableCollection<ObjectType>();
            try
            {
                string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string json = File.ReadAllText(Path.Combine(dir, OBJECTS_TYPES_PATH));
                objects = JsonSerializer.Deserialize<ObservableCollection<ObjectType>>(json);
            }
            catch (System.Exception e) { }
            return objects;
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
