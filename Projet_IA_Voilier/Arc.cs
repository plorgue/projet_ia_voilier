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
        public Point P1 {
            get
            {
                return new Point(X1, Y1); 
            }
            set
            {
                X1 = value.X;
                Y1 = value.Y;
            } 
        }
        public Point P2
        {
            get
            {
                return new Point(X2, Y2); 
            }
            set
            {
                X2 = value.X;
                Y2 = value.Y;
            }
        }

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

        public Line GetLineScaled(double scale, double shiftLeft, double shiftTop)
        {
            return new Line() { 
                X1 = this.X1 * scale + shiftLeft, Y1 = this.Y1 * scale + shiftTop, 
                X2 = this.X2 * scale + shiftLeft, Y2 = this.Y2 * scale + shiftTop };
        }


    }
}
