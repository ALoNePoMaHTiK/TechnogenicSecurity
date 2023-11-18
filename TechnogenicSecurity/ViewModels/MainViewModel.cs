using TechnogenicSecurity.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;

namespace TechnogenicSecurity.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ExplosionCalculationViewModel explosionCalculationViewModel { get; set; }
        public StraitFireCalculationViewModel straitFireCalculationViewModel { get; set; }
        public CatalogAdministrationViewModel catalogAdministrationViewModel { get; set; }
        public ClosedSpaceExplosionCalculationViewModel closedSpaceExplosionCalculationViewModel { get; set; }
        public TNTEquivalentCalculationViewModel TNTEquivalentCalculationViewModel { get; set; }
        public PlanEditViewModel planEditViewModel { get; set; }

        public MainViewModel() 
        {
            explosionCalculationViewModel = new ExplosionCalculationViewModel();
            straitFireCalculationViewModel = new StraitFireCalculationViewModel();
            catalogAdministrationViewModel = new CatalogAdministrationViewModel();
            closedSpaceExplosionCalculationViewModel = new ClosedSpaceExplosionCalculationViewModel();
            TNTEquivalentCalculationViewModel = new TNTEquivalentCalculationViewModel();
            planEditViewModel = new PlanEditViewModel();
        }  
    }
}