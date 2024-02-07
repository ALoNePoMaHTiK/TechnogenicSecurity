using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TechnogenicSecurity.Models;

namespace TechnogenicSecurity.ViewModels
{
    public class EnergyPotentialViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public EnergyPotentialViewModel()
        {
            Coefs = CatalogAdministrator.getAirSpeedAndTemperatureFlowCoefficients();
            MolarMass = 93;
            ColumnVolume = 125.6;
            LiquidPhaseVolume = 6.9;
            ColumnPressure = 500;
            ColumnTemperature = 408;
            EnvironmentTemperature = 293;   //Добавить модель для хранения температуры
            LiquidPhaseDensity = 640;
            LiquidPhaseHeatCapacity = 2.2;
            LiquidPhaseBoilingTemperature = 347;
            VaporizationHeat = 344.1;
            CombustionHeat = 46000;

            IndoorAirSpeed = 0;
            IndoorTemperature = 10;

            Calculate();
        }

        private void Calculate()
        {
            EnergyPotentialCalculation newResults = new EnergyPotentialCalculation();
            newResults.ColumnGasPhaseVolume = ColumnVolume - LiquidPhaseVolume;
            newResults.GasPhaseVolume = ColumnPressure * newResults.ColumnGasPhaseVolume * EnvironmentTemperature / (AtmosphericPressure * ColumnTemperature);
            newResults.GasPhaseDensity = MolarMass / (Z * (1 + 0.00367 * (EnvironmentTemperature-273)));
            newResults.GasPhaseMass = newResults.GasPhaseVolume * newResults.GasPhaseDensity;
            newResults.ExtensionWork = newResults.GasPhaseVolume*ColumnPressure*1.53;       //ИЗМЕНИТЬ
            newResults.BurnoutEnergy1 = newResults.GasPhaseMass*CombustionHeat + newResults.ExtensionWork;
            newResults.LiquidPhaseMass = LiquidPhaseVolume * LiquidPhaseDensity;
            newResults.TemperatureDifference = ColumnTemperature - LiquidPhaseBoilingTemperature;
            newResults.BurnoutEnergy2 = newResults.LiquidPhaseMass * (1 - Math.Exp(-LiquidPhaseHeatCapacity * newResults.TemperatureDifference / VaporizationHeat))*CombustionHeat;
            newResults.EvaporatedLiquidMass1 = newResults.BurnoutEnergy2/ CombustionHeat;

            newResults.LiquidPhaseMassRemainder = newResults.LiquidPhaseMass - newResults.EvaporatedLiquidMass1;
            newResults.LiquidPhaseVolumeRemainder = newResults.LiquidPhaseMassRemainder/ LiquidPhaseDensity;
            newResults.EvaporationArea = newResults.LiquidPhaseVolumeRemainder * 0.15 * 1000;
            newResults.SaturatedSteamPressure = AtmosphericPressure * Math.Exp((VaporizationHeat * MolarMass) * (Math.Pow(LiquidPhaseBoilingTemperature, -1) - Math.Pow(EnvironmentTemperature, -1)) / R);
            newResults.EvaporationSpeed = Math.Pow(10,-6)*AirSpeedAndTemperatureFlowCoefficient* newResults.SaturatedSteamPressure * Math.Sqrt(MolarMass);
            newResults.EvaporatedLiquidMass2 = newResults.EvaporationSpeed * newResults.EvaporationArea * 1800;
            newResults.BurnoutEnergy3 = newResults.EvaporatedLiquidMass2 * CombustionHeat;

            newResults.ColumnEnergyPotential = newResults.BurnoutEnergy1 + newResults.BurnoutEnergy2 + newResults.BurnoutEnergy3;
            newResults.GasSum = newResults.ColumnEnergyPotential / CombustionHeat;
            newResults.RelativeEnergyExplosivePotential = Math.Cbrt(newResults.ColumnEnergyPotential) / 16.534;
            if (newResults.RelativeEnergyExplosivePotential > 37 && newResults.GasSum > 5000)
                newResults.ExplosivenessCategory = 1;
            else if (newResults.RelativeEnergyExplosivePotential <27 && newResults.GasSum < 2000)
                newResults.ExplosivenessCategory = 3;
            else
                newResults.ExplosivenessCategory = 2;
            Results = newResults;
        }

        private void GenerateReport()
        {
            throw new NotImplementedException();
        }

        private void getAirSpeedAndTemperatureFlowCoefficient()
        {
            foreach (var coef in Coefs)
            {
                if (coef.IndoorAirSpeed == IndoorAirSpeed && coef.IndoorTemperature == IndoorTemperature)
                    AirSpeedAndTemperatureFlowCoefficient = coef.AirSpeedAndTemperatureFlowCoefficient;
            }
        }

        const double AtmosphericPressure = 101.3;
        const double Z = 22.4;
        const double R = 8.31;

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

        #endregion

        private ObservableCollection<AirSpeedAndTemperatureFlowCoefficientDTO> _Coefs;
        public ObservableCollection<AirSpeedAndTemperatureFlowCoefficientDTO> Coefs
        {
            get { return _Coefs; }
            set { _Coefs = value; }
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

        private double _AirSpeedAndTemperatureFlowCoefficient;
        public double AirSpeedAndTemperatureFlowCoefficient
        {
            get { return _AirSpeedAndTemperatureFlowCoefficient; }
            set { _AirSpeedAndTemperatureFlowCoefficient = value; OnPropertyChanged(); }
        }

        private EnergyPotentialCalculation _Results;
        public EnergyPotentialCalculation Results
        {
            get { return _Results; }
            set { _Results = value; OnPropertyChanged(); }
        }

        private double _MolarMass;
        public double MolarMass
        {
            get { return _MolarMass; }
            set { _MolarMass = value; OnPropertyChanged(); }
        }

        //Объем колонны
        private double _ColumnVolume;
        public double ColumnVolume
        {
            get { return _ColumnVolume; }
            set { _ColumnVolume = value; OnPropertyChanged(); }
        }

        //Объем жидкой фазы
        private double _LiquidPhaseVolume;
        public double LiquidPhaseVolume
        {
            get { return _LiquidPhaseVolume; }
            set { _LiquidPhaseVolume = value; OnPropertyChanged(); }
        }

        //Давление в колонне
        private double _ColumnPressure;
        public double ColumnPressure
        {
            get { return _ColumnPressure; }
            set { _ColumnPressure = value; OnPropertyChanged(); }
        }

        //Температура в колонне (средняя)
        private double _ColumnTemperature;
        public double ColumnTemperature
        {
            get { return _ColumnTemperature; }
            set { _ColumnTemperature = value; OnPropertyChanged(); }
        }

        //Температура окр. среды
        private double _EnvironmentTemperature;
        public double EnvironmentTemperature
        {
            get { return _EnvironmentTemperature; }
            set { _EnvironmentTemperature = value; OnPropertyChanged(); }
        }

        //Плотность ЖФ
        private double _LiquidPhaseDensity;
        public double LiquidPhaseDensity
        {
            get { return _LiquidPhaseDensity; }
            set { _LiquidPhaseDensity = value; OnPropertyChanged(); }
        }

        //Теплоемкость ЖФ
        private double _LiquidPhaseHeatCapacity;
        public double LiquidPhaseHeatCapacity
        {
            get { return _LiquidPhaseHeatCapacity; }
            set { _LiquidPhaseHeatCapacity = value; OnPropertyChanged(); }
        }

        //Температура кипения ЖФ при Р0 = 1атм
        private double _LiquidPhaseBoilingTemperature;
        public double LiquidPhaseBoilingTemperature
        {
            get { return _LiquidPhaseBoilingTemperature; }
            set { _LiquidPhaseBoilingTemperature = value; OnPropertyChanged(); }
        }

        //Теплота испарения
        private double _VaporizationHeat;
        public double VaporizationHeat
        {
            get { return _VaporizationHeat; }
            set { _VaporizationHeat = value; OnPropertyChanged(); }
        }

        //Теплота сгорания
        private double _CombustionHeat;
        public double CombustionHeat
        {
            get { return _CombustionHeat; }
            set { _CombustionHeat = value; OnPropertyChanged(); }
        }
    }
}
