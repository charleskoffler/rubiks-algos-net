using Clprolf.ArchUnitNet.Attributes;
using RubiksAlgos.Enums;
using RubiksAlgosNet.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RubiksAlgos.Agents.Impl
{
    
    [ClAgent]
    public class MovementTranslatorHelper : IMovementTranslatorHelper
    {
        private enum Modifier { Normal, Prime }
        public static IEnumerable<Mouvement> TraduireToute(IEnumerable<Mouvement> mouvements, OrientationRoot orientation)
    => mouvements.Select(m => Traduire(m, orientation));

        public static Mouvement Traduire(Mouvement mvt, OrientationRoot orientation)
        {
            if (EstRotationGlobale(mvt)) return mvt;

            // 1. Décomposition (sans cas 2)
            (FaceVisuelle face, Modifier mod) = Decomposer(mvt);

            // 2. Identification de la face réelle
            FaceReelle faceReelle = ObtenirFaceReelle(face, orientation);

            // 3. Recomposition du mouvement absolu
            return Recomposer(faceReelle, mod);
        }

        private enum FaceVisuelle { U, D, R, L, F, B }
        private enum FaceReelle { U, D, R, L, F, B }

        private static (FaceVisuelle, Modifier) Decomposer(Mouvement mvt) => mvt switch
        {
            Mouvement.U => (FaceVisuelle.U, Modifier.Normal),
            Mouvement.UPrime => (FaceVisuelle.U, Modifier.Prime),

            Mouvement.D => (FaceVisuelle.D, Modifier.Normal),
            Mouvement.DPrime => (FaceVisuelle.D, Modifier.Prime),

            Mouvement.R => (FaceVisuelle.R, Modifier.Normal),
            Mouvement.RPrime => (FaceVisuelle.R, Modifier.Prime),

            Mouvement.L => (FaceVisuelle.L, Modifier.Normal),
            Mouvement.LPrime => (FaceVisuelle.L, Modifier.Prime),

            Mouvement.F => (FaceVisuelle.F, Modifier.Normal),
            Mouvement.FPrime => (FaceVisuelle.F, Modifier.Prime),

            Mouvement.B => (FaceVisuelle.B, Modifier.Normal),
            Mouvement.BPrime => (FaceVisuelle.B, Modifier.Prime),

            _ => throw new ArgumentOutOfRangeException(nameof(mvt))
        };

        private static FaceReelle ObtenirFaceReelle(FaceVisuelle v, OrientationRoot o)
        {
            (FaceReelle haut, FaceReelle avant) = o switch
            {
                OrientationRoot.INIT => (FaceReelle.U, FaceReelle.F),
                OrientationRoot.Y => (FaceReelle.U, FaceReelle.R),
                OrientationRoot.Y2 => (FaceReelle.U, FaceReelle.B),
                OrientationRoot.Y3 => (FaceReelle.U, FaceReelle.L),

                OrientationRoot.X => (FaceReelle.F, FaceReelle.D),
                OrientationRoot.X_Y => (FaceReelle.F, FaceReelle.R),
                OrientationRoot.X_Y2 => (FaceReelle.F, FaceReelle.U),
                OrientationRoot.X_Y3 => (FaceReelle.F, FaceReelle.L),

                OrientationRoot.X2 => (FaceReelle.D, FaceReelle.B),
                OrientationRoot.X2_Y => (FaceReelle.D, FaceReelle.R),
                OrientationRoot.X2_Y2 => (FaceReelle.D, FaceReelle.F),
                OrientationRoot.X2_Y3 => (FaceReelle.D, FaceReelle.L),

                OrientationRoot.X3 => (FaceReelle.B, FaceReelle.U),
                OrientationRoot.X3_Y => (FaceReelle.B, FaceReelle.R),
                OrientationRoot.X3_Y2 => (FaceReelle.B, FaceReelle.D),
                OrientationRoot.X3_Y3 => (FaceReelle.B, FaceReelle.L),

                OrientationRoot.Z => (FaceReelle.L, FaceReelle.F),
                OrientationRoot.Z_Y => (FaceReelle.L, FaceReelle.D),
                OrientationRoot.Z_Y2 => (FaceReelle.L, FaceReelle.B),
                OrientationRoot.Z_Y3 => (FaceReelle.L, FaceReelle.U),

                OrientationRoot.Z3 => (FaceReelle.R, FaceReelle.F),
                OrientationRoot.Z3_Y => (FaceReelle.R, FaceReelle.U),
                OrientationRoot.Z3_Y2 => (FaceReelle.R, FaceReelle.B),
                OrientationRoot.Z3_Y3 => (FaceReelle.R, FaceReelle.D),

                _ => (FaceReelle.U, FaceReelle.F)
            };

            FaceReelle bas = Oppose(haut);
            FaceReelle arriere = Oppose(avant);
            FaceReelle droite = ProduitCroise(haut, avant);
            FaceReelle gauche = Oppose(droite);

            return v switch
            {
                FaceVisuelle.U => haut,
                FaceVisuelle.D => bas,
                FaceVisuelle.F => avant,
                FaceVisuelle.B => arriere,
                FaceVisuelle.R => droite,
                FaceVisuelle.L => gauche,
                _ => throw new ArgumentOutOfRangeException(nameof(v))
            };
        }

        private static FaceReelle Oppose(FaceReelle f) => f switch
        {
            FaceReelle.U => FaceReelle.D,
            FaceReelle.D => FaceReelle.U,
            FaceReelle.F => FaceReelle.B,
            FaceReelle.B => FaceReelle.F,
            FaceReelle.R => FaceReelle.L,
            FaceReelle.L => FaceReelle.R,
            _ => f
        };

        private static FaceReelle ProduitCroise(FaceReelle haut, FaceReelle avant) => (haut, avant) switch
        {
            (FaceReelle.U, FaceReelle.F) => FaceReelle.R,
            (FaceReelle.U, FaceReelle.R) => FaceReelle.B,
            (FaceReelle.U, FaceReelle.B) => FaceReelle.L,
            (FaceReelle.U, FaceReelle.L) => FaceReelle.F,

            (FaceReelle.F, FaceReelle.D) => FaceReelle.R,
            (FaceReelle.F, FaceReelle.R) => FaceReelle.U,
            (FaceReelle.F, FaceReelle.U) => FaceReelle.L,
            (FaceReelle.F, FaceReelle.L) => FaceReelle.D,

            (FaceReelle.D, FaceReelle.B) => FaceReelle.R,
            (FaceReelle.D, FaceReelle.R) => FaceReelle.F,
            (FaceReelle.D, FaceReelle.F) => FaceReelle.L,
            (FaceReelle.D, FaceReelle.L) => FaceReelle.B,

            (FaceReelle.B, FaceReelle.U) => FaceReelle.R,
            (FaceReelle.B, FaceReelle.R) => FaceReelle.D,
            (FaceReelle.B, FaceReelle.D) => FaceReelle.L,
            (FaceReelle.B, FaceReelle.L) => FaceReelle.U,

            (FaceReelle.L, FaceReelle.F) => FaceReelle.U,
            (FaceReelle.L, FaceReelle.D) => FaceReelle.F,
            (FaceReelle.L, FaceReelle.B) => FaceReelle.D,
            (FaceReelle.L, FaceReelle.U) => FaceReelle.B,

            (FaceReelle.R, FaceReelle.F) => FaceReelle.D,
            (FaceReelle.R, FaceReelle.U) => FaceReelle.F,
            (FaceReelle.R, FaceReelle.B) => FaceReelle.U,
            (FaceReelle.R, FaceReelle.D) => FaceReelle.B,

            _ => FaceReelle.R
        };

        private static Mouvement Recomposer(FaceReelle face, Modifier mod) => (face, mod) switch
        {
            (FaceReelle.U, Modifier.Normal) => Mouvement.U,
            (FaceReelle.U, Modifier.Prime) => Mouvement.UPrime,

            (FaceReelle.D, Modifier.Normal) => Mouvement.D,
            (FaceReelle.D, Modifier.Prime) => Mouvement.DPrime,

            (FaceReelle.R, Modifier.Normal) => Mouvement.R,
            (FaceReelle.R, Modifier.Prime) => Mouvement.RPrime,

            (FaceReelle.L, Modifier.Normal) => Mouvement.L,
            (FaceReelle.L, Modifier.Prime) => Mouvement.LPrime,

            (FaceReelle.F, Modifier.Normal) => Mouvement.F,
            (FaceReelle.F, Modifier.Prime) => Mouvement.FPrime,

            (FaceReelle.B, Modifier.Normal) => Mouvement.B,
            (FaceReelle.B, Modifier.Prime) => Mouvement.BPrime,

            _ => throw new ArgumentOutOfRangeException()
        };

        private static bool EstRotationGlobale(Mouvement m) =>
            m == Mouvement.x || m == Mouvement.xPrime || m == Mouvement.x2 ||
            m == Mouvement.y || m == Mouvement.yPrime || m == Mouvement.y2 ||
            m == Mouvement.z || m == Mouvement.zPrime || m == Mouvement.z2;
    }
}
