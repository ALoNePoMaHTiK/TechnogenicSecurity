using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TechnogenicSecurity.Models;
using Word = Microsoft.Office.Interop.Word;

namespace TechnogenicSecurity.ViewModels
{
    public class ExplosionCalculationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        
        public ExplosionCalculationViewModel()
        {
            Substances = CatalogAdministrator.getSubstances();
            Substance = Substances[0];

            SubstanceVolume = 500;
            EvaporationArea = 400;
            NeighborBuildingDistance = 50;
            CoolingTemperature = 300;
            FillingLevel = 0.8;

            PeopleCount = 10;
            StaffDensity = 900;
        }

        #region Справочники

        private ObservableCollection<Substance> _Substances;

        public ObservableCollection<Substance> Substances
        {
            get { return _Substances; }
            set { _Substances = value; }
        }

        #endregion

        #region Хар-ки вещества

        private Substance _Substance;
        public Substance Substance
        {
            get { return _Substance; }
            set { _Substance = value; OnPropertyChanged(); }
        }

        private int _SubstanceVolume;
        public int SubstanceVolume
        {
            get { return _SubstanceVolume; }
            set { _SubstanceVolume = value; OnPropertyChanged(); }
        }
        private int _EvaporationArea;
        public int EvaporationArea
        {
            get { return _EvaporationArea; }
            set { _EvaporationArea = value; OnPropertyChanged(); }
        }

        private int _NeighborBuildingDistance;
        public int NeighborBuildingDistance
        {
            get { return _NeighborBuildingDistance; }
            set { _NeighborBuildingDistance = value; OnPropertyChanged(); }
        }

        private int _CoolingTemperature;
        public int CoolingTemperature
        {
            get { return _CoolingTemperature; }
            set { _CoolingTemperature = value; OnPropertyChanged(); }
        }

        private double _FillingLevel;
        public double FillingLevel
        {
            get { return _FillingLevel; }
            set { _FillingLevel = value; OnPropertyChanged(); }
        }
        #endregion

        #region Хар-ки персонала

        private int _PeopleCount;
        public int PeopleCount
        {
            get { return _PeopleCount; }
            set { _PeopleCount = value; OnPropertyChanged(); }
        }

        private double _StaffDensity;
        public double StaffDensity
        {
            get { return _StaffDensity; }
            set { _StaffDensity = value; OnPropertyChanged(); }
        }

        #endregion

        #region Константы

        const double GasolineVaporVolume = 0.2;
        const int AtmosphericPressure = 101300;
        const int R = 8310;
        const int EvaporationTime = 3600; //время испарения разлившейся жидкости, с, равное либо времени полного испарения либо ограничиваемое временем 3600 с, в течение которых должны быть приняты меры по устранению аварии
        const double V0 = 22.4;
        const int AirTemperature = 61; //расчетная температура, оС, принимаемая равной максимально возможно температуре воздуха в соответствующей климатической зоне
        const double Z = 0.1; //коэффициент участия горючих газов и паров в горении

        #endregion

        private ExplosionCalculation _ExplosionCalculationResults;
        public ExplosionCalculation Results
        {
            get { return _ExplosionCalculationResults; }
            set { _ExplosionCalculationResults = value; OnPropertyChanged(); }
        }

        public DelegateCommand CalculateValuesCommand
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    Calculate();
                });
            }
        }

        public DelegateCommand GenerateWordReportCommand
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    GenerateWordReport();
                });
            }
        }

        private void Calculate()
        {
            ExplosionCalculation newResults = new ExplosionCalculation();
            newResults.FirstCloudMass = GasolineVaporVolume * Substance.MolarMass * (SubstanceVolume * AtmosphericPressure) / (R * CoolingTemperature);
            newResults.SaturatedSteamPressure = AtmosphericPressure*Math.Exp(Substance.HiddenVaporizationHeat * Substance.MolarMass * (Math.Pow(Substance.BoilingTemperature,-1) - Math.Pow(CoolingTemperature,-1))/((double)R/1000))/1000;
            newResults.EvaporationRate = (Math.Pow(10, -6) * newResults.SaturatedSteamPressure * Math.Sqrt(Substance.MolarMass));
            newResults.SecondCloudMass = newResults.EvaporationRate * EvaporationArea * 3600;
            newResults.TotalVaporMass = newResults.FirstCloudMass + newResults.SecondCloudMass;
            newResults.GasDensity = Substance.MolarMass / (V0 * (1 + 0.00367 * AirTemperature));
            newResults.BlastingCloudRadius = 3.1501 * Math.Sqrt(EvaporationTime / 3600) * Math.Pow(newResults.SaturatedSteamPressure / Substance.LCLS, 0.813) * Math.Pow(newResults.TotalVaporMass / (newResults.GasDensity * newResults.SaturatedSteamPressure),1.0/3.0);
            newResults.DetonationAreaRedius = 10 * Math.Pow(newResults.TotalVaporMass * 0.06 / (Substance.MolarMass * 2.1),1.0/3.0);
            newResults.ReducedVaporMass = (46200.0 / 4520) * newResults.TotalVaporMass * Z;
            newResults.WaveFrontExcessivePressure = 81 * Math.Pow(newResults.ReducedVaporMass, 1.0 / 3.0) / newResults.BlastingCloudRadius + 303 * Math.Pow(newResults.ReducedVaporMass, 2.0 / 3.0) / Math.Pow(newResults.BlastingCloudRadius,2) + 505 * newResults.ReducedVaporMass / Math.Pow(newResults.BlastingCloudRadius, 3);
            newResults.NeighborBuildingExcessivePressure = 81 * (Math.Pow(newResults.ReducedVaporMass, 1.0 / 3.0) / NeighborBuildingDistance) + 303 * (Math.Pow(newResults.ReducedVaporMass, 2.0 / 3.0) / Math.Pow(NeighborBuildingDistance, 2)) + 505 * newResults.ReducedVaporMass / Math.Pow(NeighborBuildingDistance, 3);
            newResults.ShockWaveCompressionPhaseImpulse = 0.123 * Math.Pow(newResults.ReducedVaporMass, 2.0 / 3) / NeighborBuildingDistance;
            newResults.WeakDestructionProbability = 5 - 0.26 * Math.Log(Math.Pow(4.6 / newResults.NeighborBuildingExcessivePressure, 3.9) + Math.Pow(0.11 / newResults.ShockWaveCompressionPhaseImpulse, 5.0));
            newResults.MediumDestructionProbability = 5 - 0.26 * Math.Log(Math.Pow(17.5 / newResults.NeighborBuildingExcessivePressure, 8.4) + Math.Pow(0.29 / newResults.ShockWaveCompressionPhaseImpulse, 9.3));
            newResults.SevereDestructionProbability = 5 - 0.26 * Math.Log(Math.Pow(40.0 / newResults.NeighborBuildingExcessivePressure, 7.4) + Math.Pow(0.26 / newResults.ShockWaveCompressionPhaseImpulse, 11.3));
            newResults.HumanLosses1 = (StaffDensity/1000)*Math.Pow(newResults.ReducedVaporMass,2.0/3);
            newResults.HumanLosses2 = 4 * newResults.HumanLosses1;
            newResults.TotalHumanLosses = newResults.HumanLosses1 + newResults.HumanLosses2;
            Results = newResults;
        }

        private void GenerateWordReport()
        {
            Dictionary<string,string> properties = new Dictionary<string,string>();
            properties.Add(nameof(GasolineVaporVolume),GasolineVaporVolume.ToString());
            properties.Add(nameof(AtmosphericPressure), AtmosphericPressure.ToString());
            properties.Add(nameof(Substance.MolarMass), Substance.MolarMass.ToString());
            properties.Add(nameof(SubstanceVolume), SubstanceVolume.ToString());
            properties.Add(nameof(CoolingTemperature), CoolingTemperature.ToString());
            properties.Add(nameof(Results.FirstCloudMass), Results.FirstCloudMass.ToString());
            Word.Application app = new Word.Application();
            app.ShowAnimation = false;
            app.Visible = false;
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path = Path.Combine(dir, "ReportTemplates\\ОтчетВзрывШаблон.docx");
            Word.Document document = app.Documents.Open(FileName: path);
            Word.Find finder = app.Selection.Find;
            foreach (var prop in properties)
            {
                finder.Text = $"<{prop.Key}>";
                finder.Replacement.Text = $"{prop.Value}";
                finder.Execute(FindText: Type.Missing,
                   MatchCase: false,
                   MatchWholeWord: false,
                   MatchAllWordForms: false,
                   Forward: true,
                   Wrap: Word.WdFindWrap.wdFindContinue,
                   ReplaceWith: Type.Missing,
                   Replace: Word.WdReplace.wdReplaceAll);
            } 
            string savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ОтчетВзрыв.docx");
            document.SaveAs2(savePath);
            document.Close();
            document = null;
            app.Quit();
            app = null;
        }

        private KeyValuePair<string,string> getVariableData(int variable)
        {
            return KeyValuePair.Create(nameof(variable),variable.ToString());
        }
    }
}
