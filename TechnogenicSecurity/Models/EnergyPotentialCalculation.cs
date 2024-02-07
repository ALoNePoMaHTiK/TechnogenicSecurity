
using DocumentFormat.OpenXml.Vml.Office;

namespace TechnogenicSecurity.Models
{
    public class EnergyPotentialCalculation
    {
        public double ColumnGasPhaseVolume { get; set; }
        public double GasPhaseVolume { get; set; }
        public double GasPhaseDensity { get; set; }
        public double GasPhaseMass { get; set; }
        public double ExtensionWork { get; set; }
        public double BlockEnergySum { get; set; }
        public double BurnoutEnergy1 { get; set; }
        public double LiquidPhaseMass { get; set; }
        public double TemperatureDifference { get; set; }
        public double BurnoutEnergy2 { get; set; }
        public double EvaporatedLiquidMass1 { get; set; }
        public double LiquidPhaseMassRemainder { get; set; }
        public double LiquidPhaseVolumeRemainder { get; set; }
        public double EvaporationArea { get; set; }
        public double SaturatedSteamPressure { get; set; }
        public double EvaporationSpeed { get; set; }
        public double EvaporatedLiquidMass2 { get; set; }
        public double BurnoutEnergy3 { get; set; }
        public double ColumnEnergyPotential { get; set; }
        public double GasSum { get; set; }
        public double RelativeEnergyExplosivePotential { get; set; }
        public double ExplosivenessCategory { get; set; }
    }
}
