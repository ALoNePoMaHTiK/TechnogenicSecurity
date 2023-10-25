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
    public class StraitFireCalculationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public StraitFireCalculationViewModel()
        {
            Substances = CatalogAdministrator.getSubstances();
            Substance = Substances[3];

            TankRadius = 28;
            TankHeight = 30;

            ShellHeight = 9;
            FluxDensity = 60;
            WindSpeed = 5;
            LowerHeatingValue = 40000;
            TankFillLevel = 0.8;
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

        private double _TankRadius;
        public double TankRadius
        {
            get { return _TankRadius; }
            set { _TankRadius = value; OnPropertyChanged(); }
        }

        private double _TankHeight;
        public double TankHeight
        {
            get { return _TankHeight; }
            set { _TankHeight = value; OnPropertyChanged(); }
        }

        #endregion

        #region Общие характеристики

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
        double V0 = 22.4;
        double TP = 22.4;
        double g = 9.8;
        double AIR_DENSITY = 1.29;
        double u = 5; // характерное время обнаружения пожара
        double C = 0.00000125; //коэффициент пропорциональности

        #endregion

        #region Комманды

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

        #endregion
    }
}
