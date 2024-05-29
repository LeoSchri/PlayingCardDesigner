using Microsoft.Win32;
using Newtonsoft.Json;
using PlayingCardDesigner.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Windows.Controls.Image;

namespace PlayingCardDesigner
{
    public static class Renderer
    {
        public static Rectangle CreateSimpleRectangle(SolidColorBrush fill, double width, double height, int cornerRadius)
        {
            return new Rectangle
            {
                Fill = fill,
                Width = width*3.675,
                Height = height*3.675,
                RadiusX = cornerRadius,
                RadiusY = cornerRadius
            };
        }

        public static Rectangle CreateRectangle(SolidColorBrush fill, double width, double height, int cornerRadius, double borderThickness, SolidColorBrush borderColor, bool hasGradient)
        {
            if (hasGradient)
            {
                var linearGradientBrush = new LinearGradientBrush(fill.Color, Brushes.White.Color, 90);
                return new Rectangle
                {
                    Fill = linearGradientBrush,
                    Width = width * 3.675,
                    Height = height * 3.675,
                    RadiusX = cornerRadius,
                    RadiusY = cornerRadius,
                    StrokeThickness = borderThickness,
                    Stroke = borderColor
                };
            }
            else return new Rectangle
            {
                Fill = fill,
                Width = width * 3.675,
                Height = height * 3.675,
                RadiusX = cornerRadius,
                RadiusY = cornerRadius,
                StrokeThickness = borderThickness,
                Stroke= borderColor
            };
        }

        public static Ellipse CreateSimpleCircle(SolidColorBrush fill, double width, double height)
        {
            return new Ellipse
            {
                Fill = fill,
                Width = width * 3.675,
                Height = height * 3.675
            };
        }

        public static Ellipse CreateCircle(SolidColorBrush fill, double width, double height, double borderThickness, SolidColorBrush borderColor)
        {
            return new Ellipse
            {
                Fill = fill,
                Width = width * 3.675,
                Height = height * 3.675,
                StrokeThickness = borderThickness,
                Stroke = borderColor
            };
        }

        public static Image CreateImage(string fileName, double height, double width)
        {
            var filePath = MainWindowViewModel.ImageDirectory + @"\" + fileName;

            if (!File.Exists(filePath))
                filePath = MainWindowViewModel.ImageDirectory + @"\Empty.png";

            Uri fileUri = new Uri(filePath);
            var image = new Image {Source = new BitmapImage(fileUri)};

            if(height!=0)
            {
                image.Height = height * 3.675;
            }

            if (width != 0)
            {
                image.Width = width * 3.675;
            }

            return image;
        }

        public static TextBox CreateTextArea(string text, double height, double width, SolidColorBrush fill, int fontsize)
        {
            return new TextBox()
            {
                Text = text,
                Foreground = fill,
                FontSize = fontsize,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                AcceptsReturn = false,
                Margin= new Thickness(0),
                Background = null,
                BorderThickness = new Thickness(0),
                IsReadOnly = true,
                Width = width * 3.675,
                Height = height * 3.675,
                Padding= new Thickness(10),
                TextWrapping= TextWrapping.Wrap
            };
        }

        public static DataGrid CreateDataTable(DataTable data, double height, double width)
        {
            return new DataGrid()
            {
                ItemsSource = data.AsDataView(),
                Height = height * 3.675,
                Width = width * 3.675,
                VerticalContentAlignment= VerticalAlignment.Center,
                HorizontalContentAlignment= HorizontalAlignment.Center,
                VerticalAlignment= VerticalAlignment.Center,
                HorizontalAlignment= HorizontalAlignment.Center
            };
        }

        public static void Render(BorderedRectangle background, List<Element> elemente, Canvas canvas)
        {
            canvas.Children.Clear();

            var rectangle = background;
            var backgroundRect = Renderer.CreateSimpleRectangle(rectangle.Fill, rectangle.Width, rectangle.Height, rectangle.CornerRadius);

            Canvas.SetTop(backgroundRect, 0);
            Canvas.SetLeft(backgroundRect, 0);
            canvas.Children.Add(backgroundRect);

            if (elemente == null || !elemente.Any())
                return;

            var tempList = elemente.ToList();
            tempList.Reverse();
            tempList.ForEach(element =>
            {
                if(element.Type == "Rectangle")
                {
                    var serializedParent = JsonConvert.SerializeObject(element);
                    BorderedRectangle child = JsonConvert.DeserializeObject<BorderedRectangle>(serializedParent);

                    var renderedElement_Rect = Renderer.CreateRectangle(element.Fill, element.Width, element.Height, child.CornerRadius, element.BorderThickness, element.BorderColor, element.Fontsize==-1?true:false);
                    Canvas.SetTop(renderedElement_Rect, element.Location_Y * 3.675);
                    Canvas.SetLeft(renderedElement_Rect, element.Location_X * 3.675);
                    canvas.Children.Add(renderedElement_Rect);
                }
                else if (element.Type == "Circle")
                {
                    var renderedElement_Circ = Renderer.CreateCircle(element.Fill, element.Width, element.Height, element.BorderThickness, element.BorderColor);
                    Canvas.SetTop(renderedElement_Circ, element.Location_Y * 3.675);
                    Canvas.SetLeft(renderedElement_Circ, element.Location_X * 3.675);
                    canvas.Children.Add(renderedElement_Circ);
                }
                else if (element.Type == "Icon")
                {
                    var image = CreateImage(element.ImageFileName, element.Height, element.Width);
                    if (image != null)
                    {
                        Canvas.SetTop(image, element.Location_Y * 3.675);
                        Canvas.SetLeft(image, element.Location_X * 3.675);
                        canvas.Children.Add(image);
                    }
                }
                else if (element.Type == "TextArea" && element.Fontsize > 0)
                {
                    var textArea = CreateTextArea(element.Name, element.Height, element.Width, element.Fill, element.Fontsize);
                    Canvas.SetTop(textArea, element.Location_Y * 3.675);
                    Canvas.SetLeft(textArea, element.Location_X * 3.675);
                    canvas.Children.Add(textArea);
                }
                else if(element.Type == "Table")
                {
                    var table = CreateDataTable(MainWindowViewModel.Main.Session.Design.Daten, element.Height, element.Width);
                    Canvas.SetTop(table, element.Location_Y * 3.675);
                    Canvas.SetLeft(table, element.Location_X * 3.675);
                    canvas.Children.Add(table);
                }
            });
        }

        public static bool RenderRow(Session currentSession, DataRow row)
        {
            try
            {
                var exportElemente = new List<Element>();
                var currentElemente =  Helper.GetListAsValue(currentSession.Design.Elemente);
                currentElemente.ForEach(element =>
                {
                    if (!string.IsNullOrEmpty(element.DataContext) && (element.Type == "Rectangle" || element.Type == "Circle"))
                    {
                        var text = "";
                        var textColor = MainWindow.Black;

                        if (element.DataContext.Contains("\""))
                        {
                            textColor = Helper.IsDarkColor(element.Fill) ? MainWindow.White : MainWindow.Black;
                            var dataContext = element.DataContext;
                            if (element.DataContext.Contains(";"))
                            {
                                text = dataContext.Replace(";", "\n");
                            }
                            else
                            {
                                text = dataContext;
                            }
                            text = text.Replace("\"", "");
                            var textArea = new TextArea(text, element.Width, element.Height, element.Location_X, element.Location_Y, textColor, element.Fontsize);
                            exportElemente.Insert(0, textArea);
                        }
                        else
                        {
                            textColor = Helper.IsDarkColor(element.Fill) ? MainWindow.White : MainWindow.Black;
                            var dataContext = row[element.DataContext].ToString();
                            text = dataContext;
                            if (dataContext.Contains("*"))
                            {
                                element.Fontsize = -1;
                            }
                            else if (dataContext.Contains(".png"))
                            {
                                var image = new Icon("", element.Width * 0.75, element.Height * 0.75, element.Location_X, element.Location_Y, MainWindowViewModel.ImageDirectory + @"\" + dataContext);
                                image.AlignTo(element, "CENTER", "CENTER");
                                exportElemente.Insert(0, image);
                            }
                            else if (dataContext.Contains("->"))
                            {
                                if (dataContext.Contains("("))
                                {
                                    if (dataContext.Contains("x"))
                                    {
                                        var parts = dataContext.Split('(')[1].Replace(")", "").Split('x').ToList();
                                        var part1 = parts[0];
                                        if (parts[0].Contains("->"))
                                            part1 = Helper.TransformMathFormular(parts[0]);
                                        var part2 = parts[1];
                                        if (parts[1].Contains("->"))
                                            part2 = Helper.TransformMathFormular(parts[1]);

                                        text = part1 + "/" + part2;
                                    }
                                    else
                                    {
                                        var innerText = dataContext.Split('(')[1].Replace(")", "");
                                        text = Helper.TransformMathFormular(innerText);
                                    }
                                }
                                else text = Helper.TransformMathFormular(dataContext);
                            }
                            else if (dataContext.Contains("|"))
                            {
                                text = dataContext.Replace(" | ", "\n");
                            }

                            var textArea = new TextArea(text, element.Width, element.Height, element.Location_X, element.Location_Y, textColor, element.Fontsize);
                            exportElemente.Insert(0, textArea);
                        }
                    }
                    else if (element.Type == "Icon" && !string.IsNullOrEmpty(element.DataContext) && string.IsNullOrEmpty(element.ImageFileName))
                    {
                        var dataContext = row[element.DataContext].ToString().Replace("*", "");
                        if (dataContext == "Ja" || dataContext == "Nein")
                        {
                            if (dataContext == "Ja")
                                element.ImageFileName =  element.DataContext + ".png";
                            else element.ImageFileName =  @"\Placeholder.png";
                        }
                        else element.ImageFileName =  dataContext + ".png";
                    }
                });

                exportElemente.AddRange(currentElemente);

                MainWindow.Window.Dispatcher.Invoke(() =>
                {
                    MainWindow.Window.Canvas_Export.Children.Clear();
                    MainWindow.Window.Canvas_Export.Visibility = Visibility.Visible;

                    Render(currentSession.Design.Background, exportElemente, MainWindow.Window.Canvas_Export);
                });

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

        public static bool Export(Canvas canvas, string designName, string fileName, double width, double height)
        {
            try
            {
                var exportDirectory = MainWindowViewModel.SessionsDirectory + $@"\Export\{designName}";
                if (!Directory.Exists(exportDirectory))
                {
                    Directory.CreateDirectory(exportDirectory);
                }
                var filePath = exportDirectory + @"\" + fileName + ".png";

                var currentSession = MainWindowViewModel.Main.Session;
                Rect bounds = new Rect(new Size(width, height));
                double dpi = 96d;

                RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, PixelFormats.Default);

                DrawingVisual dv = new DrawingVisual();
                using (DrawingContext dc = dv.RenderOpen())
                {
                    VisualBrush vb = new VisualBrush(canvas);
                    dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
                }

                rtb.Render(dv);

                BitmapEncoder pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

                MemoryStream ms = new MemoryStream();

                pngEncoder.Save(ms);
                ms.Close();

                File.WriteAllBytes(filePath, ms.ToArray());

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

        public static bool Collage(string sessionFileName, DataTable daten, double elementHeight, double elementWidth)
        {
            try
            {
                var collageWindow = new CollageWindow();
                var exportDirectory = MainWindowViewModel.SessionsDirectory + $@"\Export\{sessionFileName}";

                var exportedFiles = Directory.GetFiles(exportDirectory).ToList();
                if (!exportedFiles.Any())
                    return true;

                var filteredFilesList = exportedFiles.Where(file=> !file.Contains("Page")).ToList();

                var targetFileList = new List<string>();
                for (int i = 0; i < filteredFilesList.Count; i++)
                {
                    targetFileList.Add(filteredFilesList[i]);
                    if (daten.Columns.Contains("Anzahl"))
                    {
                        var fileInfo = new FileInfo(filteredFilesList[i]);
                        var fileName = fileInfo.Name.Replace(fileInfo.Extension, "");
                        var targetRows = daten.Select($"Name='{fileName}'");
                        if(targetRows != null && targetRows.Any())
                        {
                            var targetRow = targetRows[0];
                            if (targetRow != null)
                            {
                                var targetCount = Convert.ToInt32(targetRow["Anzahl"]);
                                if (targetCount > 1)
                                {
                                    for (int j = 1; j < targetCount; j++)
                                        targetFileList.Add(filteredFilesList[i]);
                                }
                            }
                        }
                    }
                }

                var dinA4_PrintArea_Height = (210 - 63.4)* 3.675;
                var dinA4_PrintArea_Width = (297 - 50.8)* 3.675;
                var columns = Math.Round(dinA4_PrintArea_Width / elementWidth,0);
                var rows = Math.Round(dinA4_PrintArea_Height / elementHeight,0);
                var numberOfElementsPerPage = columns * rows;
                var pageCount = Math.Ceiling((double)(targetFileList.Count/ numberOfElementsPerPage));

                collageWindow.CollageWrap.Children.Clear();
                CollageWindow.Collages = new List<Canvas>();

                var currentFileIndex = 0;
                for (int i = 0; i < pageCount; i++)
                {
                    var collageCanvas = new Canvas() { Height = rows * (elementHeight + 1), Width = columns * (elementWidth + 1), Background = Brushes.White, Margin = new Thickness(20), HorizontalAlignment=HorizontalAlignment.Center };

                    for (int row = 0; row < rows; row++)
                    {
                        for(int col = 0; col < columns; col++)
                        {
                            if (currentFileIndex < targetFileList.Count)
                            {
                                var left = col * (elementWidth+1);
                                var top = row * (elementHeight+1);

                                Uri fileUri = new Uri(targetFileList[currentFileIndex]);
                                var image = new Image { Source = new BitmapImage(fileUri), Height = elementHeight, Width = elementWidth };

                                Canvas.SetTop(image, top);
                                Canvas.SetLeft(image, left);

                                collageCanvas.Children.Add(image);
                            }
                            else break;

                            currentFileIndex++;
                        }
                    }

                    CollageWindow.Collages.Add(collageCanvas);
                    collageWindow.CollageWrap.Children.Add(collageCanvas);
                }

                collageWindow.Show();

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
