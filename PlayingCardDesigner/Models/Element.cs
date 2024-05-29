using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PlayingCardDesigner.Models
{
    public class Element : INotifyPropertyChanged
    {
        private string name;
        private double width;
        private double height;
        private double location_Y;
        private double location_X;
        private double borderThickness;
        private SolidColorBrush fill;
        private SolidColorBrush borderColor;
        private string dataContext;
        private string imageFileName;
        private int fontsize;
        private int cornerRadius;
        private string type;
        private bool isSelected;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        public double Width
        {
            get { return width; }
            set
            {
                width = value;
                OnPropertyChanged();
            }
        }

        public double Height
        {
            get { return height; }
            set
            {
                height = value;
                OnPropertyChanged();
            }
        }

        public double Location_X
        {
            get { return location_X; }
            set
            {
                location_X = value;
                OnPropertyChanged();
            }
        }

        public double Location_Y
        {
            get { return location_Y; }
            set
            {
                location_Y = value;
                OnPropertyChanged();
            }
        }

        public double BorderThickness
        {
            get { return borderThickness; }
            set
            {
                borderThickness = value;
                OnPropertyChanged();
            }
        }

        public SolidColorBrush BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                OnPropertyChanged();
            }
        }

        public SolidColorBrush Fill
        {
            get { return fill; }
            set
            {
                fill = value;
                OnPropertyChanged();
            }
        }

        public string DataContext
        {
            get { return dataContext; }
            set
            {
                dataContext = value;
                OnPropertyChanged();
            }
        }

        public string ImageFileName
        {
            get { return imageFileName; }
            set
            {
                imageFileName = value;
                OnPropertyChanged();
            }
        }

        public int CornerRadius
        {
            get { return cornerRadius; }
            set
            {
                cornerRadius = value;
                OnPropertyChanged();
            }
        }

        public int Fontsize
        {
            get { return fontsize; }
            set
            {
                fontsize = value;
                OnPropertyChanged();
            }
        }

        public string Type
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public string SizeAndLocation
        {
            get
            {
                return $"Y:{Location_Y} X:{Location_X} | H:{Height} W:{Width}";
            }
        }


        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }

        public Element()
        {
        }

        public Element(string name, double width, double height, double location_X, double location_Y, double borderThickness, SolidColorBrush borderColor, SolidColorBrush fill, int fontsize, string datacontext)
        {
            Name = name;
            Width = width;
            Height = height;
            Location_X = location_X;
            Location_Y = location_Y;
            BorderThickness = borderThickness;
            BorderColor = borderColor;
            Fill = fill;
            Fontsize = fontsize;
            DataContext = datacontext;
        }

        public void AlignTo (Element element, string verticalDirection, string horizontalDirection)
        {
            if(horizontalDirection == "CENTER")
            {
                var centerPoint = element.GetCenterPoint();
                Location_X = centerPoint.X - (Width / 2);
            }
            else if (horizontalDirection == "LEFT")
            {
                Location_X = element.Location_X;
            }
            else if(horizontalDirection == "RIGHT")
            {
                var right_X = element.location_X + element.Width;
                var target_X = right_X- Width;
                Location_X = target_X;
            }

            if (verticalDirection == "CENTER")
            {
                var centerPoint = element.GetCenterPoint();
                Location_Y = centerPoint.Y - (Height / 2);
            }
            else if (verticalDirection == "TOP")
            {
                Location_Y = element.Location_Y;
            }
            else if (verticalDirection == "BOTTOM")
            {
                var bottom_Y = element.location_Y + element.Height;
                var target_Y = bottom_Y - Height;
                Location_Y = target_Y;
            }
        }

        public Point GetCenterPoint()
        {
            return new Point(Location_X+(Width / 2), Location_Y+(Height / 2));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
