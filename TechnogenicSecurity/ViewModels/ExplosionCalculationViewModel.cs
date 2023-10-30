﻿using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;
using TechnogenicSecurity.Models;

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
            NeighborObjects = CatalogAdministrator.getNeigborObjects();
            StoringMethods = CatalogAdministrator.getStoringMethods();
            StoringMethod = StoringMethods[0];
            NeighborObject = NeighborObjects[0];
            Substance = Substances[0];

            SubstanceVolume = 500;
            EvaporationArea = 400;
            NeighborBuildingDistance = 50;
            CoolingTemperature = 300;
            FillingLevel = 0.8;

            PeopleCount = 10;
            StaffDensity = 900;
            Calculate();
        }

        #region Справочники

        private ObservableCollection<Substance> _Substances;

        public ObservableCollection<Substance> Substances
        {
            get { return _Substances; }
            set { _Substances = value; }
        }

        private ObservableCollection<ObjectType> _ObjectsTypes;

        public ObservableCollection<ObjectType> ObjectsTypes
        {
            get { return _ObjectsTypes; }
            set { _ObjectsTypes = value; }
        }

        private ObservableCollection<StoringMethod> _StoringMethods;

        public ObservableCollection<StoringMethod> StoringMethods
        {
            get { return _StoringMethods; }
            set { _StoringMethods = value; }
        }

        private ObjectType _ObjectType;
        public ObjectType ObjectType
        {
            get { return _ObjectType; }
            set { _ObjectType = value; OnPropertyChanged(); }
        }

        private StoringMethod _StoringMethod;
        public StoringMethod StoringMethod
        {
            get { return _StoringMethod; }
            set { _StoringMethod = value; OnPropertyChanged(); }
        }

        #endregion

        #region Хар-ки вещества

        private Substance _Substance;
        public Substance Substance
        {
            get { return _Substance; }
            set { _Substance = value; OnPropertyChanged(); }
        }

        private double _SubstanceVolume;
        public double SubstanceVolume
        {
            get { return _SubstanceVolume; }
            set { _SubstanceVolume = value; OnPropertyChanged(); }
        }
        private double _EvaporationArea;
        public double EvaporationArea
        {
            get { return _EvaporationArea; }
            set { _EvaporationArea = value; OnPropertyChanged(); }
        }

        private double _NeighborBuildingDistance;
        public double NeighborBuildingDistance
        {
            get { return _NeighborBuildingDistance; }
            set { _NeighborBuildingDistance = value; OnPropertyChanged(); }
        }

        private double _CoolingTemperature;
        public double CoolingTemperature
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

        private ExplosionCalculationResult _ExplosionCalculationResults;
        public ExplosionCalculationResult Results
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
            ExplosionCalculationResult newResults = new ExplosionCalculationResult();
            newResults.FirstCloudMass = GasolineVaporVolume * Substance.MolarMass * (SubstanceVolume * AtmosphericPressure) / (R * CoolingTemperature);
            newResults.SaturatedSteamPressure = AtmosphericPressure*Math.Exp(Substance.HiddenVaporizationHeat * Substance.MolarMass * (Math.Pow(Substance.BoilingTemperature,-1) - Math.Pow(CoolingTemperature,-1))/((double)R/1000))/1000;
            newResults.EvaporationRate = (Math.Pow(10, -6) * newResults.SaturatedSteamPressure * Math.Sqrt(Substance.MolarMass));
            newResults.SecondCloudMass = newResults.EvaporationRate * EvaporationArea * 3600;
            newResults.TotalVaporMass = newResults.FirstCloudMass + newResults.SecondCloudMass;
            newResults.GasDensity = Substance.MolarMass / (V0 * (1 + 0.00367 * AirTemperature));
            newResults.BlastingCloudRadius = 3.1501 * Math.Sqrt(EvaporationTime / 3600) * Math.Pow(newResults.SaturatedSteamPressure / Substance.LCLS, 0.813) * Math.Pow(newResults.TotalVaporMass / (newResults.GasDensity * newResults.SaturatedSteamPressure),1.0/3.0);
            newResults.DetonationAreaRedius = 10 * Math.Pow(newResults.TotalVaporMass * StoringMethod.Value / (Substance.MolarMass * Substance.StoichiometricGasConcentration),1.0/3.0);
            newResults.ReducedVaporMass = (46200.0 / 4520) * newResults.TotalVaporMass * Z;
            newResults.WaveFrontExcessivePressure = 81 * Math.Pow(newResults.ReducedVaporMass, 1.0 / 3) / newResults.BlastingCloudRadius + 303 * Math.Pow(newResults.ReducedVaporMass, 2.0 / 3.0) / Math.Pow(newResults.BlastingCloudRadius,2) + 505 * newResults.ReducedVaporMass / Math.Pow(newResults.BlastingCloudRadius, 3);

            newResults.WeakDestructionRadius = getBlastingRadius(ObjectType.WeakDestructionRate, newResults.ReducedVaporMass);
            newResults.MediumDestructionRadius = getBlastingRadius(ObjectType.MediumDestructionRate, newResults.ReducedVaporMass);
            newResults.SevereDestructionRadius = getBlastingRadius(ObjectType.SevereDestructionRate, newResults.ReducedVaporMass);
            newResults.FullDestructionRadius = getBlastingRadius(ObjectType.FullDestructionRate, newResults.ReducedVaporMass);

            newResults.NeighborBuildingExcessivePressure = 81 * (Math.Pow(newResults.ReducedVaporMass, 1.0 / 3) / NeighborBuildingDistance) + 303 * (Math.Pow(newResults.ReducedVaporMass, 2.0 / 3.0) / Math.Pow(NeighborBuildingDistance, 2)) + 505 * newResults.ReducedVaporMass / Math.Pow(NeighborBuildingDistance, 3);
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
            IDictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add(getVariableData(Substance.MolarMass));
            properties.Add(getVariableData(SubstanceVolume));
            properties.Add(getVariableData(Substance.HiddenVaporizationHeat));
            properties.Add(getVariableData(Substance.BoilingTemperature));
            properties.Add(getVariableData(Substance.LCLS));
            properties.Add(getVariableData(Substance.UCLS));
            properties.Add(getVariableData(Substance.GasExplosionEnergy));
            properties.Add(getVariableData(Substance.GasAirMixExplosionEnergy));
            properties.Add(getVariableData(Substance.Density));

            properties.Add(getVariableData(Results.FirstCloudMass));
            properties.Add(getVariableData(Results.SaturatedSteamPressure));
            properties.Add(getVariableData(Results.EvaporationRate));
            properties.Add(getVariableData(Results.SecondCloudMass));
            properties.Add(getVariableData(Results.TotalVaporMass));
            properties.Add(getVariableData(Results.GasDensity));
            properties.Add(getVariableData(Results.BlastingCloudRadius));
            properties.Add(getVariableData(Results.ReducedVaporMass));
            properties.Add(getVariableData(Results.WaveFrontExcessivePressure));
            properties.Add(getVariableData(Results.NeighborBuildingExcessivePressure));
            properties.Add(getVariableData(Results.ShockWaveCompressionPhaseImpulse));
            properties.Add(getVariableData(Results.WeakDestructionProbability));
            properties.Add(getVariableData(Results.MediumDestructionProbability));
            properties.Add(getVariableData(Results.SevereDestructionProbability));
            properties.Add(getVariableData(Results.HumanLosses1));
            properties.Add(getVariableData(Results.HumanLosses2));
            properties.Add(getVariableData(Results.TotalHumanLosses));

            properties.Add(getVariableData(EvaporationArea));
            properties.Add(getVariableData(EvaporationTime));
            properties.Add(getVariableData(AirTemperature));
            properties.Add(getVariableData(GasolineVaporVolume));
            properties.Add(getVariableData(AtmosphericPressure));
            properties.Add(getVariableData(CoolingTemperature));
            properties.Add(getVariableData(R));
            properties.Add(getVariableData(V0));

            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path = Path.Combine(dir, "ReportTemplates\\ОтчетВзрывШаблон.docx");

            using (WordprocessingDocument template = WordprocessingDocument.Open(path, true))
            {
                string docText = null;
                using (StreamReader sr = new StreamReader(template.MainDocumentPart.GetStream()))
                {
                    docText = sr.ReadToEnd();
                }
                using (StreamWriter writer = new StreamWriter("file.txt", false))
                {
                    writer.WriteLine(docText);
                }
                foreach (var prop in properties)
                {
                    docText = docText.Replace(prop.Key, prop.Value);
                }
                string savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ОтчетВзрыв.docx");
                using (WordprocessingDocument report = WordprocessingDocument.Create(savePath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
                {
                    foreach (var part in template.Parts)
                        report.AddPart(part.OpenXmlPart, part.RelationshipId);
                    using (StreamWriter sw = new StreamWriter(report.MainDocumentPart.GetStream(FileMode.Create)))
                    {
                        sw.Write(docText);
                    }
                }
                
            }
        }

        private KeyValuePair<string, string> getVariableData(object variable, [CallerArgumentExpression("variable")] string name = "value")
        {
            return KeyValuePair.Create(name, variable.ToString());
        }

        private double getBlastingRadius(double WaveFrontExcessivePressure, double ReducedVaporMass)
        {
            double minRadius = 100;
            double minOffset = 100;
            for (double radius = 5; radius <= 1000; radius += 0.25)
            {
                double p = 81 * Math.Pow(ReducedVaporMass, 1.0 / 3) / radius + 303 * Math.Pow(ReducedVaporMass, 2.0 / 3.0) / Math.Pow(radius, 2) + 505 * ReducedVaporMass / Math.Pow(radius, 3);
                if (Math.Abs(22.0 - p) < 1)
                {
                    if (Math.Abs(22.0 - p) < minOffset)
                    {
                        minOffset = Math.Abs(22.0 - p);
                        minRadius = radius;
                    }
                }
            }
            return minRadius;
        }
    }
}
