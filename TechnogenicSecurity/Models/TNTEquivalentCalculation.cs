using System.Collections.ObjectModel;

namespace TechnogenicSecurity.Models
{
    public class TNTEquivalentCalculation
    {

        public double TNTEquivalent { get; set; }
        public double FlammableVaporsMass { get; set; }
        public ObservableCollection<TNTDestructionDTO> Destructions { get; set; }
        
    }
}
