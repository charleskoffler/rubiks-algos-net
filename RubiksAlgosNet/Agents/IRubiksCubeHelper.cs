using Clprolf.ArchUnitNet.Attributes;
using System.Numerics;
using RubiksAlgos.Enums;

namespace RubiksAlgos.Agents
{
    [ClAgent]
    [ClFamily]
    public interface IRubiksCubeHelper
    {
        static abstract List<Mouvement> SimplifierMouvements(IEnumerable<Mouvement> mouvements);
    }
}