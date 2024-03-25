using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using TechnogenicSecurity.Models;

namespace TechnogenicSecurity.ViewModels
{
    public class StraitFireCalculationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public StraitFireCalculationViewModel()
        {
            Substances = CatalogAdministrator.getSubstances();
            Substance = Substances[0];

            //TankRadius = 28;
            //TankHeight = 30;

            //ShellHeight = 9;
            //FluxDensity = 60;
            //WindSpeed = 5;
            //LowerHeatingValue = 40000;
            //TankFillLevel = 0.8;
            Calculate();
        }

        private ObservableCollection<Substance> _Substances;

        public ObservableCollection<Substance> Substances
        {
            get { return _Substances; }
            set { _Substances = value; }
        }

        #region Хар-ки вещества

        private Substance _Substance;
        public Substance Substance 
        {
            get { return _Substance; }
            set { _Substance = value; OnPropertyChanged(); }
        }
        #endregion

        #region Хар-ки резервуара

        /// <summary>
        ///     Радиус резервуара
        /// </summary>
        private double _TankRadius;
        public double TankRadius
        {
            get { return _TankRadius; }
            set { _TankRadius = value; OnPropertyChanged(); }
        }

        /// <summary>
        ///     Высота резервуара
        /// </summary>
        private double _TankHeight;
        public double TankHeight
        {
            get { return _TankHeight; }
            set { _TankHeight = value; OnPropertyChanged(); }
        }

        #endregion

        #region Общие характеристики

        /// <summary>
        ///     Высота обваловки
        /// </summary>
        private double _ShellHeight;
        public double ShellHeight
        {
            get { return _ShellHeight; }
            set { _ShellHeight = value; OnPropertyChanged(); }
        }
        private double _FluxDensity;
        public double FluxDensity
        {
            get { return _FluxDensity; }
            set { _FluxDensity = value; OnPropertyChanged(); }
        }
        private double _WindSpeed;
        public double WindSpeed
        {
            get { return _WindSpeed; }
            set { _WindSpeed = value; OnPropertyChanged(); }
        }
        private double _HeatOfLiquidVaporization;
        public double HeatOfLiquidVaporization
        {
            get { return _HeatOfLiquidVaporization; }
            set { _HeatOfLiquidVaporization = value; OnPropertyChanged(); }
        }

        private double _LowerHeatingValue;
        public double LowerHeatingValue
        {
            get { return _LowerHeatingValue; }
            set { _LowerHeatingValue = value; OnPropertyChanged(); }
        }

        private double _TankFillLevel;
        public double TankFillLevel
        {
            get { return _TankFillLevel; }
            set { _TankFillLevel = value; OnPropertyChanged(); }
        }

        #endregion

        #region Расчитываемые хар-ки

        private double _TankVolume;
        public double TankVolume
        {
            get { return _TankVolume; }
            set { _TankVolume = value; OnPropertyChanged(); }
        }

        private double _SubstanceVolume;
        public double SubstanceVolume
        {
            get { return _SubstanceVolume; }
            set { _SubstanceVolume = value; OnPropertyChanged(); }
        }

        private double _ShellArea;
        public double ShellArea
        {
            get { return _ShellArea; }
            set { _ShellArea = value; OnPropertyChanged(); }
        }

        private double _SpillMirrorDiameter;
        public double SpillMirrorDiameter
        {
            get { return _SpillMirrorDiameter; }
            set { _SpillMirrorDiameter = value; OnPropertyChanged(); }
        }

        private double _SpillMirrorRadius;
        public double SpillMirrorRadius
        {
            get { return _SpillMirrorRadius; }
            set { _SpillMirrorRadius = value; OnPropertyChanged(); }
        }

        private double _VaporDensity;
        public double VaporDensity
        {
            get { return _VaporDensity; }
            set { _VaporDensity = value; OnPropertyChanged(); }
        }

        private double _LiquidBurnoutRate;
        public double LiquidBurnoutRate
        {
            get { return _LiquidBurnoutRate; }
            set { _LiquidBurnoutRate = value; OnPropertyChanged(); }
        }

        private double _DimensionlessWindSpeed;
        public double DimensionlessWindSpeed
        {
            get { return _DimensionlessWindSpeed; }
            set { _DimensionlessWindSpeed = value; OnPropertyChanged(); }
        }

        private double _GeometricParameters;
        public double GeometricParameters
        {
            get { return _GeometricParameters; }
            set { _GeometricParameters = value; OnPropertyChanged(); }
        }

        private double _FlameSpillHeight;
        public double FlameSpillHeight
        {
            get { return _FlameSpillHeight; }
            set { _FlameSpillHeight = value; OnPropertyChanged(); }
        }
        private double _CosSpillFireFlameAngle;
        public double CosSpillFireFlameAngle
        {
            get { return _CosSpillFireFlameAngle; }
            set { _CosSpillFireFlameAngle = value; OnPropertyChanged(); }
        }
        private double _SpillFireFlameAngle;
        public double SpillFireFlameAngle
        {
            get { return _SpillFireFlameAngle; }
            set { _SpillFireFlameAngle = value; OnPropertyChanged(); }
        }
        private double _Lr;
        public double Lr
        {
            get { return _Lr; }
            set { _Lr = value; OnPropertyChanged(); }
        }
        private double _ShellSideLenth;
        public double ShellSideLenth
        {
            get { return _ShellSideLenth; }
            set { _ShellSideLenth = value; OnPropertyChanged(); }
        }

        private List<FireParameter> _FireParameters;
        public List<FireParameter> FireParameters
        {
            get { return _FireParameters; }
            set { _FireParameters = value; OnPropertyChanged(); }
        }

        private double _EffectiveExposureTime;
        public double EffectiveExposureTime
        {
            get { return _EffectiveExposureTime; }
            set { _EffectiveExposureTime = value; OnPropertyChanged(); }
        }

        private double _SafeDistanse;
        public double SafeDistanse
        {
            get { return _SafeDistanse; }
            set { _SafeDistanse = value; OnPropertyChanged(); }
        }

        private double _AdjacentTankDistance;
        public double AdjacentTankDistance
        {
            get { return _AdjacentTankDistance; }
            set { _AdjacentTankDistance = value; OnPropertyChanged(); }
        }

        private double _MaxFireDistance;
        public double MaxFireDistance
        {
            get { return _MaxFireDistance; }
            set { _MaxFireDistance = value; OnPropertyChanged(); }
        }

        private double _NeighborsAbsorbedHeat;
        public double NeighborsAbsorbedHeat
        {
            get { return _NeighborsAbsorbedHeat; }
            set { _NeighborsAbsorbedHeat = value; OnPropertyChanged(); }
        }

        #endregion

        #region Константы

        double[] EmissivityAngles = { 1, 0.34, 0.18, 0.1, 0.07, 0.05, 0.04, 0.03, 0.02, 0.01, 0.01, 0.01 };
        const double V0 = 22.4;
        const double TP = 22.4;
        const double g = 9.8;
        const double AIR_DENSITY = 1.29;
        const double u = 5; // характерное время обнаружения пожара
        const double C = 0.00000125; //коэффициент пропорциональности

        #endregion

        #region Комманды

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

        public DelegateCommand GenerateReportCommand
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    GenerateReport();
                });
            }
        }

        private void Calculate()
        {
            TankVolume = Math.PI * TankRadius * TankRadius * TankHeight;
            SubstanceVolume = TankVolume * TankFillLevel;
            ShellArea = SubstanceVolume / ShellHeight;
            SpillMirrorDiameter = Math.Sqrt(ShellArea * 4 / Math.PI);
            SpillMirrorRadius = SpillMirrorDiameter / 2;
            VaporDensity = Substance.MolarMass / (V0 * (1.0 + 0.00367 * TP));
            LiquidBurnoutRate = (C * Substance.Density * LowerHeatingValue) / Substance.HiddenVaporizationHeat;
            DimensionlessWindSpeed = WindSpeed * Math.Pow(VaporDensity / (LiquidBurnoutRate * g * SpillMirrorDiameter), 1.0 / 3.0);
            GeometricParameters = 55 * Math.Pow(LiquidBurnoutRate / (AIR_DENSITY * Math.Sqrt(g * SpillMirrorDiameter)), 0.67) * Math.Pow(DimensionlessWindSpeed, -0.21);
            FlameSpillHeight = GeometricParameters * SpillMirrorDiameter;
            CosSpillFireFlameAngle = 0.75 * Math.Pow(DimensionlessWindSpeed, -0.49);
            SpillFireFlameAngle = (180 / Math.PI) * Math.Acos(CosSpillFireFlameAngle);
            Lr = FlameSpillHeight / SpillMirrorRadius;
            ShellSideLenth = Math.Sqrt(ShellArea);

            double[] Distances = new double[12];
            for (int i = 0; i < Distances.Length; i++)
            {
                Distances[i] = SpillMirrorRadius * (0.5 * (i + 2));
            }

            double[] IncidentHeatFluxDensities = new double[12];
            for (int i = 0; i < Distances.Length; i++)
            {
                IncidentHeatFluxDensities[i] = FluxDensity * EmissivityAngles[i] * Math.Exp(-7 * Math.Pow(10, -4) * (Distances[i] - SpillMirrorRadius));
            }

            int IndexOfGoodDistanse = 0;
            for (int i = 0; i < IncidentHeatFluxDensities.Length; i++)
            {
                if (IncidentHeatFluxDensities[i] < 4)
                {
                    IndexOfGoodDistanse = i;
                    break;
                }
            }
            SafeDistanse = Distances[IndexOfGoodDistanse];

            EffectiveExposureTime = 5 + (SafeDistanse / u);

            FireParameters = new List<FireParameter>();
            for (int i = 0; i < 12; i++)
            {
                double Distance = Distances[i];
                double IncidentHeatFluxDensity = IncidentHeatFluxDensities[i];
                double ProbitFunctionValue = -9.5 + 2.56 * Math.Log(Math.Pow(IncidentHeatFluxDensity, 4.0 / 3) * EffectiveExposureTime);

                FireParameters.Add(new FireParameter()
                {
                    Distance = Distance,
                    IncidentHeatFluxDensity = IncidentHeatFluxDensity,
                    ProbitFunctionValue = ProbitFunctionValue
                });
            }

            AdjacentTankDistance = Math.Sqrt((TankFillLevel * Math.Pow(TankRadius, 2) * TankHeight) / (ShellHeight)) - TankRadius;
            MaxFireDistance = FlameSpillHeight / Math.Tan(1);

            NeighborsAbsorbedHeat = FluxDensity * 1 * Math.Exp(-7 * Math.Pow(10, -4) * (AdjacentTankDistance - MaxFireDistance));
        }

        private void GenerateReport()
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

            properties.Add(getVariableData(TankVolume));
            properties.Add(getVariableData(SubstanceVolume));
            properties.Add(getVariableData(ShellArea));
            properties.Add(getVariableData(SpillMirrorRadius));
            properties.Add(getVariableData(VaporDensity));
            properties.Add(getVariableData(LiquidBurnoutRate));
            properties.Add(getVariableData(DimensionlessWindSpeed));
            properties.Add(getVariableData(GeometricParameters));
            properties.Add(getVariableData(FlameSpillHeight));
            properties.Add(getVariableData(CosSpillFireFlameAngle));
            properties.Add(getVariableData(SpillFireFlameAngle));
            properties.Add(getVariableData(Lr));
            properties.Add(getVariableData(ShellSideLenth));
            properties.Add(getVariableData(SafeDistanse));
            properties.Add(getVariableData(EffectiveExposureTime));
            properties.Add(getVariableData(AdjacentTankDistance));
            properties.Add(getVariableData(MaxFireDistance));
            properties.Add(getVariableData(NeighborsAbsorbedHeat));

            for(int i = 11; i >=0; i--)
            {
                properties.Add($"Distance{i}",FireParameters[i].Distance.ToString("#0.#####"));
                properties.Add($"IncidentHeatFluxDensity{i}",FireParameters[i].IncidentHeatFluxDensity.ToString("#0.#####"));
                properties.Add($"ProbitFunctionValue{i}",FireParameters[i].ProbitFunctionValue.ToString("#0.#####"));
            }

            properties.Add(getVariableData(TankRadius));
            properties.Add(getVariableData(TankHeight));
            properties.Add(getVariableData(ShellHeight));
            properties.Add(getVariableData(FluxDensity));
            properties.Add(getVariableData(WindSpeed));
            properties.Add(getVariableData(HeatOfLiquidVaporization));
            properties.Add(getVariableData(LowerHeatingValue));
            properties.Add(getVariableData(TankFillLevel));
            properties.Add(getVariableData(AIR_DENSITY));
            properties.Add(getVariableData(V0));

            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path = Path.Combine(dir, "ReportTemplates\\ОтчетПожарПроливаШаблон.docx");

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
                    string savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ОтчетПожарПролива.docx");
                    using (WordprocessingDocument report = WordprocessingDocument.Create(savePath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
                    {
                        foreach (var part in template.Parts)
                            report.AddPart(part.OpenXmlPart, part.RelationshipId);
                        using (StreamWriter sw = new StreamWriter(report.MainDocumentPart.GetStream(FileMode.Create)))
                        {
                            sw.Write(docText);
                        }
                    }
                    MessageBox.Show($"Отчет успешно сформиирован!\nРасположение : {savePath}","Отчет успешно сформирован", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ошибка при формировании отчета!\n{e.Message}", "Уведомление об ошибке", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private KeyValuePair<string, string> getVariableData(double variable, [CallerArgumentExpression("variable")] string name = "value")
        {
            return KeyValuePair.Create(name, variable.ToString("#0.#####"));
        }

        #endregion
    }
}
