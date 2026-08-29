using Raylib_cs;
using System.Numerics;

using RubiksAlgosNet.Agents.Impl;
using RubiksAlgosNet.Enums;
using RubiksAlgos.Agents.Impl;
using RubiksAlgos.Enums;
using ArchUnitNET.Domain.Extensions;

// RubiksAlgoNet Author Charles Koffler 27/8/2026
// M.I.T License

var cube = new RubiksCube();
Mouvement[] sequence = [Mouvement.x, Mouvement.R, Mouvement.U, Mouvement.RPrime, Mouvement.UPrime];
//cube.ExecuterSequence(sequence);
cube.Voir();
//cube.VoirTravailParMvt();
//cube.VoirTravailPieces();
cube.VoirMvtPiecesSimplifies();

//Console.WriteLine(OrientationReducerHelper.ObtenirOrientation(new List<Mouvement> { Mouvement.y2, Mouvement.z}));
//MovementTranslatorHelper.TraduireToute(new[] { Mouvement.R, Mouvement.L, Mouvement.U, Mouvement.D, Mouvement.F, Mouvement.B }, OrientationRoot.Y2).ForEach(m => Console.Write(m + " "));
//MovementTranslatorHelper.TraduireToute(new[] { Mouvement.R, Mouvement.L, Mouvement.U, Mouvement.D, Mouvement.F, Mouvement.B }, OrientationRoot.X2).ForEach(m => Console.Write(m + " "));

