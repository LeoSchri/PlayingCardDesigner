using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PlayingCardDesigner.Models
{
    public class Table : Element
    {
        public Table()
        {
            Type = "Table";
        }

        public Table(string name, double width, double height, double location_X, double location_Y) : base(name, width, height, location_X, location_Y, 0, null, null, 0, null)
        {
            Type = "Table";
        }
    }
}
