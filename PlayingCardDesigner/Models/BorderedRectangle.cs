using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PlayingCardDesigner.Models
{
    public class BorderedRectangle: Element
    {
        public BorderedRectangle()
        {
            Type = "Rectangle";
        }

        public BorderedRectangle(string name, double width, double height, double location_X, double location_Y, int cornerRadius, double borderThickness, SolidColorBrush borderColor, SolidColorBrush fill, int fontsize, string datacontext) :base(name, width, height, location_X, location_Y, borderThickness, borderColor, fill, fontsize, datacontext)
        {
            CornerRadius = cornerRadius;
            Type = "Rectangle";
        }
    }
}
