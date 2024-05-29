using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace PlayingCardDesigner.Models
{
    public class BorderedCircle:Element
    {
        public BorderedCircle()
        {
            Type = "Circle";
        }

        public BorderedCircle(string name, double width, double height, double location_X, double location_Y, double borderThickness, SolidColorBrush borderColor, SolidColorBrush fill, int fontsize, string datacontext) : base(name, width, height, location_X, location_Y, borderThickness, borderColor, fill, fontsize, datacontext)
        {
            Type = "Circle";
        }
    }
}
