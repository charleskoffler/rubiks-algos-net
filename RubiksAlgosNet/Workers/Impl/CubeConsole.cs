using Clprolf.ArchUnitNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using RubiksAlgosNet.Agents.Impl;
using RubiksAlgosNet.Enums;
using static RubiksAlgosNet.Agents.ICubelet;
using static RubiksAlgosNet.Agents.IRubiksCube;

namespace RubiksAlgosNet.Workers.Impl
{
    [ClInfrastructure]
    internal class CubeConsole : ICubeConsole
    {
        private readonly RubiksCube cube;
        public CubeConsole(RubiksCube cube) { this.cube = cube; }
        public void AfficherCube()
        {
            AfficherFaceAvant();
            AfficherFaceDroite();
            // Vous pouvez ajouter d'autres méthodes pour afficher les autres faces si nécessaire
        }
        public void AfficherFaceAvant()
        {
            Console.WriteLine("--- FACE AVANT (G) ---");
            for (int y = 1; y >= -1; y--) // Du haut vers le bas
            {
                for (int x = -1; x <= 1; x++) // De la gauche vers la droite
                {
                    var piece = cube.Cubelets.First(c => c.Z == 1 && c.X == x && c.Y == y);
                    Console.Write($"[{piece.Id}][{piece.Avant}] ");
                }
                Console.WriteLine();
            }
        }

        public void AfficherFaceArriere()
        {
            Console.WriteLine("--- FACE ARRIERE (B) ---");
            for (int y = 1; y >= -1; y--) // Du haut vers le bas
            {
                for (int x = -1; x <= 1; x++) // De la gauche vers la droite
                {
                    var piece = cube.Cubelets.First(c => c.Z == -1 && c.X == x && c.Y == y);
                    Console.Write($"[{piece.Id}][{piece.Arriere}] ");
                }
                Console.WriteLine();
            }
        }

        public void AfficherFaceDroite()
        {
            Console.WriteLine("--- FACE DROITE (R) ---");
            for (int y = 1; y >= -1; y--) // Du haut vers le bas
            {
                for (int z = -1; z <= 1; z++) // De l'arrière vers l'avant
                {
                    var piece = cube.Cubelets.First(c => c.X == 1 && c.Y == y && c.Z == z);
                    Console.Write($"[{piece.Id}][{piece.Droite}] ");
                }
                Console.WriteLine();
            }
        }

        public void AfficherFaceGauche()
        {
            Console.WriteLine("--- FACE GAUCHE (O) ---");
            for (int y = 1; y >= -1; y--) // Du haut vers le bas
            {
                for (int z = -1; z <= 1; z++) // De l'arrière vers l'avant
                {
                    var piece = cube.Cubelets.First(c => c.X == -1 && c.Y == y && c.Z == z);
                    Console.Write($"[{piece.Id}][{piece.Gauche}] ");
                }
                Console.WriteLine();
            }
        }
        public void AfficherHistoriqueDetaillee()
        {
            foreach (var piece in cube.Cubelets)
            {
                if (!piece.Historique.Any()) continue;

                Console.WriteLine($"=== HISTORIQUE DE LA PIÈCE [{piece.Id}] ===");

                foreach (var snapshot in piece.Historique)
                {
                    var facesVisibles = GetFacesVisibles(snapshot);
                    string orientation = string.Join(" ", facesVisibles.Select(f => $"{f.Nom}:{f.Couleur}"));

                    Console.WriteLine(
                        $"  ➜ Mouvement {snapshot.MouvementSubiReel,-7} : Pos ({snapshot.X,2}, {snapshot.Y,2}, {snapshot.Z,2}) | " +
                        $"Faces [{orientation}]"
                    );
                }

                Console.WriteLine();
            }
        }

        public void AfficherHistoriqueParCoups()
        {
            for (int i = 0; i < cube.HistoriqueMouvementsReels.Count; i++)
            {
                var mvtActuel = cube.HistoriqueMouvementsReels[i];

                Console.WriteLine($"--- Coup n°{i} : {mvtActuel} ---");

                // On cherche le snapshot enregistré pour ce mouvement précis
                var piecesImpactees = cube.Cubelets
                    .SelectMany(p => p.Historique, (piece, snap) => new { Piece = piece, Snap = snap })
                    .Where(x => x.Snap.MouvementSubiReel == mvtActuel);

                foreach (var item in piecesImpactees)
                {
                    var s = item.Snap;
                    var facesVisibles = GetFacesVisibles(s);
                    string orientation = string.Join(" ", facesVisibles.Select(f => $"{f.Nom}:{f.Couleur}"));

                    Console.WriteLine($"  • Piece [{item.Piece.Id,-3}] -> Pos: ({s.X,2}, {s.Y,2}, {s.Z,2}) | Orientation: [{orientation}]");
                }

                Console.WriteLine();
            }
        }

        public void AfficherMouvementsSimplifiesTousLesCubelets()
        {
            string sequence = HistoriqueAffichable(false);
            Console.WriteLine($"Mouvements joués ({cube.HistoriqueMouvementsRelatifs.Count-1}) : {sequence}");
            sequence = HistoriqueAffichable();
            Console.WriteLine($"Mouvements réels ({cube.HistoriqueMouvementsReels.Count - 1}) : {sequence}");

            Console.WriteLine("=== MOUVEMENTS ET ORIENTATIONS PAR PIECES AFFECTEES ===");

            bool auMoinsUnMouvement = false;

            foreach (var piece in cube.Cubelets)
            {
                var mouvementsSimplifies = RubiksCubeHelper.SimplifierMouvements(piece.Historique.Select(s => s.MouvementSubiReel));

                if (mouvementsSimplifies.Count == 0) continue;

                auMoinsUnMouvement = true;

                string listeFormatted = string.Join(" ", mouvementsSimplifies);

                var premierSnapshot = piece.Historique.First();
                var dernierSnapshot = piece.Historique.Last(); // La simplification ne peut pas changer l'état final.

                var facesVisiblesPremierSnapshot = GetFacesVisibles(premierSnapshot);
                string orientationInitiale = string.Join(" ", facesVisiblesPremierSnapshot.Select(f => $"{f.Nom}:{f.Couleur}"));

                var facesVisiblesDernierSnapshot = GetFacesVisibles(dernierSnapshot);
                string orientationFinale = string.Join(" ", facesVisiblesDernierSnapshot.Select(f => $"{f.Nom}:{f.Couleur}"));

                Console.WriteLine($"[Pièce {piece.Id,-4}] : {listeFormatted,-15} | Init: {orientationInitiale} - Finale: {orientationFinale}");
            }

            if (!auMoinsUnMouvement)
            {
                Console.WriteLine("Aucune pièce n'a subi de mouvement net.");
            }

            Console.WriteLine("=================================================");
        }

        // méthodes privées

        protected IEnumerable<(string Nom, Couleur Couleur)> GetFacesVisibles(CubeletSnapshot s)
        {
            return new[]
            {
        (Nom: "Ht",    Couleur: s.Haut),
        (Nom: "Bs",     Couleur: s.Bas),
        (Nom: "Gc",  Couleur: s.Gauche),
        (Nom: "Dr",  Couleur: s.Droite),
        (Nom: "Av",   Couleur: s.Avant),
        (Nom: "Ar", Couleur: s.Arriere)
    }.Where(f => f.Couleur != Couleur.X);
        }

        private string HistoriqueAffichable(bool reels = true)
        {
            var historique = reels ? cube.HistoriqueMouvementsReels : cube.HistoriqueMouvementsRelatifs;

            if (historique.Count < 2)
            {
                return "Historique vide.";
            }

            // Transforme chaque mouvement en sa représentation texte (ex: U, R', F2)
            string sequence = string.Join(" ", historique.Where(m => m != Mouvement.INIT).Select(m => m.ToNotation()));

            return sequence;
        }
    }
}
