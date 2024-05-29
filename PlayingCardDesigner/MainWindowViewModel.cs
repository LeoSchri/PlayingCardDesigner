using PlayingCardDesigner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PlayingCardDesigner
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public static MainWindowViewModel Main { get; set; }
        public static string SessionsDirectory { get; set; }
        public static string ImageDirectory { get; set; }

        private List<string> colorPalette;
        public List<string> ColorPalette
        {
            get { return colorPalette; }
            set
            {
                colorPalette = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> elementList;
        public ObservableCollection<string> ElementList
        {
            get { return elementList; }
            set
            {
                elementList = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> datenKontexte;
        public ObservableCollection<string> DatenKontexte
        {
            get { return datenKontexte; }
            set
            {
                datenKontexte = value;
                OnPropertyChanged();
            }
        }

        private Session session;
        public Session Session
        {
            get { return session; }
            set
            {
                session = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            Main = this;
            SessionsDirectory = Helper.FindDropboxFolder() + @"\PenAndPaper\Dice Masters\Spielkarten-Designs";
            ImageDirectory = SessionsDirectory + @"\Images";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
