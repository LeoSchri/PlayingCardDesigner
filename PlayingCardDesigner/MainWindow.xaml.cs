using Newtonsoft.Json;
using PlayingCardDesigner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace PlayingCardDesigner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Window { get; set; }

        public static SolidColorBrush White { get; set; } = (SolidColorBrush) new BrushConverter().ConvertFrom("#FFFFFF");
        public static SolidColorBrush Black { get; set; } = (SolidColorBrush) new BrushConverter().ConvertFrom("#000000");

        public static Element CurrentElement { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
            Window = this;
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        public void ShowSession()
        {
            this.MenutItem_Save.IsEnabled = true;
            this.MenutItem_Reload.IsEnabled = true;
            this.MenutItem_Export.IsEnabled = true;
            this.DockPanel_Session.Visibility = Visibility.Visible;
        }

        public static void SetStatusBar(string message)
        {
            Window.LB_Statusbar.Content = message;
        }

        public static void UpdateElements()
        {
            var currentSession = ((MainWindowViewModel)(Window.DataContext)).Session;

            var list = new ObservableCollection<string>();
            currentSession.Design.Elemente.ToList().ForEach(element => list.Add(element.Name));
            MainWindowViewModel.Main.ElementList = list;
        }

        public static void UpdateKontexte()
        {
            var currentSession = ((MainWindowViewModel)(Window.DataContext)).Session;
            if(MainWindowViewModel.Main.DatenKontexte != null)
                MainWindowViewModel.Main.DatenKontexte.Clear();
            MainWindowViewModel.Main.DatenKontexte = currentSession.Design.Kontexte;
            MainWindowViewModel.Main.DatenKontexte.Add("");
        }

        public static void UpdateColors()
        {
            var currentSession = ((MainWindowViewModel)(Window.DataContext)).Session;

            MainWindowViewModel.Main.ColorPalette = new List<string>();
            currentSession.Design.Colors.ToList().ForEach(color=> MainWindowViewModel.Main.ColorPalette.Add(color.ToString()));
        }

        private void MenuItem_New_Click(object sender, RoutedEventArgs e)
        {
            var created = Session.New();
            if (created)
                ShowSession();
        }

        private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            var opened =Session.Open();
            if (opened)
                ShowSession();
        }

        private void MenuItem_Save_Click(object sender, RoutedEventArgs e)
        {
            var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
            currentSession.Save();

            MainWindow.Window.Dispatcher.Invoke(() =>
            {
                SetStatusBar("Änderungen gespeichert");
            });
        }

        private void MenuItem_Reload_Click(object sender, RoutedEventArgs e)
        {
            Session.Reload();
            MainWindow.Window.Dispatcher.Invoke(() =>
            {
                SetStatusBar("Design aktualisiert");
            });
        }

        private void MenuItem_Export_Click(object sender, RoutedEventArgs e)
        {
            var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
            currentSession.Export();
        }

        private void MenuItem_Collage_Click(object sender, RoutedEventArgs e)
        {
            var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
            currentSession.Collage();
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
            if(currentSession != null)
                currentSession.Save();

            Application.Current.Shutdown();
        }

        private void BTN_Edit_Name_Click(object sender, RoutedEventArgs e)
        {
            LB_Design_Name.Visibility = Visibility.Collapsed;
            BTN_Edit_Name.Visibility = Visibility.Collapsed;

            TB_Design_Name.Visibility = Visibility.Visible;
            BTN_Save_Name.Visibility = Visibility.Visible;
        }

        private void BTN_Save_Name_Click(object sender, RoutedEventArgs e)
        {
            var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
            currentSession.ChangeFileName(LB_Design_Name.Content.ToString(), TB_Design_Name.Text);

            TB_Design_Name.Visibility = Visibility.Collapsed;
            BTN_Save_Name.Visibility = Visibility.Collapsed;

            LB_Design_Name.Visibility = Visibility.Visible;
            BTN_Edit_Name.Visibility = Visibility.Visible;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S)
            {
                var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
                currentSession.Save();

                MainWindow.Window.Dispatcher.Invoke(() =>
                {
                    SetStatusBar("Änderungen gespeichert");
                });
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.E)
            {
                var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
                currentSession.Export();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.A)
            {
                Session.Reload();
                UpdateColors();
                MainWindow.Window.Dispatcher.Invoke(() =>
                {
                    SetStatusBar("Design aktualisiert");
                });
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.C)
            {
                var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
                currentSession.Collage();
            }
        }

        private void BTN_Add_Rectangle_Click(object sender, RoutedEventArgs e)
        {
            CB_Rectangle_BorderColor.SelectedValue = null;
            TB_Rectangle_BorderThickness.Text = "0";
            CB_Rectangle_DataContext.SelectedValue = null;
            TB_Rectangle_DataContext.Text = "";
            CB_Rectangle_Fill.SelectedValue = null;
            TB_Rectangle_Height.Text = "0";
            TB_Rectangle_Name.Text = "Rectangle";
            TB_Rectangle_Width.Text = "0";
            TB_Rectangle_X.Text = "0";
            TB_Rectangle_Y.Text = "0";
            TB_Rectangle_CornerRadius.Text = "0";
            TB_Rectangle_Fontsize.Text = "12";

            Grid_AddRectangle.Visibility = Visibility.Visible;
        }

        private void BTN_Add_Circle_Click(object sender, RoutedEventArgs e)
        {
            CB_Circle_BorderColor.SelectedValue = null;
            TB_Circle_BorderThickness.Text = "0";
            CB_Circle_DataContext.SelectedValue = null;
            TB_Circle_DataContext.Text = "";
            CB_Circle_Fill.SelectedValue = null;
            TB_Circle_Height.Text = "0";
            TB_Circle_Name.Text = "Circle";
            TB_Circle_Width.Text = "0";
            TB_Circle_X.Text = "0";
            TB_Circle_Y.Text = "0";
            TB_Circle_Fontsize.Text = "12";

            Grid_AddCircle.Visibility = Visibility.Visible;
        }

        private void BTN_Add_Icon_Click(object sender, RoutedEventArgs e)
        {
            TB_Icon_Height.Text = "0";
            TB_Icon_Name.Text = "Icon";
            TB_Icon_Width.Text = "0";
            TB_Icon_X.Text = "0";
            TB_Icon_Y.Text = "0";
            TB_Icon_FileName.Text = "";
            CB_Icon_Datacontext.SelectedValue = null;

            Grid_AddIcon.Visibility = Visibility.Visible;
        }

        private void BTN_Size_Edit_Click(object sender, RoutedEventArgs e)
        {
            LB_Size_Width.Visibility = Visibility.Collapsed;
            LB_Size_Height.Visibility = Visibility.Collapsed;
            LB_Size_CornerRadius.Visibility = Visibility.Collapsed;
            LB_Size_Fill.Visibility = Visibility.Collapsed;
            BTN_Edit_Size.Visibility = Visibility.Collapsed;

            TB_Size_Width.Visibility = Visibility.Visible;
            TB_Size_Height.Visibility = Visibility.Visible;
            TB_Size_CornerRadius.Visibility = Visibility.Visible;
            CB_Size_Fill.Visibility = Visibility.Visible;
            BTN_Save_Size.Visibility = Visibility.Visible;
        }

        private void BTN_Size_Save_Click(object sender, RoutedEventArgs e)
        {
            var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;

            if (currentSession.Design == null)
                currentSession.Design = new PlayingCardDesign();

            var bgColor = CB_Size_Fill.SelectedValue==null?null:(SolidColorBrush)new BrushConverter().ConvertFrom(CB_Size_Fill.SelectedValue);
            currentSession.Design.Background = new BorderedRectangle("Background", Convert.ToDouble(TB_Size_Width.Text), Convert.ToDouble(TB_Size_Height.Text), 0, 0, Convert.ToInt32(TB_Size_CornerRadius.Text), 0, null, bgColor, 0, null);
            currentSession.Design.Height = Convert.ToDouble(TB_Size_Height.Text) * 3.675;
            currentSession.Design.Width = Convert.ToDouble(TB_Size_Width.Text) * 3.675;
            currentSession.Save();

            Renderer.Render(currentSession.Design.Background, currentSession.Design.Elemente.ToList(), MainWindow.Window.Canvas_Design);

            TB_Size_Width.Visibility = Visibility.Collapsed;
            TB_Size_Height.Visibility = Visibility.Collapsed;
            TB_Size_CornerRadius.Visibility = Visibility.Collapsed;
            CB_Size_Fill.Visibility = Visibility.Collapsed;
            BTN_Save_Size.Visibility = Visibility.Collapsed;

            LB_Size_Width.Visibility = Visibility.Visible;
            LB_Size_Height.Visibility = Visibility.Visible;
            LB_Size_CornerRadius.Visibility = Visibility.Visible;
            LB_Size_Fill.Visibility = Visibility.Visible;
            BTN_Edit_Size.Visibility = Visibility.Visible;
        }

        private void BTN_Edit_Element_Click(object sender, RoutedEventArgs e)
        {
            var element = (Element)((Button)sender).ToolTip;
            CurrentElement = element;

            if (element == null)
                return;

            if(element.Type == "Circle")
            {
                CB_Circle_BorderColor.SelectedValue = element.BorderColor == null ? "" : element.BorderColor.ToString();
                TB_Circle_BorderThickness.Text = element.BorderThickness.ToString();
                CB_Circle_DataContext.SelectedValue = element.DataContext;
                TB_Circle_DataContext.Text = element.DataContext;
                CB_Circle_Fill.SelectedValue = element.Fill == null ? "" : element.Fill.ToString();
                TB_Circle_Height.Text = element.Height.ToString();
                TB_Circle_Name.Text = element.Name;
                TB_Circle_Width.Text = element.Width.ToString();
                TB_Circle_X.Text = element.Location_X.ToString();
                TB_Circle_Y.Text = element.Location_Y.ToString();
                TB_Circle_Fontsize.Text = element.Fontsize.ToString();

                BTN_Create_Circle.Visibility = Visibility.Collapsed;
                RB_Circle_Top.Visibility = Visibility.Collapsed;
                RB_Circle_Bottom.Visibility = Visibility.Collapsed;
                BTN_Save_Circle.Visibility = Visibility.Visible;
                Grid_AddCircle.Visibility = Visibility.Visible;
            }
            else if(element.Type == "Rectangle")
            {
                CB_Rectangle_BorderColor.SelectedValue = element.BorderColor==null?"":element.BorderColor.ToString();
                TB_Rectangle_BorderThickness.Text = element.BorderThickness.ToString();
                CB_Rectangle_DataContext.SelectedValue = element.DataContext;
                TB_Rectangle_DataContext.Text = element.DataContext;
                CB_Rectangle_Fill.SelectedValue = element.Fill == null ? "" : element.Fill.ToString();
                TB_Rectangle_Height.Text = element.Height.ToString();
                TB_Rectangle_Name.Text = element.Name;
                TB_Rectangle_Width.Text = element.Width.ToString();
                TB_Rectangle_X.Text = element.Location_X.ToString();
                TB_Rectangle_Y.Text = element.Location_Y.ToString();
                TB_Rectangle_CornerRadius.Text = element.CornerRadius.ToString();
                TB_Rectangle_Fontsize.Text = element.Fontsize.ToString();

                BTN_Create_Rectangle.Visibility = Visibility.Collapsed;
                RB_Rectangle_Top.Visibility = Visibility.Collapsed;
                RB_Rectangle_Bottom.Visibility = Visibility.Collapsed;
                BTN_Save_Rectangle.Visibility = Visibility.Visible;
                Grid_AddRectangle.Visibility = Visibility.Visible;
            }
            else if (element.Type == "Icon")
            {
                TB_Icon_Height.Text = element.Height.ToString();
                TB_Icon_Name.Text = element.Name;
                TB_Icon_Width.Text = element.Width.ToString();
                TB_Icon_X.Text = element.Location_X.ToString();
                TB_Icon_Y.Text = element.Location_Y.ToString();
                TB_Icon_FileName.Text = element.ImageFileName;
                CB_Icon_Datacontext.SelectedValue = element.DataContext;

                BTN_Create_Icon.Visibility = Visibility.Collapsed;
                RB_Icon_Top.Visibility = Visibility.Collapsed;
                RB_Icon_Bottom.Visibility = Visibility.Collapsed;
                BTN_Save_Icon.Visibility = Visibility.Visible;
                Grid_AddIcon.Visibility = Visibility.Visible;
            }
        }

        private void BTN_Delete_Element_Click(object sender, RoutedEventArgs e)
        {
            var element = (Element)((Button)sender).ToolTip;

            if (element == null)
                return;

            var answer = MessageBox.Show($"Möchten Sie das Element {element.Name} wirklich löschen?", "Entfernen", MessageBoxButton.YesNo);
            if(answer == MessageBoxResult.Yes)
            {
                var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
                var targetElement = currentSession.Design.Elemente.ToList().Find(el => el.Name == element.Name);
                if(targetElement !=null)
                {
                    currentSession.Design.Elemente.Remove(targetElement);

                    currentSession.Save();
                    Renderer.Render(currentSession.Design.Background, currentSession.Design.Elemente.ToList(), MainWindow.Window.Canvas_Design);
                }
            }
        }

        private void BTN_MoveUp_Element_Click(object sender, RoutedEventArgs e)
        {
            var element = (Element)((Button)sender).ToolTip;

            if (element == null)
                return;

            var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
            var targetElement = currentSession.Design.Elemente.ToList().Find(el => el.Name == element.Name);
            if(targetElement !=null)
            {
                var newIndex = currentSession.Design.Elemente.IndexOf(targetElement)-1;
                if(newIndex>-1)
                {
                    currentSession.Design.Elemente.Remove(targetElement);
                    currentSession.Design.Elemente.Insert(newIndex, targetElement);
                }

                currentSession.Save();
                Renderer.Render(currentSession.Design.Background, currentSession.Design.Elemente.ToList(), MainWindow.Window.Canvas_Design);
            }
        }

        private void BTN_MoveDown_Element_Click(object sender, RoutedEventArgs e)
        {
            var element = (Element)((Button)sender).ToolTip;

            if (element == null)
                return;

            var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
            var targetElement = currentSession.Design.Elemente.ToList().Find(el => el.Name == element.Name);
            if (targetElement != null)
            {
                var newIndex = currentSession.Design.Elemente.IndexOf(targetElement) +1;
                if (newIndex < currentSession.Design.Elemente.Count)
                {
                    currentSession.Design.Elemente.Remove(targetElement);
                    currentSession.Design.Elemente.Insert(newIndex, targetElement);
                }

                currentSession.Save();
                Renderer.Render(currentSession.Design.Background, currentSession.Design.Elemente.ToList(), MainWindow.Window.Canvas_Design);
            }
        }

        private void BTN_MoveTop_Element_Click(object sender, RoutedEventArgs e)
        {
            var element = (Element)((Button)sender).ToolTip;

            if (element == null)
                return;

            var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
            var targetElement = currentSession.Design.Elemente.ToList().Find(el => el.Name == element.Name);
            if (targetElement != null)
            {
                currentSession.Design.Elemente.Remove(targetElement);
                currentSession.Design.Elemente.Insert(0, targetElement);

                currentSession.Save();
                Renderer.Render(currentSession.Design.Background, currentSession.Design.Elemente.ToList(), MainWindow.Window.Canvas_Design);
            }
        }

        private void BTN_MoveBottom_Element_Click(object sender, RoutedEventArgs e)
        {
            var element = (Element)((Button)sender).ToolTip;

            if (element == null)
                return;

            var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
            var targetElement = currentSession.Design.Elemente.ToList().Find(el => el.Name == element.Name);
            if (targetElement != null)
            {
                currentSession.Design.Elemente.Remove(targetElement);
                currentSession.Design.Elemente.Add(targetElement);

                currentSession.Save();
                Renderer.Render(currentSession.Design.Background, currentSession.Design.Elemente.ToList(), MainWindow.Window.Canvas_Design);
            }
        }

        private void BTN_Element_AlignTo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var elementName1 = CB_Element1.SelectedValue.ToString();
                var elementName2 = CB_Element2.SelectedValue.ToString();

                if (!string.IsNullOrEmpty(elementName1)&& !string.IsNullOrEmpty(elementName2))
                {
                    var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
                    var targetElement1 = currentSession.Design.Elemente.ToList().Find(el => el.Name == elementName1);
                    var targetElement2 = currentSession.Design.Elemente.ToList().Find(el => el.Name == elementName2);

                    if (targetElement1 != null && targetElement2 != null)
                    {
                        var verticalDirection = "NONE";

                        if ((bool)RB_Align_Top.IsChecked)
                            verticalDirection = "TOP";
                        if ((bool)RB_Align_Vertical.IsChecked)
                            verticalDirection = "CENTER";
                        if ((bool)RB_Align_Bottom.IsChecked)
                            verticalDirection = "BOTTOM";

                        var horizontalDirection = "NONE";

                        if ((bool)RB_Align_Left.IsChecked)
                            horizontalDirection = "LEFT";
                        if ((bool)RB_Align_Horizontal.IsChecked)
                            horizontalDirection = "CENTER";
                        if ((bool)RB_Align_Right.IsChecked)
                            horizontalDirection = "RIGHT";

                        if (verticalDirection == "NONE" && horizontalDirection == "NONE")
                            return;

                        targetElement1.AlignTo(targetElement2, verticalDirection, horizontalDirection);
                        currentSession.Design.Elemente.Insert(currentSession.Design.Elemente.IndexOf(targetElement1), targetElement1);
                        currentSession.Design.Elemente.Remove(targetElement1);

                        currentSession.Save();
                        Renderer.Render(currentSession.Design.Background, currentSession.Design.Elemente.ToList(), MainWindow.Window.Canvas_Design);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MainWindow.Window.Dispatcher.Invoke(() =>
                {
                    SetStatusBar("Error: " + ex.Message);
                });
            }

            CB_Element1.SelectedValue = null;
            CB_Element2.SelectedValue = null;
        }

        private void BTN_Delete_Color_Click(object sender, RoutedEventArgs e)
        {
            var color = (SolidColorBrush)((Button)sender).ToolTip;

            if (color == null)
                return;

            var answer = MessageBox.Show($"Möchten Sie die Farbe {color} wirklich löschen?", "Entfernen", MessageBoxButton.YesNo);
            if (answer == MessageBoxResult.Yes)
            {
                var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
                var targetColor = currentSession.Design.Colors.ToList().Find(c => c == color);
                if (targetColor != null)
                {
                    currentSession.Design.Colors.Remove(targetColor);

                    currentSession.Save();
                }
            }
        }

        private void BTN_Create_Circle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
                var name = Helper.GetNextElementName(currentSession, TB_Circle_Name.Text);

                var dataContext = CB_Circle_DataContext.SelectedValue == null ? null : CB_Circle_DataContext.SelectedValue.ToString();
                if (string.IsNullOrEmpty(dataContext))
                    dataContext = TB_Circle_DataContext.Text;

                BorderedCircle circle = new BorderedCircle
                {
                    Width = Convert.ToDouble(TB_Circle_Width.Text),
                    Height = Convert.ToDouble(TB_Circle_Height.Text),
                    DataContext = dataContext,
                    Location_X = Convert.ToDouble(TB_Circle_X.Text),
                    Location_Y = Convert.ToDouble(TB_Circle_Y.Text),
                    BorderThickness = Convert.ToDouble(TB_Circle_BorderThickness.Text),
                    BorderColor = CB_Circle_BorderColor.SelectedValue == null ? null : (SolidColorBrush)new BrushConverter().ConvertFrom(CB_Circle_BorderColor.SelectedValue),
                    Fill = CB_Circle_Fill.SelectedValue == null ? null : (SolidColorBrush)new BrushConverter().ConvertFrom(CB_Circle_Fill.SelectedValue),
                    Name = TB_Circle_Name.Text,
                    Fontsize = Convert.ToInt32(TB_Circle_Fontsize.Text)
                };

                if ((bool)RB_Circle_Bottom.IsChecked)
                    currentSession.Design.Elemente.Add(circle);
                else currentSession.Design.Elemente.Insert(0, circle);

                currentSession.Save();

                Renderer.Render(currentSession.Design.Background, currentSession.Design.Elemente.ToList(), MainWindow.Window.Canvas_Design);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MainWindow.Window.Dispatcher.Invoke(() =>
                {
                    SetStatusBar("Error: " + ex.Message);
                });
            }
            Grid_AddCircle.Visibility = Visibility.Collapsed;
        }

        private void BTN_Create_Rectangle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
                var name = Helper.GetNextElementName(currentSession, TB_Rectangle_Name.Text);

                var dataContext = CB_Rectangle_DataContext.SelectedValue == null ? null : CB_Rectangle_DataContext.SelectedValue.ToString();
                if (string.IsNullOrEmpty(dataContext))
                    dataContext = TB_Rectangle_DataContext.Text;

                BorderedRectangle rectangle = new BorderedRectangle
                {
                    Width = Convert.ToDouble(TB_Rectangle_Width.Text),
                    Height = Convert.ToDouble(TB_Rectangle_Height.Text),
                    DataContext = dataContext,
                    Location_X = Convert.ToDouble(TB_Rectangle_X.Text),
                    Location_Y = Convert.ToDouble(TB_Rectangle_Y.Text),
                    BorderThickness = Convert.ToDouble(TB_Rectangle_BorderThickness.Text),
                    BorderColor = CB_Rectangle_BorderColor.SelectedValue == null ? null : (SolidColorBrush)new BrushConverter().ConvertFrom(CB_Rectangle_BorderColor.SelectedValue),
                    Fill = CB_Rectangle_Fill.SelectedValue == null ? null : (SolidColorBrush)new BrushConverter().ConvertFrom(CB_Rectangle_Fill.SelectedValue),
                    Name = TB_Rectangle_Name.Text,
                    CornerRadius = Convert.ToInt32(TB_Rectangle_CornerRadius.Text),
                    Fontsize = Convert.ToInt32(TB_Rectangle_Fontsize.Text)
                };

                if ((bool)RB_Rectangle_Bottom.IsChecked)
                    currentSession.Design.Elemente.Add(rectangle);
                else currentSession.Design.Elemente.Insert(0, rectangle);

                currentSession.Save();

                Renderer.Render(currentSession.Design.Background, currentSession.Design.Elemente.ToList(), MainWindow.Window.Canvas_Design);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MainWindow.Window.Dispatcher.Invoke(() =>
                {
                    SetStatusBar("Error: " + ex.Message);
                });
            }

            Grid_AddRectangle.Visibility = Visibility.Collapsed;
        }

        private void BTN_Create_Icon_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
                var name = Helper.GetNextElementName(currentSession, TB_Icon_Name.Text);

                Icon icon = new Icon
                {
                    Name = TB_Icon_Name.Text,
                    Location_X = Convert.ToDouble(TB_Icon_X.Text),
                    Location_Y = Convert.ToDouble(TB_Icon_Y.Text),
                    Height = Convert.ToDouble(TB_Icon_Height.Text),
                    Width = Convert.ToDouble(TB_Icon_Width.Text),
                    ImageFileName = string.IsNullOrEmpty(TB_Icon_FileName.Text) ? "" : TB_Icon_FileName.Text,
                    DataContext = CB_Icon_Datacontext.SelectedValue==null?"":CB_Icon_Datacontext.SelectedValue.ToString()
                };

                if ((bool)RB_Icon_Bottom.IsChecked)
                    currentSession.Design.Elemente.Add(icon);
                else currentSession.Design.Elemente.Insert(0, icon);

                currentSession.Save();

                Renderer.Render(currentSession.Design.Background, currentSession.Design.Elemente.ToList(), MainWindow.Window.Canvas_Design);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MainWindow.Window.Dispatcher.Invoke(() =>
                {
                    SetStatusBar("Error: " + ex.Message);
                });
            }

            Grid_AddIcon.Visibility = Visibility.Collapsed;
        }

        private void BTN_Save_Circle_Click(object sender, RoutedEventArgs e)
        {
            var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
            var targetElement = currentSession.Design.Elemente.ToList().Find(element => element.Name == CurrentElement.Name);

            if (targetElement == null)
                return;

            var dataContext = CB_Circle_DataContext.SelectedValue == null ? null : CB_Circle_DataContext.SelectedValue.ToString();
            if (string.IsNullOrEmpty(dataContext))
                dataContext = TB_Circle_DataContext.Text;

            BorderedCircle circle = new BorderedCircle
            {
                Width = Convert.ToDouble(TB_Circle_Width.Text),
                Height = Convert.ToDouble(TB_Circle_Height.Text),
                DataContext = dataContext,
                Location_X = Convert.ToDouble(TB_Circle_X.Text),
                Location_Y = Convert.ToDouble(TB_Circle_Y.Text),
                BorderThickness = Convert.ToDouble(TB_Circle_BorderThickness.Text),
                BorderColor = string.IsNullOrEmpty(CB_Circle_BorderColor.SelectedValue.ToString()) ? null : (SolidColorBrush)new BrushConverter().ConvertFrom(CB_Circle_BorderColor.SelectedValue),
                Fill = string.IsNullOrEmpty(CB_Circle_Fill.SelectedValue.ToString()) ? null : (SolidColorBrush)new BrushConverter().ConvertFrom(CB_Circle_Fill.SelectedValue),
                Name = TB_Circle_Name.Text,
                Fontsize = Convert.ToInt32(TB_Circle_Fontsize.Text)
            };

            currentSession.Design.Elemente.Insert(currentSession.Design.Elemente.IndexOf(targetElement), circle);
            currentSession.Design.Elemente.Remove(targetElement);

            currentSession.Save();

            Renderer.Render(currentSession.Design.Background, currentSession.Design.Elemente.ToList(), MainWindow.Window.Canvas_Design);

            Grid_AddCircle.Visibility = Visibility.Collapsed;
            BTN_Save_Circle.Visibility = Visibility.Collapsed;
            BTN_Create_Circle.Visibility = Visibility.Visible;
        }

        private void BTN_Save_Rectangle_Click(object sender, RoutedEventArgs e)
        {
            var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
            var targetElement = currentSession.Design.Elemente.ToList().Find(element => element.Name == CurrentElement.Name);

            if (targetElement == null)
                return;

            var dataContext = CB_Rectangle_DataContext.SelectedValue == null ? null : CB_Rectangle_DataContext.SelectedValue.ToString();
            if (string.IsNullOrEmpty(dataContext))
                dataContext = TB_Rectangle_DataContext.Text;

            BorderedRectangle rectangle = new BorderedRectangle
            {
                Width = Convert.ToDouble(TB_Rectangle_Width.Text),
                Height = Convert.ToDouble(TB_Rectangle_Height.Text),
                DataContext = dataContext,
                Location_X = Convert.ToDouble(TB_Rectangle_X.Text),
                Location_Y = Convert.ToDouble(TB_Rectangle_Y.Text),
                BorderThickness = Convert.ToDouble(TB_Rectangle_BorderThickness.Text),
                BorderColor = string.IsNullOrEmpty(CB_Rectangle_BorderColor.SelectedValue.ToString()) ? null : (SolidColorBrush)new BrushConverter().ConvertFrom(CB_Rectangle_BorderColor.SelectedValue),
                Fill = string.IsNullOrEmpty(CB_Rectangle_Fill.SelectedValue.ToString()) ? null : (SolidColorBrush)new BrushConverter().ConvertFrom(CB_Rectangle_Fill.SelectedValue),
                Name = TB_Rectangle_Name.Text,
                CornerRadius = Convert.ToInt32(TB_Rectangle_CornerRadius.Text),
                Fontsize = Convert.ToInt32(TB_Rectangle_Fontsize.Text)
            };

            currentSession.Design.Elemente.Insert(currentSession.Design.Elemente.IndexOf(targetElement), rectangle);
            currentSession.Design.Elemente.Remove(targetElement);

            currentSession.Save();

            Renderer.Render(currentSession.Design.Background, currentSession.Design.Elemente.ToList(), MainWindow.Window.Canvas_Design);

            Grid_AddRectangle.Visibility = Visibility.Collapsed;
            BTN_Save_Rectangle.Visibility = Visibility.Collapsed;
            BTN_Create_Rectangle.Visibility = Visibility.Visible;
        }

        private void BTN_Save_Icon_Click(object sender, RoutedEventArgs e)
        {
            var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
            var targetElement = currentSession.Design.Elemente.ToList().Find(element => element.Name == CurrentElement.Name);

            if (targetElement == null)
                return;

            Icon icon = new Icon
            {
                Name = TB_Icon_Name.Text,
                Location_X = Convert.ToDouble(TB_Icon_X.Text),
                Location_Y = Convert.ToDouble(TB_Icon_Y.Text),
                ImageFileName = string.IsNullOrEmpty(TB_Icon_FileName.Text) ? "" : TB_Icon_FileName.Text,
                DataContext = CB_Icon_Datacontext.SelectedValue==null?"":CB_Icon_Datacontext.SelectedValue.ToString()
            };

            if (!string.IsNullOrEmpty(TB_Icon_Width.Text))
                icon.Width = Convert.ToDouble(TB_Icon_Width.Text);

            if (!string.IsNullOrEmpty(TB_Icon_Height.Text))
                icon.Height = Convert.ToDouble(TB_Icon_Height.Text);

            currentSession.Design.Elemente.Insert(currentSession.Design.Elemente.IndexOf(targetElement), icon);
            currentSession.Design.Elemente.Remove(targetElement);

            currentSession.Save();
            Renderer.Render(currentSession.Design.Background, currentSession.Design.Elemente.ToList(), MainWindow.Window.Canvas_Design);

            Grid_AddIcon.Visibility = Visibility.Collapsed;
            BTN_Save_Icon.Visibility = Visibility.Collapsed;
            BTN_Create_Icon.Visibility = Visibility.Visible;
        }

        private void BTN_Import_DataContext_Click(object sender, RoutedEventArgs e)
        {
            Session.ImportDataContext();
            UpdateKontexte();
        }

        private void BTN_Render_Example_Click(object sender, RoutedEventArgs e)
        {
            var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
            if (currentSession.Design.Daten != null && currentSession.Design.Daten.Rows.Count > 0)
            {
                Renderer.RenderRow(currentSession, currentSession.Design.Daten.Rows[0]);
            }
        }

        private void BTN_Cancel_Save_Circle_Click(object sender, RoutedEventArgs e)
        {
            Grid_AddCircle.Visibility = Visibility.Collapsed;
            BTN_Save_Circle.Visibility = Visibility.Collapsed;
            BTN_Create_Circle.Visibility = Visibility.Visible;
        }

        private void BTN_Cancel_Save_Rectangle_Click(object sender, RoutedEventArgs e)
        {
            Grid_AddRectangle.Visibility = Visibility.Collapsed;
            BTN_Save_Rectangle.Visibility = Visibility.Collapsed;
            BTN_Create_Rectangle.Visibility = Visibility.Visible;
        }

        private void BTN_Cancel_Save_Icon_Click(object sender, RoutedEventArgs e)
        {
            Grid_AddIcon.Visibility = Visibility.Collapsed;
            BTN_Save_Icon.Visibility = Visibility.Collapsed;
            BTN_Create_Icon.Visibility = Visibility.Visible;
        }

        private void BTN_Move_Elements_Click(object sender, RoutedEventArgs e)
        {
            var moveAmount = Convert.ToDouble(TB_Move_Amount.Text);
            if (moveAmount == 0)
                return;

            var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
            var selectedElements = currentSession.Design.Elemente.Where(element => element.IsSelected).ToList();
            if (selectedElements == null)
                return;

            selectedElements.ForEach(element =>
            {
                if((bool)RB_Move_Left.IsChecked)
                {
                    element.Location_X -= moveAmount;
                }
                else if ((bool)RB_Move_Up.IsChecked)
                {
                    element.Location_Y -= moveAmount;
                }
                else if ((bool)RB_Move_Right.IsChecked)
                {
                    element.Location_X += moveAmount;
                }
                else if ((bool)RB_Move_Down.IsChecked)
                {
                    element.Location_Y += moveAmount;
                }
            });

            currentSession.Save();
            Renderer.Render(currentSession.Design.Background, currentSession.Design.Elemente.ToList(), MainWindow.Window.Canvas_Design);
        }

        private void BTN_Add_Color_Click(object sender, RoutedEventArgs e)
        {
            var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
            var color = string.IsNullOrEmpty(TB_Add_Color.Text) ? null : (SolidColorBrush)new BrushConverter().ConvertFrom(TB_Add_Color.Text);
            currentSession.Design.Colors.Add(color);
            currentSession.Save();
        }

        private void BTN_Export_Click(object sender, RoutedEventArgs e)
        {
            var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
            currentSession.Export();
        }

        private void BTN_Collage_Click(object sender, RoutedEventArgs e)
        {
            var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
            currentSession.Collage();
        }

        private void BTN_UnselectAll_Elemente_Click(object sender, RoutedEventArgs e)
        {
            var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
            currentSession.Design.Elemente.ToList().ForEach(element =>element.IsSelected = false);
        }

        private void BTN_Add_Table_Click(object sender, RoutedEventArgs e)
        {
            TB_Table_Height.Text = "0";
            TB_Table_Width.Text = "0";
            TB_Table_X.Text = "0";
            TB_Table_Y.Text = "0";

            Grid_AddTable.Visibility = Visibility.Visible;
        }

        private void BTN_Create_Table_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;

                var table = new Models.Table
                {
                    Name = "Table",
                    Location_X = Convert.ToDouble(TB_Table_X.Text),
                    Location_Y = Convert.ToDouble(TB_Table_Y.Text),
                    Height = Convert.ToDouble(TB_Table_Height.Text),
                    Width = Convert.ToDouble(TB_Table_Width.Text)
                };

                if ((bool)RB_Table_Bottom.IsChecked)
                    currentSession.Design.Elemente.Add(table);
                else currentSession.Design.Elemente.Insert(0, table);

                currentSession.Save();

                Renderer.Render(currentSession.Design.Background, currentSession.Design.Elemente.ToList(), MainWindow.Window.Canvas_Design);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MainWindow.Window.Dispatcher.Invoke(() =>
                {
                    SetStatusBar("Error: " + ex.Message);
                });
            }

            Grid_AddTable.Visibility = Visibility.Collapsed;
        }

        private void BTN_Save_Table_Click(object sender, RoutedEventArgs e)
        {
            var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
            var targetElement = currentSession.Design.Elemente.ToList().Find(element => element.Name == CurrentElement.Name);

            if (targetElement == null)
                return;

            var table = new Models.Table
            {
                Name = "Table",
                Location_X = Convert.ToDouble(TB_Table_X.Text),
                Location_Y = Convert.ToDouble(TB_Table_Y.Text),
                Height = Convert.ToDouble(TB_Table_Height.Text),
                Width = Convert.ToDouble(TB_Table_Width.Text),
            };

            currentSession.Design.Elemente.Insert(currentSession.Design.Elemente.IndexOf(targetElement), table);
            currentSession.Design.Elemente.Remove(targetElement);

            currentSession.Save();
            Renderer.Render(currentSession.Design.Background, currentSession.Design.Elemente.ToList(), MainWindow.Window.Canvas_Design);

            Grid_AddTable.Visibility = Visibility.Collapsed;
            BTN_Save_Table.Visibility = Visibility.Collapsed;
            BTN_Create_Table.Visibility = Visibility.Visible;
        }

        private void BTN_Cancel_Save_Table_Click(object sender, RoutedEventArgs e)
        {
            Grid_AddTable.Visibility = Visibility.Collapsed;
            BTN_Save_Table.Visibility = Visibility.Collapsed;
            BTN_Create_Table.Visibility = Visibility.Visible;
        }
    }
}
