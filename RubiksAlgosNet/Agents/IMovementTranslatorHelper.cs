using Clprolf.ArchUnitNet.Attributes;
using RubiksAlgos.Enums;
using RubiksAlgosNet.Enums;

namespace RubiksAlgos.Agents
{
    [ClAgent]
    public interface IMovementTranslatorHelper
    {
        /// <summary>
        /// Exemple: Y2: R -> L , U -> U, F -> B, D -> D, L -> R, B -> F
        /// X2: R -> R, L -> L, U -> D, D -> U, F -> B, B -> F
        /// </summary>
        /// <param name="mvt"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        static abstract Mouvement Traduire(Mouvement mvt, OrientationRoot orientation);
        static abstract IEnumerable<Mouvement> TraduireToute(IEnumerable<Mouvement> mouvements, OrientationRoot orientation);
    }
}