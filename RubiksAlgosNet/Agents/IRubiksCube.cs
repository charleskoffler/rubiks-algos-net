using RubiksAlgosNet.Agents.Impl;
using RubiksAlgosNet.Enums;

namespace RubiksAlgosNet.Agents
{
   
    public interface IRubiksCube
    {
       
        List<Cubelet> Cubelets { get; }
        // Historique de tous les mouvements demandés au cube dans l'ordre chronologique
        List<Mouvement> HistoriqueMouvements { get; }

        void Executer(Mouvement mvt);
        void ExecuterSequence(IEnumerable<Mouvement> sequence);

        void Voir();
        void VoirTravailPieces();
        void VoirTravailParMvt();
        void VoirMvtPiecesSimplifies();
    }
}