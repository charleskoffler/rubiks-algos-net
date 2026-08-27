using Raylib_cs;
using System.Numerics;

using RubiksAlgosNet.Agents.Impl;
using RubiksAlgosNet.Enums;

// RubiksAlgoNet Author Charles Koffler 27/8/2026
// M.I.T License

var cube = new RubiksCube();
//Mouvement[] sequence = [Mouvement.R, Mouvement.U, Mouvement.RPrime, Mouvement.UPrime];
//cube.ExecuterSequence(sequence);
cube.Voir();
//cube.VoirTravailParMvt();
//cube.VoirTravailPieces();
cube.VoirMvtPiecesSimplifies();