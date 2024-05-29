using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PlayingCardDesigner.Models
{
    public class TextArea:Element
    {
        public TextArea()
        {
            Type = "TextArea";
        }

        public TextArea(string name, double width, double height, double location_X, double location_Y, SolidColorBrush fill, int fontsize) : base(name, width, height, location_X, location_Y, 0, null, fill, fontsize, null)
        {
            Type = "TextArea";
        }
    }
}
