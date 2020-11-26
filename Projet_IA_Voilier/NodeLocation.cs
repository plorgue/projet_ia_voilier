using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace Projet_IA_Voilier
{
    class NodeLocation : GenericNode
    {
        public Point Location { get; set; }
        public static char Wind { get; set; }
        public double Theta { get; set; }

        private readonly int pas = 2;
        private readonly double vMax = 45;


        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="location">Position géographique associées</param>
        /// <param name="wind">Type de vent</param>
        /// <param name="theta">Angle entre l'ordonnée et le vecteur drapeau-bateau</param>
        public NodeLocation(Point location, char wind, double theta)
        {
            Location = location;
            Wind = wind;
            Theta = theta;
        }

        public override double CalculeHCost(GenericNode startingNode, GenericNode destinationNode)
        {
            if(startingNode is NodeLocation && destinationNode is NodeLocation)
            {
                Point startingPoint = (startingNode as NodeLocation).Location;
                Point destination = (destinationNode as NodeLocation).Location;
                
                if (Location.X * Math.Sin(Theta) + Location.Y * Math.Cos(Theta) > startingPoint.X * Math.Sin(Theta) + startingPoint.Y * Math.Cos(Theta))
                    return 1000000;
                else
                    return Math.Sqrt(Math.Pow(Location.X - destination.X, 2) + Math.Pow(Location.Y - destination.Y, 2))/vMax;
            }
            else
            {
                return 0;
            }
        }

        public override bool EndState(GenericNode destinationNode)
        {
            if(destinationNode is NodeLocation)
            {
                Point destination = (destinationNode as NodeLocation).Location;
                double dist = Math.Sqrt(Math.Pow(destination.X-Location.X, 2) + Math.Pow(destination.Y-Location.Y, 2));
                return dist <= pas;
            }
            else
            {
                return false;
            }
        }

        public override double GetArcCost(GenericNode N2)
        {
            if (N2 is NodeLocation)
            {
                double time = TimeEstimation(this.Location, (N2 as NodeLocation).Location);
                return time;
            }
            else
                return 1000000;
        }

        public override List<GenericNode> GetListSucc()
        {
            List<GenericNode> nodes = new List<GenericNode>();
            for(int i = -1; i <= 1; i++)
            {
                for(int j = -1; j <= 1; j++)
                {
                    if (!(j == 0 && i == 0))
                    {
                        Point newPoint = new Point(Location.X + i*pas, Location.Y + j*pas);
                        NodeLocation newNode = new NodeLocation(newPoint, Wind, Theta);
                        nodes.Add(newNode);
                    }
                }
            }
            return nodes;
        }

        public override bool IsEqual(GenericNode N2)
        {
            if(N2 is NodeLocation)
            {
                NodeLocation L2 = (N2 as NodeLocation);
                return (L2.Location.X == this.Location.X && L2.Location.Y == this.Location.Y);
            }
            else
            {
                return false;
            }
        }

        public double TimeEstimation(Point p1, Point p2)
        {
            double x1 = p1.X;
            double y1 = p1.Y;
            double x2 = p2.X;
            double y2 = p2.Y;
            double distance = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
            if (distance > 10) return 1000000;
            double windspeed = GetWindSpeed((y1 + y2) / 2.0);
            double winddirection = GetWindDirection((y1 + y2) / 2.0);
            double boatspeed;
            double boatdirection = Math.Atan2(y2 - y1, x2 - x1) * 180 / Math.PI;
            // On ramène entre 0 et 360
            if (boatdirection < 0) boatdirection += 360;
            boatdirection += 90; // car repère inversé (y vers le bas)  

            // calcul de la différence angulaire
            double alpha = Math.Abs(boatdirection - winddirection);
            // On se ramène à une différence entre 0 et 180 :
            if (alpha > 180) alpha = 360 - alpha;   
            if (alpha <= 45)
            {
                /* (0.6 + 0.3α / 45) v_v */
                boatspeed = (0.6 + 0.3 * alpha / 45) * windspeed;
            }
            else if (alpha <= 90)
            {
                /*v_b=(0.9-0.2(α-45)/45) v_v */
                boatspeed = (0.9 - 0.2 * (alpha - 45) / 45) * windspeed;
            }
            else if (alpha < 150)
            {
                /* v_b=0.7(1-(α-90)/60) v_v */
                boatspeed = 0.7 * (1 - (alpha - 90) / 60) * windspeed;
            }
            else
                return 1000000;
            // estimation du temps de navigation entre p1 et p2

            return (distance / boatspeed);
        }
        
        public double GetWindSpeed(double y)
        {
            if (Wind == 'a')
                return 50;
            else if (Wind == 'b')
                if (y > 150)
                    return 50;
                else return 20;
            else if (y > 150)
                return 50;
            else return 20;
        }

        public double GetWindDirection(double y)
        {
            if (Wind == 'a')
                return 30;
            else if (Wind == 'b')
                if (y > 150)
                    return 180;
                else return 90;
            else if (y > 150)
                return 170;
            else return 65;
        }







        public static void Sortie(string desc, object value)
        {
            string TAG = "NODELOCATION";
            Debug.WriteLine(new StringBuilder(TAG).Append(": ").Append(desc).Append(": ").Append(value.ToString()).ToString());
        }
    }

}
