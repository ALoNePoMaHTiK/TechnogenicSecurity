using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TechnogenicSecurity.Models;

namespace TechnogenicSecurity.ViewModels
{
    public class ClosedSpaceExplosionCalculationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ClosedSpaceExplosionCalculationViewModel()
        {
            Coefs = CatalogAdministrator.getAirSpeedAndTemperatureFlowCoefficients();
            Substances = CatalogAdministrator.getSubstances();
            Substance = Substances[0];

            Substance.MolarMass = 240;
            Substance.BoilingTemperature = 57;
            Substance.HiddenVaporizationHeat = 345400;
            Substance.Density = 860;

            RoomLength = 54;
            RoomWidth = 12;
            RoomHeight = 8.5;

            PumpCount = 4;
            PumpProductivity = 2.78;
            PumpFillVolume = 25.76;
            PumpArea = 12.88;
            SupplyPipelineLength = 3;
            OutletPipelineLength = 4.4;
            PipelineDiameter = 1.02;
            EmergencyVentilationRate = 9;
            IndoorAirSpeed = 1;
            IndoorTemperature = 20;
            SubstanceTemperature = 22.4;
            AutoShutdownTime = 120;
            RoomFreeVolumePersent = 0.8;
            Betta = 26.5;

            Calculate();
        }

        private ObservableCollection<Substance> _Substances;

        public ObservableCollection<Substance> Substances
        {
            get { return _Substances; }
            set { _Substances = value; }
        }

        private ObservableCollection<AirSpeedAndTemperatureFlowCoefficientDTO> _Coefs;
        public ObservableCollection<AirSpeedAndTemperatureFlowCoefficientDTO> Coefs
        {
            get { return _Coefs; }
            set { _Coefs = value; }
        }

        #region Размеры зала

        private double _RoomLength;
        public double RoomLength
        {
            get { return _RoomLength; }
            set { _RoomLength = value; OnPropertyChanged(); }
        }

        private double _RoomWidth;
        public double RoomWidth
        {
            get { return _RoomWidth; }
            set { _RoomWidth = value; OnPropertyChanged(); }
        }

        private double _RoomHeight;
        public double RoomHeight
        {
            get { return _RoomHeight; }
            set { _RoomHeight = value; OnPropertyChanged(); }
        }

        #endregion

        #region Общие параметры

        private Substance _Substance;
        public Substance Substance
        {
            get { return _Substance; }
            set { _Substance = value; OnPropertyChanged(); }
        }

        private double _PumpCount;
        public double PumpCount
        {
            get { return _PumpCount; }
            set { _PumpCount = value; OnPropertyChanged(); }
        }
        private double _PumpProductivity;
        public double PumpProductivity
        {
            get { return _PumpProductivity; }
            set { _PumpProductivity = value; OnPropertyChanged(); }
        }
        
        private double _PumpFillVolume;
        public double PumpFillVolume
        {
            get { return _PumpFillVolume; }
            set { _PumpFillVolume = value; OnPropertyChanged(); }
        }

        private double _PumpArea;
        public double PumpArea
        {
            get { return _PumpArea; }
            set { _PumpArea = value; OnPropertyChanged(); }
        }

        private double _SupplyPipelineLength;
        public double SupplyPipelineLength
        {
            get { return _SupplyPipelineLength; }
            set { _SupplyPipelineLength = value; OnPropertyChanged(); }
        }

        private double _OutletPipelineLength;
        public double OutletPipelineLength
        {
            get { return _OutletPipelineLength; }
            set { _OutletPipelineLength = value; OnPropertyChanged(); }
        }

        private double _PipelineDiameter;
        public double PipelineDiameter
        {
            get { return _PipelineDiameter; }
            set { _PipelineDiameter = value; OnPropertyChanged(); }
        }

        private double _EmergencyVentilationRate;
        public double EmergencyVentilationRate
        {
            get { return _EmergencyVentilationRate; }
            set { _EmergencyVentilationRate = value; OnPropertyChanged(); }
        }

        private double _IndoorAirSpeed;
        public double IndoorAirSpeed
        {
            get { return _IndoorAirSpeed; }
            set { _IndoorAirSpeed = value; OnPropertyChanged(); getAirSpeedAndTemperatureFlowCoefficient(); }
        }
        private double _IndoorTemperature;
        public double IndoorTemperature
        {
            get { return _IndoorTemperature; }
            set { _IndoorTemperature = value; OnPropertyChanged(); getAirSpeedAndTemperatureFlowCoefficient(); }
        }

        private double _SubstanceTemperature;
        public double SubstanceTemperature
        {
            get { return _SubstanceTemperature; }
            set { _SubstanceTemperature = value; OnPropertyChanged(); }
        }

        private double _AutoShutdownTime;
        public double AutoShutdownTime
        {
            get { return _AutoShutdownTime; }
            set { _AutoShutdownTime = value; OnPropertyChanged(); }
        }
        
        private double _RoomFreeVolumePersent;
        public double RoomFreeVolumePersent
        {
            get { return _RoomFreeVolumePersent; }
            set { _RoomFreeVolumePersent = value; OnPropertyChanged(); }
        }

        private double _Betta;
        public double Betta
        {
            get { return _Betta; }
            set { _Betta = value; OnPropertyChanged(); }
        }
        
        private double _AirSpeedAndTemperatureFlowCoefficient;
        public double AirSpeedAndTemperatureFlowCoefficient
        {
            get { return _AirSpeedAndTemperatureFlowCoefficient; }
            set { _AirSpeedAndTemperatureFlowCoefficient = value; OnPropertyChanged(); }
        }

        #endregion

        private ClosedSpaceExplosionCalculation _Results;
        public ClosedSpaceExplosionCalculation Results
        {
            get { return _Results; }
            set { _Results = value; OnPropertyChanged(); }
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

        #region Константы

        const double AtmosphericPressure = 101.3;
        const double R = 8310;
        const double V0 = 22.4;
        const double Z = 0.3;
        const double Kn = 3;

        #endregion

        private void Calculate()
        {
            ClosedSpaceExplosionCalculation newResults = new ClosedSpaceExplosionCalculation();
            newResults.OilVolumeFromPipeline = (PumpProductivity * AutoShutdownTime) + (Math.PI * Math.Pow(PipelineDiameter, 2)/4)*(OutletPipelineLength+SupplyPipelineLength);
            newResults.EnteringOilVolume = newResults.OilVolumeFromPipeline+ PumpFillVolume;
            newResults.RoomArea = RoomWidth * RoomLength;
            newResults.PumpsArea = PumpCount * PumpArea;
            newResults.FreeRoomArea = newResults.RoomArea - newResults.PumpsArea;
            newResults.SubstanceLayer = newResults.EnteringOilVolume / newResults.FreeRoomArea;
            newResults.SaturatedSteamPressure = AtmosphericPressure * Math.Exp(Substance.HiddenVaporizationHeat * Substance.MolarMass * (Math.Pow(Substance.BoilingTemperature + 273, -1) - Math.Pow(SubstanceTemperature + 273, -1)) / R);
            newResults.EvaporationRate = (Math.Pow(10, -6) * newResults.SaturatedSteamPressure * AirSpeedAndTemperatureFlowCoefficient * Math.Sqrt(Substance.MolarMass));
            newResults.EmergencySpillVaporMass = newResults.EvaporationRate * newResults.FreeRoomArea * 3600;
            newResults.SpilledOilMass = newResults.EnteringOilVolume * Substance.Density;
            newResults.EvaporatedOilPersent = newResults.EmergencySpillVaporMass / newResults.SpilledOilMass * 100;
            newResults.VaporDensity = Substance.MolarMass/(V0 * (1+0.00367*SubstanceTemperature));
            newResults.StoichiometricGasConcentration = 100 / (1 + 4.84 * Betta);
            newResults.RoomFreeVolume = RoomLength*RoomWidth*RoomHeight* RoomFreeVolumePersent;
            newResults.ShockWaveExcessivePressure = (100*(900-AtmosphericPressure)* newResults.EmergencySpillVaporMass*Z)/((1+EmergencyVentilationRate) * newResults.RoomFreeVolume * newResults.VaporDensity * newResults.StoichiometricGasConcentration * Kn);
            Results = newResults;
        }

        private void GenerateWordReport()
        {
            IDictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add(getVariableData(Substance.MolarMass));
            properties.Add(getVariableData(Substance.HiddenVaporizationHeat));
            properties.Add(getVariableData(Substance.BoilingTemperature));
            properties.Add(getVariableData(Substance.LCLS));
            properties.Add(getVariableData(Substance.UCLS));
            properties.Add(getVariableData(Substance.GasExplosionEnergy));
            properties.Add(getVariableData(Substance.GasAirMixExplosionEnergy));
            properties.Add(getVariableData(Substance.Density));
            properties.Add(getVariableData(Substance.StoichiometricGasConcentration));

            properties.Add(getVariableData(PumpCount));
            properties.Add(getVariableData(PumpProductivity));
            properties.Add(getVariableData(PumpFillVolume));
            properties.Add(getVariableData(PumpArea));
            properties.Add(getVariableData(SupplyPipelineLength));
            properties.Add(getVariableData(OutletPipelineLength));
            properties.Add(getVariableData(PipelineDiameter));
            properties.Add(getVariableData(EmergencyVentilationRate));
            properties.Add(getVariableData(IndoorAirSpeed));
            properties.Add(getVariableData(IndoorTemperature));
            properties.Add(getVariableData(SubstanceTemperature));
            properties.Add(getVariableData(AutoShutdownTime));
            properties.Add(getVariableData(RoomFreeVolumePersent));
            properties.Add(getVariableData(Betta));
            properties.Add(getVariableData(AirSpeedAndTemperatureFlowCoefficient));

            properties.Add(getVariableData(Results.OilVolumeFromPipeline));
            properties.Add(getVariableData(Results.EnteringOilVolume));
            properties.Add(getVariableData(Results.RoomArea));
            properties.Add(getVariableData(Results.PumpsArea));
            properties.Add(getVariableData(Results.FreeRoomArea));
            properties.Add(getVariableData(Results.SubstanceLayer));
            properties.Add(getVariableData(Results.SaturatedSteamPressure));
            properties.Add(getVariableData(Results.EvaporationRate));
            properties.Add(getVariableData(Results.EmergencySpillVaporMass));
            properties.Add(getVariableData(Results.SpilledOilMass));
            properties.Add(getVariableData(Results.EvaporatedOilPersent));
            properties.Add(getVariableData(Results.VaporDensity));
            properties.Add(getVariableData(Results.StoichiometricGasConcentration));
            properties.Add(getVariableData(Results.RoomFreeVolume));
            properties.Add(getVariableData(Results.ShockWaveExcessivePressure));

            properties.Add(getVariableData(AtmosphericPressure));
            properties.Add(getVariableData(R));
            properties.Add(getVariableData(V0));
            properties.Add(getVariableData(Z));
            properties.Add(getVariableData(Kn));

            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path = Path.Combine(dir, "ReportTemplates\\ОтчетВзрывОграниченноеПространствоШаблон.docx");

            try
            {
                using (WordprocessingDocument template = WordprocessingDocument.Open(path, true))
                {
                    string docText = null;
                    using (StreamReader sr = new StreamReader(template.MainDocumentPart.GetStream()))
                    {
                        docText = sr.ReadToEnd();
                    }
                    foreach (var prop in properties)
                    {
                        docText = docText.Replace(prop.Key, prop.Value);
                    }
                    string savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ОтчетВзрывОграниченноеПространство.docx");
                    using (WordprocessingDocument report = WordprocessingDocument.Create(savePath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
                    {
                        foreach (var part in template.Parts)
                            report.AddPart(part.OpenXmlPart, part.RelationshipId);
                        using (StreamWriter sw = new StreamWriter(report.MainDocumentPart.GetStream(FileMode.Create)))
                        {
                            sw.Write(docText);
                        }
                    }
                    MessageBox.Show($"Отчет успешно сформиирован!\nРасположение : {savePath}", "Отчет успешно сформирован", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch(Exception e) 
            {
                MessageBox.Show($"Ошибка при формировании отчета!\n{e.Message}", "Уведомление об ошибке", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private KeyValuePair<string, string> getVariableData(double variable, [CallerArgumentExpression("variable")] string name = "value")
        {
            return KeyValuePair.Create(name, variable.ToString("#0.#####"));
        }

        private void getAirSpeedAndTemperatureFlowCoefficient()
        {
            foreach(var coef  in Coefs)
            {
                if (coef.IndoorAirSpeed == IndoorAirSpeed && coef.IndoorTemperature == IndoorTemperature)
                    AirSpeedAndTemperatureFlowCoefficient = coef.AirSpeedAndTemperatureFlowCoefficient;
            }
        }
    }
}
