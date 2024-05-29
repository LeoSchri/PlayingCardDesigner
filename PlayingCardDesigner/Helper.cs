using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Win32;
using PlayingCardDesigner.Models;
using System.Drawing;

namespace PlayingCardDesigner
{
    public static class Helper
    {
        public static string GetFileFromFileDialog(string title, string initialDirectory, string filter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = title,
                InitialDirectory = initialDirectory,
                Filter = filter
            };
            if (openFileDialog.ShowDialog() == true)
                return openFileDialog.FileName;
            else return "";
        }

        public static string FindDropboxFolder()
        {
            var dropboxDirectory = "";
            if (Directory.Exists(@"D:\Shared\Dropbox"))
                dropboxDirectory = @"D:\Shared\Dropbox";
            else if (Directory.Exists(@"C:\Users\Leo\Dropbox"))
                dropboxDirectory = @"C:\Users\Leo\Dropbox";
            else if (Directory.Exists(@"C:\Users\glanzer\Dropbox"))
                dropboxDirectory = @"C:\Users\glanzer\Dropbox";

            return dropboxDirectory;
        }

        public static string GetNextFileName(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            var fileName = fileInfo.Name.Replace(fileInfo.Extension,"");
            if (!File.Exists(filePath))
                return fileName;
            else
            {
                var counter = 1;
                while (true)
                {
                    fileInfo = new FileInfo(filePath);
                    var newFileName = fileInfo.Name.Replace(fileInfo.Extension,"") + "-" + counter;
                    var newPath = fileInfo.Directory + @"\" + newFileName + fileInfo.Extension;
                    if (!File.Exists(newPath))
                    {
                        return newFileName;
                    }
                    else counter++;
                }
            }
        }

        public static string GetNextElementName(Session session, string currentName)
        {
            var newName = currentName;
            if(!session.Design.Elemente.Any())
                return newName;

            var counter = 1;
            while (true)
            {
                var foundElement = session.Design.Elemente.ToList().Find(element => element.Name == newName);
                if(foundElement == null)
                    return newName;
                else
                {
                    newName = currentName + "-" + counter;
                    counter++;
                }
            }
        }

        public static ObservableCollection<string> GetHeaderFromCSV(string csvFile)
        {
            var headers = new ObservableCollection<string>();

            if (string.IsNullOrEmpty(csvFile))
                return headers;

            var lines = File.ReadAllLines(csvFile).ToList();
            if(lines.Any())
            {
                var firstLine = lines.FirstOrDefault().Replace("\"", "");
                headers = new ObservableCollection<string>(firstLine.Split(new char[] { ';' }).ToList());
            }

            return headers;
        }

        public static DataTable GetDataFromCSV(string csvFile)
        {
            var dt = new DataTable();

            if (string.IsNullOrEmpty(csvFile))
                return dt;

            using (StreamReader sr = new StreamReader(csvFile))
            {
                string[] headers = sr.ReadLine().Split(';');
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(';');
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }

        public static bool IsDarkColor(SolidColorBrush color)
        {
            var isDark = false;

            if (color == null)
                return isDark;

            MainWindow.Window.Dispatcher.Invoke(() =>
            {
                var col = ColorTranslator.FromHtml(color.ToString());
                if (col.R * 0.2126 + col.G * 0.7152 + col.B * 0.0722 < 255 / 2)
                {
                    isDark =  true;
                }
                else
                {
                    isDark = false;
                }
            });

            return isDark;
        }

        public static string TransformMathFormular(string formular)
        {
            var result = new List<string>();

            if(formular.Contains("->+"))
            {
                var parts = formular.Split(new[] { "->+" }, StringSplitOptions.RemoveEmptyEntries);
                var basis = Convert.ToDouble(parts[0]);
                var addition = Convert.ToDouble(parts[1]);

                result.Add(basis.ToString());
                var temp = basis;
                for (int i = 0; i < 5; i++)
                {
                    temp += basis;
                    result.Add(temp.ToString());
                }

                return string.Join("\n", result);
            }
            else return formular;
        }

        public static List<Element> GetListAsValue(ObservableCollection<Element> elements)
        {
            var newElements = new List<Element>();
            foreach (var element in elements)
            {
                newElements.Add(GetElementAsValue(element));
            }
            return newElements;
        }

        public static Element GetElementAsValue(Element element)
        {
            var newElement = new Element();

            var props = typeof(Element).GetProperties().ToList();
            foreach (var prop in props)
            {
                try
                {
                    prop.SetValue(newElement, prop.GetValue(element));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return newElement;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        public static extern int MessageBoxTimeout(IntPtr hwnd, String text, String title, uint type, Int16 wLanguageId, Int32 milliseconds);
    }
}
