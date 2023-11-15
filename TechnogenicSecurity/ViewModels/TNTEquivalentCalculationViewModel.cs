using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
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
    public class TNTEquivalentCalculationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public TNTEquivalentCalculationViewModel() 
        {
            ExplosiveEnergyPotential = 60100000;

            Coefs = new TNTDestructionDTO[]
            {
                new TNTDestructionDTO
                {
                    Description = "Полное разрушение зданий с массивными стенами",
                    K = 3.8
                },new TNTDestructionDTO
                {
                    Description = "Разрушение стен кирпичных зданий толщиной в 1,5 кирпича; перемещение цилиндрических резервуаров; разрушение трубопроводных эстакад",
                    K = 5.6
                },new TNTDestructionDTO
                {
                    Description = "Разрушение перекрытий промышленных зданий; разрушение промышленных стальных несущих конструкций; деформации трубопроводных эстакад",
                    K = 9.6
                },new TNTDestructionDTO
                {
                    Description = "Разрушение перегородок и кровли зданий; повреждение стальных конструкций каркасов, ферм",
                    K = 28
                },new TNTDestructionDTO
                {
                    Description = "Граница зоны повреждений зданий; частичное повреждение остекления",
                    K = 56
                }
            };

            SubstanceTypes = new ObservableCollection<SubstanceType>();
            SubstanceTypes.Add(new SubstanceType { Name = "Водород", Value = 1.0 });
            SubstanceTypes.Add(new SubstanceType { Name = "Горючие газы", Value = 0.5 });
            SubstanceTypes.Add(new SubstanceType { Name = "Пары легковоспламеняющихся и горючих жидкостей", Value = 0.3 });
            SubstanceTypes.Add(new SubstanceType { Name = "Общий случай", Value = 0.1 });

            SubstanceType = SubstanceTypes[3];

            Calculate();
        }

        public ObservableCollection<SubstanceType> SubstanceTypes { get; set; }

        private SubstanceType _SubstanceType;
        public SubstanceType SubstanceType
        {
            get { return _SubstanceType; }
            set { _SubstanceType = value; OnPropertyChanged(); }
        }

        private TNTEquivalentCalculation _Results;
        public TNTEquivalentCalculation Results
        {
            get { return _Results; }
            set { _Results = value; OnPropertyChanged(); }
        }
        private double _ExplosiveEnergyPotential;
        public double ExplosiveEnergyPotential
        {
            get { return _ExplosiveEnergyPotential; }
            set { _ExplosiveEnergyPotential = value; OnPropertyChanged(); }
        }


        public const int TNTSpecificExplosionEnergy = 4520;

        public TNTDestructionDTO[] Coefs { get; set; }


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

        private void Calculate()
        { 
            TNTEquivalentCalculation newResults = new TNTEquivalentCalculation();
            newResults.FlammableVaporsMass = ExplosiveEnergyPotential / 46000;
            newResults.TNTEquivalent = (0.4 * 46000) / (0.9 * TNTSpecificExplosionEnergy) * SubstanceType.Value * newResults.FlammableVaporsMass;

            newResults.Destructions = new ObservableCollection<TNTDestructionDTO>();
            for (int i = 0; i < Coefs.Length; i++)
            {
                newResults.Destructions.Add(new TNTDestructionDTO
                {
                    K = Coefs[i].K,
                    Description = Coefs[i].Description,
                    Radius = Coefs[i].K * Math.Pow(newResults.TNTEquivalent,1.0/3)/Math.Pow((1+Math.Pow(3180/newResults.TNTEquivalent,2)),1.0/6)
                });
            }
            
            Results = newResults;
        }
    }
}
