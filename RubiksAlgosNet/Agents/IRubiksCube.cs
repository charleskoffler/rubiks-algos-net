using Clprolf.ArchUnitNet.Attributes;
using RubiksAlgos.Agents.Impl;
using RubiksAlgos.Enums;

namespace RubiksAlgos.Agents
{
    [ClDomain]
    [ClFamily]
    public interface IRubiksCube
    {
       
        List<Cubelet> Cubelets { get; }
        // Historique de tous les mouvements demandés au cube dans l'ordre chronologique
        List<Mouvement> HistoriqueMouvementsRelatifs { get; }
        List<Mouvement> HistoriqueMouvementsReels { get; }
        List<Mouvement> ListeOrientations { get; }

        void Executer(Mouvement mvt, bool svgderHisto = true);
        void ExecuterSequence(IEnumerable<Mouvement> sequence);

        void Voir();
        void VoirTravailPieces();
        void VoirTravailParMvt();
        void VoirMvtPiecesSimplifies();
    }
}