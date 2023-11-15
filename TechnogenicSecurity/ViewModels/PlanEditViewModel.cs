using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using TechnogenicSecurity.Models;

namespace TechnogenicSecurity.ViewModels
{
    public class PlanEditViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private Point _Position;
        public Point Position
        {
            get { return _Position; }
            set { _Position = value; OnPropertyChanged(); }
        }

        public DelegateCommand GetPositionCommand
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    GetMousePosition(obj);
                });
            }
        }

        private void GetMousePosition(object target)
        {
            Position = Mouse.GetPosition((IInputElement)target);
        }
    }
}
