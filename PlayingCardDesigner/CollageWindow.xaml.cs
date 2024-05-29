using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PlayingCardDesigner
{
    /// <summary>
    /// Interaction logic for CollageWindow.xaml
    /// </summary>
    public partial class CollageWindow : Window
    {
        public static List<Canvas> Collages { get; set; }

        public CollageWindow()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        private void BTN_Export_Collage_Click(object sender, RoutedEventArgs e)
        {
            var currentSession = MainWindowViewModel.Main.Session;
            if (currentSession == null)
                return;

            CollageWindow.Collages.ForEach(collage =>
            {
                collage.UpdateLayout();
                if (collage.Children.Count > 0)
                {
                    var fileName = "Page " + (CollageWindow.Collages.IndexOf(collage) + 1);
                    var exportSuccess = Renderer.Export(collage, currentSession.SessionFileName, fileName, collage.Width * 3.675, collage.Height * 3.675);
                    if (exportSuccess)
                    {
                        MainWindow.SetStatusBar($"{currentSession.SessionFileName} - {fileName} exportiert");
                        Helper.MessageBoxTimeout((System.IntPtr)0, $"{currentSession.SessionFileName} - {fileName} exportiert", "Export", 0, 0, 100);
                    }
                    else
                    {
                        MainWindow.SetStatusBar($"{fileName} konnte nicht exportiert werden");
                    }
                }
            });
        }
    }
}
