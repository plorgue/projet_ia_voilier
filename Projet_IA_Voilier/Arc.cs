using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Projet_IA_Voilier
{
    public class Arc
    {
        public double X1 { get; set; }
        public double X2 { get; set; }
        public double Y1 { get; set; }
        public double Y2 { get; set; }

        public Arc()
        {
        }

        public Arc(Point p1, Point p2) : this(p1.X, p2.X, p1.Y, p2.Y)
        {
        }
        public Arc(double x1, double x2, double y1, double y2)
        {
            X1 = x1;
            X2 = x2;
            Y1 = y1;
            Y2 = y2;
        }

    }
}
