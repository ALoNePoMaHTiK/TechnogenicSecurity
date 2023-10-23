﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TechnogenicSecurity.Models
{
    public class ExplosionCalculation
    {
        public double FirstCloudMass { get; set; }
        public double EvaporationRate { get; set; }
        public double SaturatedSteamPressure { get; set; }
        public double SecondCloudMass { get; set; }
        public double TotalVaporMass { get; set; }
        public double GasDensity { get; set; }
        public double BlastingCloudRadius { get; set; }
        public double DetonationAreaRedius { get; set; }
        public double ReducedVaporMass { get; set; }
        public double WaveFrontExcessivePressure { get; set; }
        public double NeighborBuildingExcessivePressure { get; set; }
        public double ShockWaveCompressionPhaseImpulse { get; set; }
        public double WeakDestructionProbability { get; set; }
        public double MediumDestructionProbability { get; set; }
        public double SevereDestructionProbability { get; set; }
        public double HumanLosses1 { get; set; }
        public double HumanLosses2 { get; set; }
        public double TotalHumanLosses { get; set; }
    }
}