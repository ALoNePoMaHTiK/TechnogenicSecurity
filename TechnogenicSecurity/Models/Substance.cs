namespace TechnogenicSecurity.Models
{
    public class Substance
    {
        public string SubstanceName { get; set; }
        public double MolarMass { get; set; }
        public double LCLS { get; set; } //lower concentration limit of (flame) spread НКПР
        public double UCLS { get; set; } //upper concentration limit of (flame) spread ВКПР
        public double GasExplosionEnergy { get; set; }         //энергия взрыва газа смеси
        public double GasAirMixExplosionEnergy { get; set; }   //энергия взрыва газо-воздушной смеси
        public double Density { get; set; } //плотность стехиометрической смеси
        public double HiddenVaporizationHeat { get; set; }  //Скрытая теплота испарения
        public double BoilingTemperature { get; set; }  //Температура кипения
        public double StoichiometricGasConcentration { get; set; }
    }
}
