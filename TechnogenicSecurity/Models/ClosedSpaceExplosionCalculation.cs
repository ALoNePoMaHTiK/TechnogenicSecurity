using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnogenicSecurity.Models
{
    public class ClosedSpaceExplosionCalculation
    {
        public double OilVolumeFromPipeline { get; set; }
        public double EnteringOilVolume { get; set; }
        public double RoomArea { get; set; }
        public double PumpsArea { get; set; }
        public double FreeRoomArea { get; set; }
        public double SubstanceLayer { get; set; }
        public double SaturatedSteamPressure { get; set; }
        public double EvaporationRate { get; set; }
        public double EmergencySpillVaporMass{ get; set; }
        public double SpilledOilMass{ get; set; }
        public double EvaporatedOilPersent{ get; set; }
        public double VaporDensity{ get; set; }
        public double ShockWaveExcessivePressure { get; set; }
    }
}
