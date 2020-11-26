using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace Projet_IA_Voilier
{

    class SearchTree    
    {
        public List<GenericNode> lOuverts;
        public List<GenericNode> lFermes;
        public GenericNode StartingNode { get; set; }
        public GenericNode DestinationNode { get; set; }

        public SearchTree(GenericNode startintPoint, GenericNode destination)
        {   
            StartingNode = startintPoint;
            DestinationNode = destination;
        }

        public int CountInOpenList()
        {
            return lOuverts.Count;
        }
        public int CountInClosedList()
        {
            return lFermes.Count;
        }

        private GenericNode ChercheNodeDansFermes(GenericNode N2)
        {
            int i = 0;

            while (i < lFermes.Count)
            {
                if (lFermes[i].IsEqual(N2))
                    return lFermes[i];
                i++;
            }
            return null;
        }

        private GenericNode ChercheNodeDansOuverts(GenericNode N2)
        {
            int i = 0;

            while (i < lOuverts.Count)
            {
                if (lOuverts[i].IsEqual(N2))
                    return lOuverts[i];
                i++;
            }
            return null;
        }

        public List<GenericNode> RechercheSolutionAEtoile(out int nbrFerme, out int nbrOuvert, out long duration ,out List<Arc> arcs)
        {
            long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            Sortie("simu", "start");
            arcs = new List<Arc>();

            lOuverts = new List<GenericNode>();
            lFermes = new List<GenericNode>();
            // Le noeud passé en paramètre est supposé être le noeud initial
            GenericNode N = StartingNode;
            lOuverts.Add(StartingNode);

            // tant que le noeud n'est pas terminal et que ouverts n'est pas vide
            while (lOuverts.Count != 0 && N.EndState(DestinationNode) == false)
            {
                if ((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - milliseconds > 20 * 60 * 1000) 
                    break;
                // Le meilleur noeud des ouverts est supposé placé en tête de liste
                // On le place dans les fermés
                lOuverts.Remove(N);
                lFermes.Add(N);

                if(N.GetNoeud_Parent() != null)
                {
                    arcs.Add(new Arc((N.GetNoeud_Parent() as NodeLocation).Location, (N as NodeLocation).Location));
                }

                // Il faut trouver les noeuds successeurs de N
                this.MAJSuccesseurs(N);
                // Inutile de retrier car les insertions ont été faites en respectant l'ordre

                // On prend le meilleur, donc celui en position 0, pour continuer à explorer les états
                // A condition qu'il existe bien sûr
                if (lOuverts.Count > 0)
                {
                    N = lOuverts[0];
                }
                else
                {
                    N = null;
                }
            }

            // A* terminé
            // On retourne le chemin qui va du noeud initial au noeud final sous forme de liste
            // Le chemin est retrouvé en partant du noeud final et en accédant aux parents de manière
            // itérative jusqu'à ce qu'on tombe sur le noeud initial
            List<GenericNode> _LN = new List<GenericNode>();
            if (N != null)
            {
                _LN.Add(N);

                while (N != StartingNode)
                {
                    N = N.GetNoeud_Parent();
                    _LN.Insert(0, N);  // On insère en position 1
                }
            }

            Sortie("simu", "end");
            duration = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - milliseconds;
            nbrOuvert = lOuverts.Count;
            nbrFerme = lFermes.Count;
            return _LN;
        }

        public void MAJSuccesseurs(GenericNode N)
        {
            // On fait appel à GetListSucc, méthode abstraite qu'on doit réécrire pour chaque
            // problème. Elle doit retourner la liste complète des noeuds successeurs de N.
            List<GenericNode> listsucc = N.GetListSucc();
            foreach (GenericNode N2 in listsucc)
            {
                // N2 est-il une copie d'un nœud déjà vu et placé dans la liste des fermés ?
                GenericNode N2bis = ChercheNodeDansFermes(N2);
                if (N2bis == null)
                {
                    // Rien dans les fermés. Est-il dans les ouverts ?
                    N2bis = ChercheNodeDansOuverts(N2);
                    if (N2bis != null)
                    {
                        // Il existe, donc on l'a déjà vu, N2 n'est qu'une copie de N2Bis
                        // Le nouveau chemin passant par N est-il meilleur ?
                        if (N.GetGCost() + N.GetArcCost(N2) < N2bis.GetGCost())
                        {
                            // Mise à jour de N2bis
                            N2bis.SetGCost(N.GetGCost() + N.GetArcCost(N2));
                            // HCost pas recalculé car toujours bon
                            N2bis.RecalculeCoutTotal(); // somme de GCost et HCost
                            // Mise à jour de la famille ....
                            N2bis.Supprime_Liens_Parent();
                            N2bis.SetNoeud_Parent(N);
                            // Mise à jour des ouverts
                            lOuverts.Remove(N2bis);
                            this.InsertNewNodeInOpenList(N2bis);
                        }
                        // else on ne fait rien, car le nouveau chemin est moins bon
                    }
                    else
                    {
                        // N2 est nouveau, MAJ et insertion dans les ouverts
                        N2.SetGCost(N.GetGCost() + N.GetArcCost(N2));
                        N2.SetNoeud_Parent(N);
                        N2.CalculCoutTotal(StartingNode, DestinationNode); // somme de GCost et HCost
                        //Sortie("Total", N2.GetHCost());
                        this.InsertNewNodeInOpenList(N2);
                    }
                }
                // else il est dans les fermés donc on ne fait rien,
                // car on a déjà trouvé le plus court chemin pour aller en N2
            }
        }

        public void InsertNewNodeInOpenList(GenericNode NewNode)
        {
            // Insertion pour respecter l'ordre du cout total le plus petit au plus grand
            if (this.lOuverts.Count == 0)
                lOuverts.Add(NewNode); 
            else
            {
                GenericNode N = lOuverts[0];
                bool trouve = false;
                int i = 0;
                do
                    if (NewNode.Cout_Total < N.Cout_Total)
                    {
                        lOuverts.Insert(i, NewNode);
                        trouve = true;
                    }
                    else
                    {
                        i++;
                        if (lOuverts.Count == i)
                        {
                            N = null;
                            lOuverts.Insert(i, NewNode);
                        }
                        else
                            N = lOuverts[i];
                    }
                while ((N != null) && (trouve == false));
            }
        }
        
        // Si on veut afficher l'arbre de recherche, il suffit de passer un treeview en paramètres
        // Celui-ci est mis à jour avec les noeuds de la liste des fermés, on ne tient pas compte des ouverts
        public void GetSearchTree(System.Windows.Controls.TreeView TV)
        {
            if (lFermes == null) return;
            if (lFermes.Count == 0) return;

            // On suppose le TreeView préexistant
            TV.Items.Clear();

            TreeNode TN = new TreeNode(lFermes[0].ToString());
            TV.Items.Add(TN);

            AjouteBranche(lFermes[0], TN);

        }
        // AjouteBranche est exclusivement appelée par GetSearchTree; les noeuds sont ajoutés de manière récursive
        private void AjouteBranche(GenericNode GN, TreeNode TN)
        {
            foreach (GenericNode GNfils in GN.GetEnfants())
            {
                TreeNode TNfils = new TreeNode(GNfils.ToString());
                TN.Nodes.Add(TNfils);
                if (GNfils.GetEnfants().Count > 0) AjouteBranche(GNfils, TNfils);
            }
        }
        


        public static void Sortie(string desc, object value)
        {
            string TAG = "SEARCHTREE";
            Debug.WriteLine(new StringBuilder(TAG).Append(": ").Append(desc).Append(": ").Append(value.ToString()).ToString());
        }
    }
}
