using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Projet_IA_Voilier
{
    public partial class MainWindow : Window
    {
        public PathDrawing pathDrawing { get; set; }
        
        int NbrOfColumn = 4; // nombre de colonne dans le control panel
        private Dictionary<string, string> hints = new Dictionary<string, string>();
        Point startingPoint = new Point(0, 0);
        Point destinationPoint = new Point(0, 0);
        char wind = 'a';
        List<Arc> path = new List<Arc>(); 
        long mockUpDuration = 0;
        int nbrOuvert = 0;
        int nbrFerme = 0;
        List<Arc> arcs = new List<Arc>();
        double totalTime = 0;

        public MainWindow()
        {
            InitializeComponent();
            InitWindow();
            InitHintDictionnary();
        }

        // ------ Méthodes -------

        // Initialisation
        private void InitWindow()
        {
            double height = SystemParameters.PrimaryScreenHeight;
            double width = SystemParameters.PrimaryScreenWidth;
            this.Resources["ColumnWidth_ControlPanel"] = new GridLength(Math.Max(width / NbrOfColumn, 270));
            
        }
        private void InitHintDictionnary()
        {
            hints.Add("tb_xInit", "0 - 300");
            hints.Add("tb_yInit", "0 - 300");
            hints.Add("tb_xTarget", "0 - 300");
            hints.Add("tb_yTarget", "0 - 300");
            hints.Add("tb_windDir", "En degrée");
            hints.Add("tb_windSpeed", "En km/h");
        }

         
        // Evenements
        private void CanvasSea_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            pathDrawing = new PathDrawing(canvasSea, e.NewSize.Height, e.NewSize.Width);
            ShowResult();
        }

        /// <summary>
        /// Lance la simulation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRun_Click(object sender, RoutedEventArgs e)
        {
            tb_running.Visibility = Visibility.Visible;
            tb_running.Text = "Simulation en cours";
            tb_running.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            arcs.Clear();
            path.Clear();
            var t = Task.Run(() => RunMockUp()).ContinueWith((antecedent) => ShowResult());
        }

        /// <summary>
        /// Enleve l'hint de la textbox quand elle est focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditText_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (sender as TextBox);
            if (textBox.Text.Equals(hints[textBox.Name]))
            {
                textBox.Text = "";
                textBox.Foreground = (SolidColorBrush) Application.Current.Resources["TextColorDark"];
            }
        }

        /// <summary>
        /// Ajoute l'hint à la textbox si elle est vide quand elle perd le focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditText_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (sender as TextBox);
            if (textBox.Text.Equals(""))
            {
                textBox.Text = hints[textBox.Name];
                textBox.Foreground = (SolidColorBrush) Application.Current.Resources["TextColorHint"];
            }
        }

        private void xInit_TextChanged(object sender, TextChangedEventArgs e)
        {
            AllocateTextBoxIntValue(sender);
            TryShowBoat();
        }

        private void yInit_TextChanged(object sender, TextChangedEventArgs e)
        {
            AllocateTextBoxIntValue(sender);
            TryShowBoat();
        }

        private void xTarget_TextChanged(object sender, TextChangedEventArgs e)
        {
            AllocateTextBoxIntValue(sender);
            TryShowFlag();
        }

        private void yTarget_TextChanged(object sender, TextChangedEventArgs e)
        {
            AllocateTextBoxIntValue(sender);
            TryShowFlag();
        }
        private void rb_wind_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton)
            {
                if(RotateTransformWindArrowDown != null && RotateTransformWindArrowUp != null)
                {
                    switch ((sender as RadioButton).Name)
                    {                   
                    case "rb_windA":
                        wind = 'a';
                        RotateTransformWindArrowUp.Angle = 30;
                        RotateTransformWindArrowDown.Angle = 30;
                        break;
                    case "rb_windB":
                        wind = 'b';
                        RotateTransformWindArrowUp.Angle = 90;
                        RotateTransformWindArrowDown.Angle = 180;
                        break;
                    case "rb_windC":
                        wind = 'c';
                        RotateTransformWindArrowUp.Angle = 65;
                        RotateTransformWindArrowDown.Angle = 170;
                        break;
                    }
                }
            }
        }


        // Autres méthodes
        private void RunMockUp()
        {
            if (!tb_xInit.Equals("") &&
                !tb_yInit.Equals("") &&
                !tb_xTarget.Equals("") &&
                !tb_yTarget.Equals(""))
            {
                double theta = GetCoordSystemThetaRotation();
                NodeLocation startingNode = new NodeLocation(startingPoint, wind, theta);
                NodeLocation destination = new NodeLocation(destinationPoint, wind, theta);
                SearchTree search = new SearchTree(startingNode, destination);
                List<GenericNode> res = search.RechercheSolutionAEtoile(out nbrFerme, out nbrOuvert, out mockUpDuration, out arcs);

                totalTime = res[res.Count - 1].GetGCost();
                for(int i = 1; i < res.Count; i++)
                {
                    NodeLocation node1 = res[i - 1] as NodeLocation;
                    NodeLocation node2 = res[i] as NodeLocation;
                    path.Add(new Arc
                    {
                        X1 = node1.Location.X,
                        X2 = node2.Location.X,
                        Y1 = node1.Location.Y,
                        Y2 = node2.Location.Y
                    });
                }
            }            
        }
        private void ShowResult()
        {
            Dispatcher.Invoke(() =>
            {
                //notifier fin de simulation
                tb_running.Text = "Simulation finie";
                tb_running.Foreground = new SolidColorBrush(Color.FromRgb(0, 100, 0));
                //clean la vu
                pathDrawing.ClearDraw();
                //dessier les lignes
                Sortie("arcs",arcs.Count);
                pathDrawing.DrawArcs(arcs, Color.FromArgb(120, 255, 255, 255), 1);
                pathDrawing.DrawArcs(path, Color.FromRgb(0, 180, 0), 3);
                //dessiner bateau et drapeau
                pathDrawing.DrawStart(startingPoint);
                pathDrawing.DrawDestination(destinationPoint);
                //afficher le temps, nombre d'ouvert, fermé
                tb_durMockUp.Text = "Durée simulation: " + Math.Round(mockUpDuration / 1000.0,2) + "s";
                tb_durReal.Text = "Durée de navigation estimé: " + Math.Round(totalTime,2) + "h";
                tb_open.Text = "Nombre d'ouvert: " + nbrOuvert;
                tb_close.Text = "Nombre de fermé: " + nbrFerme;
                tb_somme.Text = "Nombre de noeuds total: " + (nbrFerme + nbrOuvert);

            });
        }

        /// <summary>
        /// Set intRef value with text of textbox
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="intRef"></param>
        private void AllocateTextBoxIntValue(object textBox)
        {
            if (textBox is TextBox)
            {
                TextBox tb = (textBox as TextBox);
                int value = -1;
                int.TryParse((textBox as TextBox).Text, out value);
                if (value >= 0)
                {
                    if (tb.Name == tb_xInit.Name)
                        startingPoint.X = value;
                    else if (tb.Name == tb_yInit.Name)
                        startingPoint.Y = value;
                    else if (tb.Name == tb_xTarget.Name)
                        destinationPoint.X = value;
                    else
                        destinationPoint.Y = value;
                }
            }
        }

        public double GetCoordSystemThetaRotation()
        {
            if (startingPoint.X >= destinationPoint.X && startingPoint.Y >= destinationPoint.Y)
                return Math.Atan(Math.Abs(startingPoint.X - destinationPoint.X) / Math.Abs(startingPoint.Y - destinationPoint.Y));
            else if (startingPoint.X >= destinationPoint.X && startingPoint.Y <= destinationPoint.Y)
                return Math.PI / 2 + Math.Atan(Math.Abs(startingPoint.Y - destinationPoint.Y) / Math.Abs(startingPoint.X - destinationPoint.X));
            else if (startingPoint.X <= destinationPoint.X && startingPoint.Y >= destinationPoint.Y)
                return -Math.Atan(Math.Abs(startingPoint.X - destinationPoint.X) / Math.Abs(startingPoint.Y - destinationPoint.Y));
            else
                return Math.PI + Math.Atan(Math.Abs(startingPoint.X - destinationPoint.X) / Math.Abs(startingPoint.Y - destinationPoint.Y));
        }

        private void TryShowBoat()
        {
            if (startingPoint.X < 301 && startingPoint.Y < 301 && pathDrawing != null)
                pathDrawing.DrawStart(startingPoint);
        }

        private void TryShowFlag()
        {
            if (startingPoint.X < 301 && destinationPoint.Y < 301 && pathDrawing != null) 
                pathDrawing.DrawDestination(destinationPoint);
        }

        public void Sortie(string desc, object value)
        {
            string TAG = "MAINWINDOW";
            Debug.WriteLine(new StringBuilder(TAG).Append(": ").Append(desc).Append(": ").Append(value.ToString()).ToString());
        }

    }
}
