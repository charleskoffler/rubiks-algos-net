using Clprolf.ArchUnitNet.Attributes;
using RubiksAlgos.Enums;
using RubiksAlgosNet.Enums;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RubiksAlgos.Agents.Impl
{
    [ClAgent]
    public class OrientationReducer: IOrientationReducer
    {
     // Exemples y' x = Z_Y3 ; x y = X_Y ; z' y2 = Z3_Y2 ; y2 z = Z3_Y2

        private OrientationReducer() { }

        // Représentation des 6 faces du cube par leur position initiale (0 à 5)
        // 0: Haut, 1: Bas, 2: Avant, 3: Arrière, 4: Droite, 5: Gauche
        private enum Face { Haut = 0, Bas = 1, Avant = 2, Arriere = 3, Droite = 4, Gauche = 5 }

        public static OrientationRoot ObtenirOrientation(IEnumerable<Mouvement> mouvements)
        {
            Face haut = Face.Haut;
            Face avant = Face.Avant;
            Face droite = Face.Droite;

            foreach (var mvt in mouvements)
            {
                AppliquerMouvementLocal(ref haut, ref avant, ref droite, mvt);
            }

            return DeterminerOrientationRoot(haut, avant);
        }

        // Méthodes privées

        private static void AppliquerMouvementLocal(ref Face haut, ref Face avant, ref Face droite, Mouvement mvt)
        {
            Face tmpHaut = haut;
            Face tmpAvant = avant;
            Face tmpDroite = droite;

            Face bas = Opposite(tmpHaut);
            Face arriere = Opposite(tmpAvant);
            Face gauche = Opposite(tmpDroite);

            switch (mvt)
            {
                // --- Mouvement X (Bascule vers l'arrière) ---
                // Le Haut va à l'Arrière, l'Avant va au Haut, etc.
                case Mouvement.x:
                    haut = tmpAvant;
                    avant = bas;
                    break;
                case Mouvement.xPrime:
                    haut = arriere;
                    avant = tmpHaut;
                    break;
                case Mouvement.x2:
                    haut = bas;
                    avant = arriere;
                    break;

                // --- Mouvement Y (Pivot à droite vu du dessus) ---
                // Le Haut ne bouge pas, l'Avant va à la Gauche, la Droite va à l'Avant
                case Mouvement.y:
                    avant = tmpDroite;
                    droite = arriere;
                    break;
                case Mouvement.yPrime:
                    avant = gauche;
                    droite = tmpAvant;
                    break;
                case Mouvement.y2:
                    avant = arriere;
                    droite = gauche;
                    break;

                // --- Mouvement Z (Penche vers la droite) ---
                // L'Avant ne bouge pas, la Gauche va au Haut, le Haut va à la Droite
                case Mouvement.z:
                    haut = gauche;
                    droite = tmpHaut;
                    break;
                case Mouvement.zPrime:
                    haut = tmpDroite;
                    droite = bas;
                    break;
                case Mouvement.z2:
                    haut = bas;
                    droite = gauche;
                    break;
            }
        }

        private static Face Opposite(Face f) => f switch
        {
            Face.Haut => Face.Bas,
            Face.Bas => Face.Haut,
            Face.Avant => Face.Arriere,
            Face.Arriere => Face.Avant,
            Face.Droite => Face.Gauche,
            Face.Gauche => Face.Droite,
            _ => f
        };

        private static OrientationRoot DeterminerOrientationRoot(Face haut, Face avant)
        {
            return (haut, avant) switch
            {
                // --- Groupes HAUT (Face Haut d'origine au plafond) ---
                (Face.Haut, Face.Avant) => OrientationRoot.INIT,
                (Face.Haut, Face.Droite) => OrientationRoot.Y,
                (Face.Haut, Face.Arriere) => OrientationRoot.Y2,
                (Face.Haut, Face.Gauche) => OrientationRoot.Y3,

                // --- Groupes BAS (Face Bas d'origine au plafond) ---
                (Face.Bas, Face.Arriere) => OrientationRoot.X2,
                (Face.Bas, Face.Droite) => OrientationRoot.X2_Y,
                (Face.Bas, Face.Avant) => OrientationRoot.X2_Y2,
                (Face.Bas, Face.Gauche) => OrientationRoot.X2_Y3,

                // --- Groupes AVANT (Face Avant d'origine au plafond) ---
                (Face.Avant, Face.Bas) => OrientationRoot.X,
                (Face.Avant, Face.Droite) => OrientationRoot.X_Y,
                (Face.Avant, Face.Haut) => OrientationRoot.X_Y2,
                (Face.Avant, Face.Gauche) => OrientationRoot.X_Y3,

                // --- Groupes ARRIÈRE (Face Arrière d'origine au plafond) ---
                (Face.Arriere, Face.Haut) => OrientationRoot.X3,
                (Face.Arriere, Face.Droite) => OrientationRoot.X3_Y,
                (Face.Arriere, Face.Bas) => OrientationRoot.X3_Y2,
                (Face.Arriere, Face.Gauche) => OrientationRoot.X3_Y3,

                // --- Groupes DROITE (Face Droite d'origine au plafond) ---
                (Face.Droite, Face.Avant) => OrientationRoot.Z3,
                (Face.Droite, Face.Haut) => OrientationRoot.Z3_Y,
                (Face.Droite, Face.Arriere) => OrientationRoot.Z3_Y2,
                (Face.Droite, Face.Bas) => OrientationRoot.Z3_Y3,

                // --- Groupes GAUCHE (Face Gauche d'origine au plafond) ---
                (Face.Gauche, Face.Avant) => OrientationRoot.Z,
                (Face.Gauche, Face.Haut) => OrientationRoot.Z_Y,
                (Face.Gauche, Face.Arriere) => OrientationRoot.Z_Y2,
                (Face.Gauche, Face.Bas) => OrientationRoot.Z_Y3, // <-- Fixé : (Gauche, Bas) est Z_Y3

                _ => throw new InvalidOperationException($"Combinaison impossible : Haut={haut}, Avant={avant}")
            };
        }
    }
}
