﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_IA_Voilier
{
    // classe abstraite, il est donc impératif de créer une classe qui en hérite
    // pour résoudre un problème particulier en y ajoutant des infos liées au contexte du problème
    
    abstract public class GenericNode
    {
        protected double GCost;               //coût du chemin du noeud initial jusqu'à ce noeud
        protected double HCost;               //estimation heuristique du coût pour atteindre le noeud final
        protected double TotalCost;           //coût total (g+h)
        protected GenericNode ParentNode;     // noeud parent
        protected List<GenericNode> Enfants;  // noeuds enfants

        //propriétés
        public double Cout_Total
        {
            get { return TotalCost; }
            set { TotalCost = value; }
        }

        // contructeur
        public GenericNode()
        {
            ParentNode = null;
            Enfants = new List<GenericNode>();
        }


        //assesseurs
        public double GetHCost()
        {
            return HCost;
        }

        public double GetGCost()
        {
            return GCost;
        }

        public void SetGCost(double g)
        {
            GCost = g;
        }

        public List<GenericNode> GetEnfants()
        {
            return Enfants;
        }

        public GenericNode GetNoeud_Parent()
        {
            return ParentNode;
        }

        public void SetNoeud_Parent(GenericNode g)
        {
            ParentNode = g;
            g.Enfants.Add(this);
        }

        // Méthodes
        public void Supprime_Liens_Parent()
        {
            if (ParentNode == null) return;
            ParentNode.Enfants.Remove(this);
            ParentNode = null;
        }

        public void CalculCoutTotal(GenericNode N0, GenericNode Nf)
        {
            HCost = CalculeHCost(N0, Nf);
            TotalCost = GCost + HCost;
        }

        public void RecalculeCoutTotal()
        {
            TotalCost = GCost + HCost;
        }

        // Méthodes abstrates
        public abstract bool IsEqual(GenericNode N2);
        public abstract double GetArcCost(GenericNode N2);
        public abstract bool EndState(GenericNode Nf);
        public abstract List<GenericNode> GetListSucc();
        public abstract double CalculeHCost(GenericNode N0, GenericNode Nf);
    }
    
}
