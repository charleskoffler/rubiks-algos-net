using Clprolf.ArchUnitNet.Attributes;
using RubiksAlgosNet.Enums;

namespace RubiksAlgosNet.Agents
{
    [ClDomain]
    [ClFamily]
    public interface ICubelet
    {
        public enum Couleur
        {
            X, // Plastique noir / Intérieur
            W, // White (Blanc - Haut)
            Y, // Yellow (Jaune - Bas)
            G, // Green (Vert - Avant)
            B, // Blue (Bleu - Arrière)
            R, // Red (Rouge - Droite)
            O  // Orange (Orange - Gauche)
        }

        /// <summary>
        /// Représente l'état exact d'un Cubelet à un instant T.
        /// </summary>
        public readonly record struct CubeletSnapshot(
            int X, int Y, int Z,
            Couleur Haut,
            Couleur Bas,
            Couleur Gauche,
            Couleur Droite,
            Couleur Avant,
            Couleur Arriere,
            Mouvement MouvementSubiReel
        );

        Couleur Arriere { get; set; }
        Couleur Avant { get; set; }
        Couleur Bas { get; set; }
        Couleur Droite { get; set; }
        Couleur Gauche { get; set; }
        Couleur Haut { get; set; }
        string Id { get; }
        int X { get; set; }
        int Y { get; set; }
        int Z { get; set; }
        /// <summary>
        /// Historique complet de la vie de ce Cubelet.
        /// </summary>
        public List<CubeletSnapshot> Historique { get; }
        public List<Mouvement> ObtenirMouvementsSimplifies();

        void AppliquerRotationX(bool horaire, Mouvement mvt);
        void AppliquerRotationY(bool horaire, Mouvement mvt);
        void AppliquerRotationZ(bool horaire, Mouvement mvt);
    }
}