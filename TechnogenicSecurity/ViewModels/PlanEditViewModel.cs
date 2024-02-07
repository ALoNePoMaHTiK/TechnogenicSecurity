using System.ComponentModel;
using System.Runtime.CompilerServices;
using TechnogenicSecurity.Models;
using System.Drawing;
using System.IO;
using System.Windows.Input;
using System.Windows;
using Microsoft.Win32;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Windows.Media;
using DashStyle = System.Drawing.Drawing2D.DashStyle;
using Pen = System.Drawing.Pen;

namespace TechnogenicSecurity.ViewModels
{
    public class PlanEditViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public PlanEditViewModel()
        {
            if (!Directory.Exists(".\\Plans\\"))
                Directory.CreateDirectory("\\Plans\\");
            PlanSourcePath = "./Images/placeholder.png";
            DirectoryInfo di = new DirectoryInfo(".\\Plans\\");
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            CirclesStack = new Stack<PlanCircle>();
            BitmapImageSource = new BitmapImage();
            PenWidth = 5;

            
            DashStyles = new ObservableCollection<PenDashStyle>();
            DashStyles.Add(new PenDashStyle { DashStyle=DashStyle.Solid, Name="Прямая" });
            DashStyles.Add(new PenDashStyle { DashStyle=DashStyle.Dash, Name="Штрих" });
            DashStyles.Add(new PenDashStyle { DashStyle=DashStyle.DashDot, Name="Штрих-точка" });
            DashStyles.Add(new PenDashStyle { DashStyle=DashStyle.DashDotDot, Name="Штрих-точка-точка" });
            CurrentDashStyle = DashStyles[0];

            Colors = new ObservableCollection<System.Drawing.Color>();
            Colors.Add(System.Drawing.Color.LimeGreen);
            Colors.Add(System.Drawing.Color.DarkRed);
            Colors.Add(System.Drawing.Color.DarkGreen);
            Colors.Add(System.Drawing.Color.DarkOrchid);
            CurrentColor = Colors[0];
        }

        private Stack<PlanCircle> CirclesStack;

        private string _PlanSourcePath;
        public string PlanSourcePath
        {
            get { return _PlanSourcePath; }
            set { _PlanSourcePath = value; OnPropertyChanged(); }
        }

        private double _PenWidth;
        public double PenWidth
        {
            get { return _PenWidth; }
            set { _PenWidth = value; OnPropertyChanged(); }
        }

        private ObservableCollection<System.Drawing.Color> _Colors;
        public ObservableCollection<System.Drawing.Color> Colors
        {
            get { return _Colors; }
            set { _Colors = value; }
        }

        public ObservableCollection<PenDashStyle> _DashStyles;
        public ObservableCollection<PenDashStyle> DashStyles
        {
            get { return _DashStyles; }
            set { _DashStyles = value; }
        }
        private PenDashStyle _CurrentDashStyle;
        public PenDashStyle CurrentDashStyle
        {
            get { return _CurrentDashStyle; }
            set { _CurrentDashStyle = value; OnPropertyChanged(); }
        }

        private System.Drawing.Color _CurrentColor;
        public System.Drawing.Color CurrentColor
        {
            get { return _CurrentColor; }
            set { _CurrentColor = value; OnPropertyChanged(); }
        }

        private BitmapImage _BitmapImageSource;
        public BitmapImage BitmapImageSource
        {
            get { return _BitmapImageSource; }
            set { _BitmapImageSource = value; OnPropertyChanged(); }
        }

        public Bitmap CurrentBitmap;
        private Bitmap SourceBitmap;

        public DelegateCommand AddPointCommand
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    AddPoint(obj);
                    DrawCircle();
                });
            }
        }

        public DelegateCommand OpenDialogCommand
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    OpenDialog();
                });
            }
        }

        public DelegateCommand UndoCircleCommand
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    UndoCircle();
                });
            }
        }

        public DelegateCommand SavePlanCommand
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    OpenSaveFileDialog();
                });
            }
        }

        private void SavePlan()
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string path = Path.Combine(dir, $"Plans\\Result_{CirclesStack.Count}.png");

            CurrentBitmap.Save(path);
            BitmapImage newBitmap = new BitmapImage();
            FileStream stream = File.OpenRead(path);

            newBitmap.BeginInit();
            newBitmap.CacheOption = BitmapCacheOption.OnLoad;
            newBitmap.StreamSource = stream;
            newBitmap.EndInit();
            stream.Close();
            stream.Dispose();
            BitmapImageSource = newBitmap;
            File.Delete(Path.Combine(dir, $"Plans\\Result_{CirclesStack.Count - 1}.png"));
            File.Delete(Path.Combine(dir, $"Plans\\Result_{CirclesStack.Count + 1}.png"));
        }

        private void DrawCircle()
        {
            Graphics graphics = Graphics.FromImage(CurrentBitmap);
            PlanCircle circle = CirclesStack.Pop();
            Pen pen = new Pen(circle.Color, circle.Width);
            pen.DashStyle = CurrentDashStyle.DashStyle;
            float diameter = 100;

            graphics.DrawEllipse(pen, (float)(circle.Center.X - diameter / 2), (float)(circle.Center.Y - diameter / 2),
                                          diameter, diameter);
            CirclesStack.Push(circle);
            SavePlan();
            graphics.Dispose();
        }

        private void DrawCircles()
        {
            Graphics graphics = Graphics.FromImage(CurrentBitmap);
            
            float diameter = 100;
            foreach (PlanCircle circle in CirclesStack)
            {
                Pen pen = new Pen(circle.Color, circle.Width);
                pen.DashStyle = circle.DashStyle;
                graphics.DrawEllipse(pen, (float)(circle.Center.X - diameter / 2), (float)(circle.Center.Y - diameter / 2),
                                          diameter, diameter);
            }
            SavePlan();
            graphics.Dispose();
        }

        private void UndoCircle()
        {
            if (CirclesStack.Count > 0)
            {
                CirclesStack.Pop();
                CurrentBitmap = SourceBitmap.Clone(new Rectangle(0, 0, SourceBitmap.Width, SourceBitmap.Height), SourceBitmap.PixelFormat);
                DrawCircles();
            }
        }

        private void OpenDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                SourceBitmap = GetBitmap(openFileDialog.FileName);
                CurrentBitmap = SourceBitmap.Clone(new Rectangle(0, 0, SourceBitmap.Width, SourceBitmap.Height), SourceBitmap.PixelFormat);

                var newBitmap = new BitmapImage();
                var stream = File.OpenRead(openFileDialog.FileName);

                newBitmap.BeginInit();
                newBitmap.CacheOption = BitmapCacheOption.OnLoad;
                newBitmap.StreamSource = stream;
                newBitmap.EndInit();
                stream.Close();
                stream.Dispose();
                BitmapImageSource = newBitmap;
            }
        }

        private void OpenSaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                CurrentBitmap.Save(saveFileDialog.FileName);
            }
        }

        private void AddPoint(object target)
        {
            CirclesStack.Push(new PlanCircle { Center= Mouse.GetPosition((IInputElement)target) ,Color=CurrentColor,Width=(float)PenWidth, DashStyle= CurrentDashStyle.DashStyle});
        }
        private Bitmap GetBitmap(string fileName)
        {
            Bitmap bitmap;
            using (Stream bmpStream = File.Open(fileName, FileMode.Open))
            {
                Image image = Image.FromStream(bmpStream);
                bitmap = new Bitmap(image);
                image.Dispose();
            }
            return bitmap;
        }
    }
}
