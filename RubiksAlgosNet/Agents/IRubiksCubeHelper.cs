using Clprolf.ArchUnitNet.Attributes;
using System.Numerics;
using RubiksAlgosNet.Enums;

namespace RubiksAlgosNet.Agents
{
    [ClAgent]
    [ClFamily]
    public interface IRubiksCubeHelper
    {
        static abstract List<Mouvement> SimplifierMouvements(IEnumerable<Mouvement> mouvements);
    }
}