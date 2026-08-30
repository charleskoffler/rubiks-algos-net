using Clprolf.ArchUnitNet.Attributes;
using RubiksAlgos.Enums;

namespace RubiksAlgos.Agents
{
    [ClAgent]
    [ClFamily]
    public interface IMovementTranslatorHelper
    {
        /// <summary>
        /// Ne change pas les mouvements de type rotation globale (X, Y, Z, X2, Y2, Z2, X3, Y3, Z3), ou métas(M, M', E, E', S, S')
        /// Exemple: Y2: R -> L , U -> U, F -> B, D -> D, L -> R, B -> F
        /// X2: R -> R, L -> L, U -> D, D -> U, F -> B, B -> F
        /// </summary>
        /// <param name="mvt"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        static abstract Mouvement Traduire(Mouvement mvt, OrientationRoot orientation);
        static abstract IEnumerable<Mouvement> TraduireToute(IEnumerable<Mouvement> mouvements, OrientationRoot orientation);
        static abstract bool EstRotationGlobaleOuMetaMvt(Mouvement m);
    }
}