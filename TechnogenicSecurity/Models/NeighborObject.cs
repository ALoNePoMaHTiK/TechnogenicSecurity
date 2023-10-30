using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnogenicSecurity.Models
{
    public class ObjectType
    {
        public string ObjectTypeName { get; set; }
        public double WeakDestructionRate { get; set; }
        public double MediumDestructionRate { get; set; }
        public double SevereDestructionRate { get; set; }
        public double FullDestructionRate { get; set; }
    }
}
