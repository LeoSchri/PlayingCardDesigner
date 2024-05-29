using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PlayingCardDesigner.Models
{
    public class Icon:Element
    {
        public Icon()
        {
            Type = "Icon";
        }

        public Icon(string name, double width, double height, double location_X, double location_Y, string fileName) : base(name, width, height, location_X, location_Y, 0, null, null, 0, null)
        {
            ImageFileName = fileName;
            Type = "Icon";
        }
    }
}
