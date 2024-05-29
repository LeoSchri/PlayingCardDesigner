using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PlayingCardDesigner.Models
{
    [Serializable]
    public class PlayingCardDesign:INotifyPropertyChanged
    {
        private double height;
        public double Height
        {
            get { return height; }
            set
            {
                height = value;
                OnPropertyChanged();
            }
        }

        private double width;
        public double Width
        {
            get { return width; }
            set
            {
                width = value;
                OnPropertyChanged();
            }
        }

        private BorderedRectangle background;
        public BorderedRectangle Background
        {
            get { return background; }
            set
            {
                background = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<SolidColorBrush> colors;
        public ObservableCollection<SolidColorBrush> Colors
        {
            get { return colors; }
            set
            {
                colors = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Element> elemente;
        public ObservableCollection<Element> Elemente
        {
            get { return elemente; }
            set
            {
                elemente = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> kontexte;
        public ObservableCollection<string> Kontexte
        {
            get { return kontexte; }
            set
            {
                kontexte = value;
                OnPropertyChanged();
            }
        }

        private DataTable daten;
        public DataTable Daten
        {
            get { return daten; }
            set
            {
                daten = value;
                OnPropertyChanged();
            }
        }

        public PlayingCardDesign()
        {
            Elemente = new ObservableCollection<Element>();
            Kontexte = new ObservableCollection<string>();
            Colors = new ObservableCollection<SolidColorBrush>();
            Daten = new DataTable();
        }

        public static bool Serialize(PlayingCardDesign design, string filePath)
        {
            if (design == null)
            {
                return true;
            }
            try
            {
                string jsonString = JsonConvert.SerializeObject(design, Formatting.Indented);
                File.WriteAllText(filePath, jsonString);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MainWindow.Window.Dispatcher.Invoke(() =>
                {
                    MainWindow.SetStatusBar("Error: " + ex.Message);
                });

                return false;
            }
        }

        public static PlayingCardDesign Deserialize(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            try
            {
                var json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<PlayingCardDesign>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MainWindow.Window.Dispatcher.Invoke(() =>
                {
                    MainWindow.SetStatusBar("Error: " + ex.Message);
                });
                return null;
            }
        }

        public static PlayingCardDesign ReadFromFile(string filePath)
        {
            return Deserialize(filePath);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
