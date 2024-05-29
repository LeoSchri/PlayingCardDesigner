using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace PlayingCardDesigner.Models
{
    public class Session
    {
        public string SessionFileName { get; set; }
        public string SessionFilePath { get; set; }
        public PlayingCardDesign Design { get; set; }

        public Session()
        {
        }

        public bool Save()
        {
            return PlayingCardDesign.Serialize(Design, SessionFilePath);
        }

        public bool Export()
        {
            try
            {
                var currentSession = MainWindowViewModel.Main.Session;
                if (currentSession == null)
                    return false;

                if(currentSession.Design.Daten != null && currentSession.Design.Daten.Rows.Count > 0)
                {
                    MainWindow.Window.Dispatcher.Invoke(() =>
                    {
                        MainWindow.SetStatusBar("Export gestartet");
                    });

                    var success = true;
                    foreach (DataRow row in currentSession.Design.Daten.Rows)
                    {
                        MainWindow.Window.Canvas_Export.Children.Clear();

                        var renderSuccess = Renderer.RenderRow(currentSession, row);

                        if (renderSuccess)
                        {
                            MainWindow.Window.Dispatcher.Invoke(() =>
                            {
                                MainWindow.SetStatusBar($"{row["Name"]} erstellt");
                                MainWindow.Window.Canvas_Export.UpdateLayout();
                                if (MainWindow.Window.Canvas_Export.Children.Count > 0)
                                {
                                    var exportSuccess = Renderer.Export(MainWindow.Window.Canvas_Export, currentSession.SessionFileName, row["Name"].ToString(), currentSession.Design.Width * 3.675, currentSession.Design.Height * 3.675);
                                    if (exportSuccess)
                                    {
                                        MainWindow.SetStatusBar($"{currentSession.SessionFileName + "-" + row["Name"].ToString()} exportiert");
                                        Helper.MessageBoxTimeout((System.IntPtr)0, $"{currentSession.SessionFileName + "-" + row["Name"].ToString()} exportiert", "Export", 0, 0, 100);
                                    }
                                    else
                                    {
                                        MainWindow.SetStatusBar($"{row["Name"]} konnte nicht exportiert werden");
                                        success = false;
                                    }
                                }
                            });
                        }
                        else
                        {
                            MainWindow.Window.Dispatcher.Invoke(() =>
                            {
                                MainWindow.SetStatusBar($"{row["Name"]} konnte nicht erstellt werden");
                            });
                            success = false;
                        }
                    }

                    if (success)
                    {
                        MainWindow.Window.Dispatcher.Invoke(() =>
                        {
                            MainWindow.SetStatusBar("Design exportiert");
                        });
                    }
                }

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

        public bool Collage()
        {
            try
            {
                var currentSession = MainWindowViewModel.Main.Session;
                if (currentSession == null)
                    return false;

                var answer = MessageBox.Show($"Möchten Sie einen Export durchführen?", "Export", MessageBoxButton.YesNo);
                if (answer == MessageBoxResult.Yes)
                    Export();
                return Renderer.Collage(currentSession.SessionFileName, currentSession.Design.Daten, currentSession.Design.Height, currentSession.Design.Width);
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

        public bool ChangeFileName(string oldName, string newName)
        {
            try
            {
                File.Delete(SessionFilePath);

                SessionFileName = newName;
                SessionFilePath = MainWindowViewModel.SessionsDirectory + @"\" + newName + ".json";
                Save();

                MainWindow.UpdateElements();
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

        public static bool ImportDataContext()
        {
            try
            {
                var currentSession = MainWindowViewModel.Main.Session;
                if (currentSession == null)
                    return false;

                var csvFile = Helper.GetFileFromFileDialog("Datenkontext", MainWindowViewModel.SessionsDirectory + @"\Data", "CSV Files(*.csv) | *.csv");

                currentSession.Design.Kontexte = Helper.GetHeaderFromCSV(csvFile);
                currentSession.Design.Daten = Helper.GetDataFromCSV(csvFile);
                currentSession.Save();

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

        public static bool New()
        {
            try
            {
                var fileName = Helper.GetNextFileName(MainWindowViewModel.SessionsDirectory + @"\NewPlayingCardDesign.json");
                var newSession = new Session
                {
                    Design = new PlayingCardDesign(),
                    SessionFileName = fileName,
                    SessionFilePath = MainWindowViewModel.SessionsDirectory + @"\"+fileName+".json"
                };

                MainWindowViewModel.Main.Session = newSession;

                newSession.Save();
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

        public static bool Open()
        {
            try
            {
                var designPath = Helper.GetFileFromFileDialog("Spielkarten -Design", MainWindowViewModel.SessionsDirectory, "JSON Files(*.json) | *.json");

                var fileInfo = new FileInfo(designPath);
                var fileName = fileInfo.Name.Replace(fileInfo.Extension, "");
                var openedSession = new Session
                {

                    Design = PlayingCardDesign.ReadFromFile(designPath),
                    SessionFileName = fileName,
                    SessionFilePath = designPath
                };

                MainWindowViewModel.Main.Session = openedSession;
                MainWindow.UpdateColors();
                MainWindow.UpdateElements();
                MainWindow.UpdateKontexte();
                Renderer.Render(openedSession.Design.Background, openedSession.Design.Elemente.ToList(), MainWindow.Window.Canvas_Design);
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

        public static bool Reload()
        {
            try
            {
                var currentSession = MainWindowViewModel.Main.Session;
                if (currentSession == null)
                    return false;

                currentSession.Design = PlayingCardDesign.ReadFromFile(currentSession.SessionFilePath);

                MainWindow.UpdateColors();
                MainWindow.UpdateElements();
                MainWindow.UpdateKontexte();
                Renderer.Render(currentSession.Design.Background, currentSession.Design.Elemente.ToList(), MainWindow.Window.Canvas_Design);
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
    }
}
