using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
            SubstanceVolume = 500;
            EvaporationArea = 400;
            NeighborBuildingDistance = 50;
            CoolingTemperature = 300;
            LiquidDensity = 740;
            MolarMass = 94;
            SpecificEvaporationHeat = 287.3; 
            BoilingTemperature = 413;
            FillingLevel = 0.8;
            LowerConcentrationLimit = 0.04;

            PeopleCount = 10;
            StaffDensity = 900;
        }

        #region Хар-ки вещества

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

        private int _LiquidDensity;
        public int LiquidDensity
        {
            get { return _LiquidDensity; }
            set { _LiquidDensity = value; OnPropertyChanged(); }
        }

        private int _MolarMass;
        public int MolarMass
        {
            get { return _MolarMass; }
            set { _MolarMass = value; OnPropertyChanged(); }
        }

        private double _SpecificEvaporationHeat;//скрытая теплота испарения
        public double SpecificEvaporationHeat
        {
            get { return _SpecificEvaporationHeat; }
            set { _SpecificEvaporationHeat = value; OnPropertyChanged(); }
        }

        private int _BoilingTemperature;
        public int BoilingTemperature
        {
            get { return _BoilingTemperature; }
            set { _BoilingTemperature = value; OnPropertyChanged(); }
        }

        private double _FillingLevel;
        public double FillingLevel
        {
            get { return _FillingLevel; }
            set { _FillingLevel = value; OnPropertyChanged(); }
        }

        private double _LowerConcentrationLimit;
        public double LowerConcentrationLimit
        {
            get { return _LowerConcentrationLimit; }
            set { _LowerConcentrationLimit = value; OnPropertyChanged(); }
        }
        #endregion

        #region Хар-ки персонала

        private int _PeopleCount;
        public int PeopleCount
        {
            get { return _PeopleCount; }
            set { _PeopleCount = value; OnPropertyChanged(); }
        }

        private int _StaffDensity;
        public int StaffDensity
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

        #region Расчитываемые значения

        private double _FirstCloudMass;
        public double FirstCloudMass
        {
            get { return _FirstCloudMass; }
            set { _FirstCloudMass = value; OnPropertyChanged(); }
        }

        private double _EvaporationRate;
        public double EvaporationRate
        {
            get { return _EvaporationRate; }
            set { _EvaporationRate = value; OnPropertyChanged(); }
        }

        private double _SaturatedSteamPressure;
        public double SaturatedSteamPressure
        {
            get { return _SaturatedSteamPressure; }
            set { _SaturatedSteamPressure = value; OnPropertyChanged(); }
        }

        private double _SecondCloudMass;
        public double SecondCloudMass
        {
            get { return _SecondCloudMass; }
            set { _SecondCloudMass = value; OnPropertyChanged(); }
        }

        private double _TotalVaporMass;
        public double TotalVaporMass
        {
            get { return _TotalVaporMass; }
            set { _TotalVaporMass = value; OnPropertyChanged(); }
        }

        private double _GasDensity;
        public double GasDensity
        {
            get { return _GasDensity; }
            set { _GasDensity = value; OnPropertyChanged(); }
        }

        private double _BlastingCloudRadius;
        public double BlastingCloudRadius
        {
            get { return _BlastingCloudRadius; }
            set { _BlastingCloudRadius = value; OnPropertyChanged(); }
        }

        private double _DetonationAreaRedius;
        public double DetonationAreaRedius
        {
            get { return _DetonationAreaRedius; }
            set { _DetonationAreaRedius = value; OnPropertyChanged(); }
        }

        private double _ReducedVaporMass;
        public double ReducedVaporMass
        {
            get { return _ReducedVaporMass; }
            set { _ReducedVaporMass = value; OnPropertyChanged(); }
        }

        private double _WaveFrontExcessivePressure;
        public double WaveFrontExcessivePressure
        {
            get { return _WaveFrontExcessivePressure; }
            set { _WaveFrontExcessivePressure = value; OnPropertyChanged(); }
        }

        private double _NeighborBuildingExcessivePressure;
        public double NeighborBuildingExcessivePressure
        {
            get { return _NeighborBuildingExcessivePressure; }
            set { _NeighborBuildingExcessivePressure = value; OnPropertyChanged(); }
        }

        #endregion

        public DelegateCommand CalculateValues
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    Calculate();
                });
            }
        }

        private void Calculate()
        {
            FirstCloudMass = GasolineVaporVolume * MolarMass * (SubstanceVolume * AtmosphericPressure) / (R * CoolingTemperature);
            SaturatedSteamPressure = AtmosphericPressure*Math.Exp(SpecificEvaporationHeat*MolarMass*(Math.Pow(BoilingTemperature,-1) - Math.Pow(CoolingTemperature,-1))/((double)R/1000))/1000;
            EvaporationRate = (Math.Pow(10, -6) * SaturatedSteamPressure * Math.Sqrt(MolarMass));
            SecondCloudMass = EvaporationRate * EvaporationArea * 3600;
            TotalVaporMass = FirstCloudMass + SecondCloudMass;
            GasDensity = MolarMass / (V0 * (1 + 0.00367 * AirTemperature));
            BlastingCloudRadius = 3.1501 * Math.Sqrt(EvaporationTime / 3600) * Math.Pow(SaturatedSteamPressure/LowerConcentrationLimit, 0.813) * Math.Pow(TotalVaporMass / (GasDensity*SaturatedSteamPressure),1.0/3.0);
            DetonationAreaRedius = 10 * Math.Pow(TotalVaporMass * 0.06 / (MolarMass * 2.1),1.0/3.0);
            ReducedVaporMass = (46200.0 / 4520) * TotalVaporMass * Z;
            WaveFrontExcessivePressure = 81 * Math.Pow(ReducedVaporMass, 1.0 / 3.0) / BlastingCloudRadius + 303 * Math.Pow(ReducedVaporMass, 2.0 / 3.0) / Math.Pow(BlastingCloudRadius,2) + 505 * ReducedVaporMass / Math.Pow(BlastingCloudRadius, 3);
            NeighborBuildingExcessivePressure = 81 * (Math.Pow(ReducedVaporMass, 1.0 / 3.0) / NeighborBuildingDistance) + 303 * (Math.Pow(ReducedVaporMass, 2.0 / 3.0) / Math.Pow(NeighborBuildingDistance, 2)) + 505 * ReducedVaporMass / Math.Pow(NeighborBuildingDistance, 3);

        }
    }
}
