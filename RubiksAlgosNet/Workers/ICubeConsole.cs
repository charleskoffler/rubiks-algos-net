using Clprolf.ArchUnitNet.Attributes;
using RubiksAlgosNet.Agents.Impl;

namespace RubiksAlgosNet.Workers.Impl
{
    [ClInfrastructure]
    [ClFamily]
    internal interface ICubeConsole: ICubeInfra
    {
        void AfficherFaceArriere();
        void AfficherFaceAvant();
        void AfficherFaceDroite();
        void AfficherFaceGauche();
        void AfficherHistoriqueDetaillee();
        void AfficherHistoriqueParCoups();
        public void AfficherMouvementsSimplifiesTousLesCubelets();
    }
}