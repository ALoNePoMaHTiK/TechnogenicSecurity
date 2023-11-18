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
using DocumentFormat.OpenXml.Wordprocessing;
using System.Windows.Media.Imaging;

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
            Points = new Stack<System.Windows.Point>();
            BitmapImageSource = new BitmapImage();
        }

        private Stack<System.Windows.Point> Points;

        private string _PlanSourcePath;
        public string PlanSourcePath
        {
            get { return _PlanSourcePath; }
            set { _PlanSourcePath = value; OnPropertyChanged(); }
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

            string path = Path.Combine(dir, $"Plans\\Result_{Points.Count}.png");

            BitmapImage newBitmap = new BitmapImage();
            FileStream stream = File.OpenRead(path);

            newBitmap.BeginInit();
            newBitmap.CacheOption = BitmapCacheOption.OnLoad;
            newBitmap.StreamSource = stream;
            newBitmap.EndInit();
            stream.Close();
            stream.Dispose();
            BitmapImageSource = newBitmap;
            File.Delete(Path.Combine(dir, $"Plans\\Result_{Points.Count - 1}.png"));
            File.Delete(Path.Combine(dir, $"Plans\\Result_{Points.Count + 1}.png"));
        }

        private void DrawCircle()
        {
            Graphics graphics = Graphics.FromImage(CurrentBitmap);
            Pen pen = new Pen(System.Drawing.Color.Blue, 5);
            float radius = 100;
            System.Windows.Point point = Points.Pop();
            graphics.DrawEllipse(pen, (float)(point.X - radius/2), (float)(point.Y - radius/2),
                                          radius, radius);
            Points.Push(point);
            SavePlan();
            graphics.Dispose();
        }

        private void DrawCircles()
        {
            Graphics graphics = Graphics.FromImage(CurrentBitmap);
            Pen pen = new Pen(System.Drawing.Color.Blue, 5);
            float radius = 100;
            foreach (System.Windows.Point point in Points)
            {
                graphics.DrawEllipse(pen, (float)(point.X - radius / 2), (float)(point.Y - radius / 2),
                                      radius, radius);
            }
            SavePlan();
            graphics.Dispose();
        }

        private void UndoCircle()
        {
            if (Points.Count > 0)
            {
                Points.Pop();
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
            Points.Push(Mouse.GetPosition((IInputElement)target));
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
