using Clprolf.ArchUnitNet.Attributes;
using RubiksAlgos.Enums;
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
            if (EstRotationGlobaleOuMetaMvt(mvt)) return mvt;

            // 1. Décomposition (sans cas 2)
            (MvtVisuel face, Modifier mod) = Decomposer(mvt);

            // 2. Identification de la face réelle
            MvtReel faceReelle = ObtenirFaceReelle(face, orientation);

            // 3. Recomposition du mouvement absolu
            return Recomposer(faceReelle, mod);
        }
        public static bool EstRotationGlobaleOuMetaMvt(Mouvement m) =>
           m == Mouvement.x || m == Mouvement.xPrime || m == Mouvement.x2 ||
           m == Mouvement.y || m == Mouvement.yPrime || m == Mouvement.y2 ||
           m == Mouvement.z || m == Mouvement.zPrime || m == Mouvement.z2 ||
           m == Mouvement.M || m == Mouvement.MPrime || m == Mouvement.r || m == Mouvement.rPrime;

        private enum MvtVisuel { U, D, R, L, F, B }
        private enum MvtReel { U, D, R, L, F, B }

        private static (MvtVisuel, Modifier) Decomposer(Mouvement mvt) => mvt switch
        {
            Mouvement.U => (MvtVisuel.U, Modifier.Normal),
            Mouvement.UPrime => (MvtVisuel.U, Modifier.Prime),

            Mouvement.D => (MvtVisuel.D, Modifier.Normal),
            Mouvement.DPrime => (MvtVisuel.D, Modifier.Prime),

            Mouvement.R => (MvtVisuel.R, Modifier.Normal),
            Mouvement.RPrime => (MvtVisuel.R, Modifier.Prime),

            Mouvement.L => (MvtVisuel.L, Modifier.Normal),
            Mouvement.LPrime => (MvtVisuel.L, Modifier.Prime),

            Mouvement.F => (MvtVisuel.F, Modifier.Normal),
            Mouvement.FPrime => (MvtVisuel.F, Modifier.Prime),

            Mouvement.B => (MvtVisuel.B, Modifier.Normal),
            Mouvement.BPrime => (MvtVisuel.B, Modifier.Prime),

            _ => throw new ArgumentOutOfRangeException(nameof(mvt))
        };

        private static MvtReel ObtenirFaceReelle(MvtVisuel v, OrientationRoot o)
        {
            (MvtReel haut, MvtReel avant) = o switch
            {
                OrientationRoot.INIT => (MvtReel.U, MvtReel.F),
                OrientationRoot.Y => (MvtReel.U, MvtReel.R),
                OrientationRoot.Y2 => (MvtReel.U, MvtReel.B),
                OrientationRoot.Y3 => (MvtReel.U, MvtReel.L),

                OrientationRoot.X => (MvtReel.F, MvtReel.D),
                OrientationRoot.X_Y => (MvtReel.F, MvtReel.R),
                OrientationRoot.X_Y2 => (MvtReel.F, MvtReel.U),
                OrientationRoot.X_Y3 => (MvtReel.F, MvtReel.L),

                OrientationRoot.X2 => (MvtReel.D, MvtReel.B),
                OrientationRoot.X2_Y => (MvtReel.D, MvtReel.R),
                OrientationRoot.X2_Y2 => (MvtReel.D, MvtReel.F),
                OrientationRoot.X2_Y3 => (MvtReel.D, MvtReel.L),

                OrientationRoot.X3 => (MvtReel.B, MvtReel.U),
                OrientationRoot.X3_Y => (MvtReel.B, MvtReel.R),
                OrientationRoot.X3_Y2 => (MvtReel.B, MvtReel.D),
                OrientationRoot.X3_Y3 => (MvtReel.B, MvtReel.L),

                OrientationRoot.Z => (MvtReel.L, MvtReel.F),
                OrientationRoot.Z_Y => (MvtReel.L, MvtReel.D),
                OrientationRoot.Z_Y2 => (MvtReel.L, MvtReel.B),
                OrientationRoot.Z_Y3 => (MvtReel.L, MvtReel.U),

                OrientationRoot.Z3 => (MvtReel.R, MvtReel.F),
                OrientationRoot.Z3_Y => (MvtReel.R, MvtReel.U),
                OrientationRoot.Z3_Y2 => (MvtReel.R, MvtReel.B),
                OrientationRoot.Z3_Y3 => (MvtReel.R, MvtReel.D),

                _ => (MvtReel.U, MvtReel.F)
            };

            MvtReel bas = Oppose(haut);
            MvtReel arriere = Oppose(avant);
            MvtReel droite = DeduireMvtReelDroit(haut, avant);
            MvtReel gauche = Oppose(droite);

            return v switch
            {
                MvtVisuel.U => haut,
                MvtVisuel.D => bas,
                MvtVisuel.F => avant,
                MvtVisuel.B => arriere,
                MvtVisuel.R => droite,
                MvtVisuel.L => gauche,
                _ => throw new ArgumentOutOfRangeException(nameof(v))
            };
        }

        private static MvtReel Oppose(MvtReel f) => f switch
        {
            MvtReel.U => MvtReel.D,
            MvtReel.D => MvtReel.U,
            MvtReel.F => MvtReel.B,
            MvtReel.B => MvtReel.F,
            MvtReel.R => MvtReel.L,
            MvtReel.L => MvtReel.R,
            _ => f
        };

        private static MvtReel DeduireMvtReelDroit(MvtReel reelPourVisuelHaut, MvtReel reelPourVisuelAvt) => (reelPourVisuelHaut, reelPourVisuelAvt) switch
        {
            (MvtReel.U, MvtReel.F) => MvtReel.R,
            (MvtReel.U, MvtReel.R) => MvtReel.B,
            (MvtReel.U, MvtReel.B) => MvtReel.L,
            (MvtReel.U, MvtReel.L) => MvtReel.F,

            (MvtReel.F, MvtReel.D) => MvtReel.R,
            (MvtReel.F, MvtReel.R) => MvtReel.U,
            (MvtReel.F, MvtReel.U) => MvtReel.L,
            (MvtReel.F, MvtReel.L) => MvtReel.D,

            (MvtReel.D, MvtReel.B) => MvtReel.R,
            (MvtReel.D, MvtReel.R) => MvtReel.F,
            (MvtReel.D, MvtReel.F) => MvtReel.L,
            (MvtReel.D, MvtReel.L) => MvtReel.B,

            (MvtReel.B, MvtReel.U) => MvtReel.R,
            (MvtReel.B, MvtReel.R) => MvtReel.D,
            (MvtReel.B, MvtReel.D) => MvtReel.L,
            (MvtReel.B, MvtReel.L) => MvtReel.U,

            (MvtReel.L, MvtReel.F) => MvtReel.U,
            (MvtReel.L, MvtReel.D) => MvtReel.F,
            (MvtReel.L, MvtReel.B) => MvtReel.D,
            (MvtReel.L, MvtReel.U) => MvtReel.B,

            (MvtReel.R, MvtReel.F) => MvtReel.D,
            (MvtReel.R, MvtReel.U) => MvtReel.F,
            (MvtReel.R, MvtReel.B) => MvtReel.U,
            (MvtReel.R, MvtReel.D) => MvtReel.B,

            _ => MvtReel.R
        };

        private static Mouvement Recomposer(MvtReel face, Modifier mod) => (face, mod) switch
        {
            (MvtReel.U, Modifier.Normal) => Mouvement.U,
            (MvtReel.U, Modifier.Prime) => Mouvement.UPrime,

            (MvtReel.D, Modifier.Normal) => Mouvement.D,
            (MvtReel.D, Modifier.Prime) => Mouvement.DPrime,

            (MvtReel.R, Modifier.Normal) => Mouvement.R,
            (MvtReel.R, Modifier.Prime) => Mouvement.RPrime,

            (MvtReel.L, Modifier.Normal) => Mouvement.L,
            (MvtReel.L, Modifier.Prime) => Mouvement.LPrime,

            (MvtReel.F, Modifier.Normal) => Mouvement.F,
            (MvtReel.F, Modifier.Prime) => Mouvement.FPrime,

            (MvtReel.B, Modifier.Normal) => Mouvement.B,
            (MvtReel.B, Modifier.Prime) => Mouvement.BPrime,

            _ => throw new ArgumentOutOfRangeException()
        };

    }
}
