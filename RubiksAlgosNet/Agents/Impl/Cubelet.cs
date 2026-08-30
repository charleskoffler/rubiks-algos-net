using Clprolf.ArchUnitNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using RubiksAlgos.Enums;
using static RubiksAlgos.Agents.ICubelet;

namespace RubiksAlgos.Agents.Impl
{
    [ClDomain]
    public class Cubelet : ICubelet
    {

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Couleur Haut { get; set; }
        public Couleur Bas { get; set; }
        public Couleur Gauche { get; set; }
        public Couleur Droite { get; set; }
        public Couleur Avant { get; set; }
        public Couleur Arriere { get; set; }

        public Cubelet(int x, int y, int z)
        {
            X = x; Y = y; Z = z;
            Haut = Bas = Gauche = Droite = Avant = Arriere = Couleur.X;
        }

        /// <summary>
        /// Renvoie l'identifiant unique de la pièce (ex: "GRW", "RW", "G" ou "CENTRE")
        /// </summary>
        public string Id
        {
            get
            {
                // 1. On liste toutes les couleurs visibles de la pièce
                var couleurs = new[] { Haut, Bas, Gauche, Droite, Avant, Arriere }
                    .Where(c => c != Couleur.X)
                    .Select(c => c.ToString())
                    .OrderBy(c => c); // Tri alphabétique (ex: R, G, W -> G, R, W)

                string id = string.Join("", couleurs);

                // Pour le cubelet central tout noir (0,0,0)
                return string.IsNullOrEmpty(id) ? "CENTRE" : id;
            }
        }

        public List<CubeletSnapshot> Historique { get; } = new();

        public List<Mouvement> ObtenirMouvementsSimplifies()
        {
            return RubiksCubeHelper.SimplifierMouvements(Historique.Select(s => s.MouvementSubiReel));
        }

        // --- ROTATION AXE X (Faces R et L) ---
        public void AppliquerRotationX(bool horaire, Mouvement mvt)
        {
            int tempY = Y, tempZ = Z;
            Y = horaire ? tempZ : -tempZ;
            Z = horaire ? -tempY : tempY;

            // Permutation des 4 faces impactées (Haut, Avant, Bas, Arriere)
            var h = Haut; var av = Avant; var b = Bas; var ar = Arriere;
            if (horaire) { Haut = av; Avant = b; Bas = ar; Arriere = h; }
            else { Haut = ar; Arriere = b; Bas = av; Avant = h; }

            Mouvement mouvement;
            if (horaire) mouvement = Mouvement.R;
            else mouvement = Mouvement.RPrime;

            EnregistrerEtat(mouvement);
        }

        // --- ROTATION AXE Y (Faces U et D) ---
        public void AppliquerRotationY(bool horaire, Mouvement mvt)
        {
            int tempX = X, tempZ = Z;
            X = horaire ? -tempZ : tempZ;
            Z = horaire ? tempX : -tempX;

            // Permutation des 4 faces impactées (Avant, Droite, Arriere, Gauche)
            var av = Avant; var d = Droite; var ar = Arriere; var g = Gauche;
            if (horaire) { Avant = d; Droite = ar; Arriere = g; Gauche = av; }
            else { Avant = g; Gauche = ar; Arriere = d; Droite = av; }

            Mouvement mouvement;
            if (horaire) mouvement = Mouvement.U;
            else mouvement = Mouvement.UPrime;

            EnregistrerEtat(mouvement);
        }

        // --- ROTATION AXE Z (Faces F et B) ---
        public void AppliquerRotationZ(bool horaire, Mouvement mvt)
        {
            int tempX = X, tempY = Y;
            X = horaire ? tempY : -tempY;
            Y = horaire ? -tempX : tempX;

            // Permutation des 4 faces impactées (Haut, Droite, Bas, Gauche)
            var h = Haut; var d = Droite; var b = Bas; var g = Gauche;
            if (horaire) { Haut = g; Gauche = b; Bas = d; Droite = h; }
            else { Haut = d; Droite = b; Bas = g; Gauche = h; }

            Mouvement mouvement;
            if (horaire) mouvement = Mouvement.F;
            else mouvement = Mouvement.FPrime;

            EnregistrerEtat(mouvement);
        }


        /// <summary>
        /// Enregistre une capture de l'état actuel suite à un mouvement.
        /// </summary>
        internal void EnregistrerEtat(Mouvement mvt)
        {
            Historique.Add(new CubeletSnapshot(
                X, Y, Z,
                Haut, Bas, Gauche, Droite, Avant, Arriere,
                mvt
            ));
        }

    }
}
