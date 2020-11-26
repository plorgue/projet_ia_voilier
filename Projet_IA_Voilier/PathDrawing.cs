using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Projet_IA_Voilier
{
    public class PathDrawing
    {
        // controls
        private readonly Canvas canvas;
        private Image boat, flag;
        private readonly List<Line> lines = new List<Line>();

        // dimensions
        private Thickness offsets;
        private double seaSize;
        private double mapSize;
        private readonly double height;
        private readonly double width;
        private double padding;
        private readonly double iconSize = 50;
        private double scale;
        private readonly int maxCoordinates = 300;

        // apparence
        //double lineThickness = 1;

        public PathDrawing(Canvas canvas, double height, double width)
        {
            this.canvas = canvas;
            this.height = height;
            this.width = width;

            InitializeProperties();

            //J'enlève tous les dessins, utilie quand on redimensionne la fenêtre
            canvas.Children.Clear();

            // L'image est un peu trop claire donc j'ajoute un rectangle par dessus pour l'assombrir
            DarkenBackground(); 

            CreateBoatAndFlag();
        }

        private void InitializeProperties()
        {
            padding = iconSize;
            seaSize = Math.Min(height, width);
            mapSize = seaSize - 2 * padding;
            double margin = (Math.Max(height, width) - seaSize) / 2;
            offsets = height > width ? (new Thickness(padding, margin + padding, padding, margin + padding)) : (new Thickness(margin + padding, padding, margin + padding, padding));
            scale = mapSize / maxCoordinates;
        }

        private void DarkenBackground()
        {
            byte color = 0;
            byte alpha = (byte)(0.3 * 255);
            Rectangle rectangle = new Rectangle
            {
                Width = seaSize,
                Height = seaSize,
                Fill = new SolidColorBrush(Color.FromArgb(alpha, color, color, color)),
                Margin = new Thickness(offsets.Left - padding, offsets.Top - padding, offsets.Right - padding, offsets.Bottom - padding)
            };
            canvas.Children.Add(rectangle);
        }

        /// <summary>
        /// Initialise boat et flag pour les utiliser en tant que point de départ et d'arriver
        /// </summary>
        private void CreateBoatAndFlag()
        {
            boat = new Image
            {
                Height = iconSize,
                Width = iconSize,
                Visibility = Visibility.Hidden,
                Source = new BitmapImage(new Uri(@"http://zezete2.z.e.pic.centerblog.net/o/7db60b64.png"))
            };
            flag = new Image
            {
                Height = iconSize,
                Width = iconSize,
                Visibility = Visibility.Hidden,
                Source = new BitmapImage(new Uri(@"https://cdn.pixabay.com/photo/2016/09/01/15/42/flag-1636453_960_720.png"))
            };
            canvas.Children.Add(flag);
            canvas.Children.Add(boat);
        }

        /// <summary>
        /// Ajoute une étape au trajet et dessine les modifications
        /// </summary>
        /// <param name="point"></param>
        public void DrawArc(Arc arc, Color lineColor, double lineThickness)
        {
            Line line = arc.GetLineScaled(scale, offsets.Left, offsets.Top);
            line.Stroke = new SolidColorBrush(lineColor);
            line.StrokeThickness = lineThickness;
            lines.Add(line);
            canvas.Children.Add(line);
        }

        public void DrawArcs(List<Arc> arcs, Color lineColor, double lineThichness)
        {
            foreach(Arc arc in arcs)
            {
                DrawArc(arc, lineColor, lineThichness);
            }
        }

        /// <summary>
        /// Affiche le bateau, le point de départ
        /// </summary>
        /// <param name="point"></param>
        public void DrawStart(Point point)
        {
            Point start = new Point
            {
                X = point.X * mapSize / maxCoordinates + offsets.Left,
                Y = point.Y * mapSize / maxCoordinates + offsets.Top,
            };

            boat.Visibility = Visibility.Visible;
            boat.Margin = new Thickness(start.X - iconSize / 2, start.Y - iconSize / 1.1, 0, 0);
            
        }

        /// <summary>
        /// Affiche le drapeau, l'objectif
        /// </summary>
        /// <param name="destination"></param>
        public void DrawDestination(Point destination)
        {
            double x = destination.X * mapSize / maxCoordinates - iconSize / 5 + offsets.Left;
            double y = destination.Y * mapSize / maxCoordinates - iconSize + offsets.Top;

            flag.Visibility = Visibility.Visible;
            flag.Margin = new Thickness(x, y, 0, 0);
        }

        public void ClearDraw()
        {

            List<UIElement> elementsToRemove = new List<UIElement>(); 
            for(int i = 1; i < canvas.Children.Count; i++)
            {
                if (canvas.Children[i] is Line)
                    elementsToRemove.Add(canvas.Children[i]);
            }
            foreach(UIElement element in elementsToRemove)
                canvas.Children.Remove(element);


        }



        public void Sortie(string desc, object value)
        {
            string TAG = "PATHDRAWING";
            Debug.WriteLine(new StringBuilder(TAG).Append(": ").Append(desc).Append(": ").Append(value.ToString()).ToString());
        } 


    }
}
