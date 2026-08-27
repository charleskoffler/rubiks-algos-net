using Clprolf.ArchUnitNet.Attributes;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Text;
using RubiksAlgosNet.Agents;
using RubiksAlgosNet.Enums;
using RubiksAlgosNet.Workers;
using RubiksAlgosNet.Workers.Impl;
using static RubiksAlgosNet.Agents.ICubelet;

namespace RubiksAlgosNet.Agents.Impl
{
    [ClDomain]
    public class RubiksCube : RubiksCubeHelper, IRubiksCube
    {
        private readonly ICubeInfra worker;
        private readonly ICubeConsole consoleWorker;

        public List<Cubelet> Cubelets { get; } = new List<Cubelet>();

        public List<Mouvement> HistoriqueMouvements { get; } = new();

        public RubiksCube()
        {
            this.worker = new Cube3DInfra(this);
            this.consoleWorker = new CubeConsole(this);

            InitialiserCube();
        }

        public void Voir()
        {
            worker.AfficherCube();
        }

        public void VoirTravailPieces()
        {
            consoleWorker.AfficherHistoriqueDetaillee();
        }

        public void VoirTravailParMvt()
        {
            consoleWorker.AfficherHistoriqueParCoups();
        }

        public void VoirMvtPiecesSimplifies()
        {
            consoleWorker.AfficherMouvementsSimplifiesTousLesCubelets();
        }

        public void Executer(Mouvement mvt)
        {
            // 1. On garde la trace du mouvement global
            HistoriqueMouvements.Add(mvt);

            switch (mvt)
            {
                case Mouvement.R: Tourner_R(false); break;
                case Mouvement.RPrime: Tourner_R(true); break;
                case Mouvement.L: Tourner_L(false); break;
                case Mouvement.LPrime: Tourner_L(true); break;
                case Mouvement.F: Tourner_F(false); break;
                case Mouvement.FPrime: Tourner_F(true); break;
                case Mouvement.B: Tourner_B(false); break;
                case Mouvement.BPrime: Tourner_B(true); break;
                case Mouvement.U: Tourner_U(false); break;
                case Mouvement.UPrime: Tourner_U(true); break;
                case Mouvement.D: Tourner_D(false); break;
                case Mouvement.DPrime: Tourner_D(true); break;
                default: throw new NotSupportedException("Mouvement non supporté");
            }
        }

        public void ExecuterSequence(IEnumerable<Mouvement> sequence)
        {
            foreach (var mvt in sequence)
            {
                Executer(mvt);
            }
        }

        // Méthodes privées ou internal

        private void ExecuterRotation(Func<Cubelet, bool> filtre, Action<Cubelet> actionRotation)
        {
            foreach (var c in Cubelets.Where(filtre))
            {
                actionRotation(c);
            }
        }

        private void InitialiserCube()
        {

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        var c = new Cubelet(x, y, z);

                        // Peinture initiale des faces externes
                        if (y == 1) c.Haut = Couleur.W;
                        if (y == -1) c.Bas = Couleur.Y;
                        if (x == -1) c.Gauche = Couleur.O;
                        if (x == 1) c.Droite = Couleur.R;
                        if (z == 1) c.Avant = Couleur.G;
                        if (z == -1) c.Arriere = Couleur.B;

                        Cubelets.Add(c);
                        c.EnregistrerEtat(Mouvement.INIT);
                    }
                }
            }
            HistoriqueMouvements.Add(Mouvement.INIT);
        }

        // --- AXE X : R (Right: X=1) & L (Left: X=-1) ---
        internal void Tourner_R(bool prime = false) => ExecuterRotation(c => c.X == 1, c => c.AppliquerRotationX(!prime, prime ? Mouvement.RPrime : Mouvement.R));
        internal void Tourner_L(bool prime = false) => ExecuterRotation(c => c.X == -1, c => c.AppliquerRotationX(prime, prime ? Mouvement.LPrime : Mouvement.L));

        // --- AXE Y : U (Up: Y=1) & D (Down: Y=-1) ---
        internal void Tourner_U(bool prime = false) => ExecuterRotation(c => c.Y == 1, c => c.AppliquerRotationY(!prime, prime ? Mouvement.UPrime : Mouvement.U));
        internal void Tourner_D(bool prime = false) => ExecuterRotation(c => c.Y == -1, c => c.AppliquerRotationY(prime, prime ? Mouvement.DPrime : Mouvement.D));

        // --- AXE Z : F (Front: Z=1) & B (Back: Z=-1) ---
        internal void Tourner_F(bool prime = false) => ExecuterRotation(c => c.Z == 1, c => c.AppliquerRotationZ(!prime, prime ? Mouvement.FPrime : Mouvement.F));
        internal void Tourner_B(bool prime = false) => ExecuterRotation(c => c.Z == -1, c => c.AppliquerRotationZ(prime, prime ? Mouvement.BPrime : Mouvement.B));

    }
}