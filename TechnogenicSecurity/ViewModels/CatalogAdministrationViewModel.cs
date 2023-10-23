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
    public class CatalogAdministrationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public CatalogAdministrationViewModel()
        {
            Substances = CatalogAdministrator.getSubstances();
            Substance = Substances[0];
        }

        private ObservableCollection<Substance> _Substances;

        public ObservableCollection<Substance> Substances
        {
            get { return _Substances; }
            set { _Substances = value; }
        }

        private Substance _Substance;
        public Substance Substance
        {
            get { return _Substance; }
            set { _Substance = value; OnPropertyChanged(); }
        }
    }
}
