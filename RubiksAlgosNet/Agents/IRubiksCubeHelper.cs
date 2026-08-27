using Clprolf.ArchUnitNet.Attributes;
using System.Numerics;
using RubiksAlgosNet.Enums;

namespace RubiksAlgosNet.Agents
{
    [ClAgent]
    public interface IRubiksCubeHelper
    {
        static abstract List<Mouvement> SimplifierMouvements(IEnumerable<Mouvement> mouvements);
    }
}