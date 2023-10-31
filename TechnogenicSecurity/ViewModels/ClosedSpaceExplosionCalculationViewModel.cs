using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
            SubstanceTemperature = 22.4;
            AutoShutdownTime = 120;

            Calculate();
        }

        private ObservableCollection<Substance> _Substances;

        public ObservableCollection<Substance> Substances
        {
            get { return _Substances; }
            set { _Substances = value; }
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
            set { _IndoorAirSpeed = value; OnPropertyChanged(); }
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

        #region Константы

        const double AtmosphericPressure = 101.3;
        const double R = 8310;

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
            newResults.EvaporationRate = (Math.Pow(10, -6) * newResults.SaturatedSteamPressure * 7.7 * Math.Sqrt(Substance.MolarMass));
            newResults.EmergencySpillVaporMass = newResults.EvaporationRate * newResults.FreeRoomArea * 3600;

            Results = newResults;
        }
    }
}
