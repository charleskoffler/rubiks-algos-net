using Clprolf.ArchUnitNet.Attributes;
using RubiksAlgos.Enums;
using RubiksAlgosNet.Enums;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RubiksAlgos.Agents
{
    [ClAgent]
    [ClFamily]
    public interface IOrientationReducerHelper
    {
        static abstract OrientationRoot ObtenirOrientation(IEnumerable<Mouvement> mouvements);
        static abstract Quaternion ObtenirRotation(OrientationRoot orientation);
    }
}
