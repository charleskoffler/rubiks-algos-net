using Clprolf.ArchUnitNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace RubiksAlgosNet.Enums;

[ClDraft]
public static class MouvementExtensions
{
    public static string ToNotation(this Mouvement mvt)
    {
        return mvt switch
        {
            Mouvement.RPrime => "R'",
            Mouvement.LPrime => "L'",
            Mouvement.UPrime => "U'",
            Mouvement.DPrime => "D'",
            Mouvement.FPrime => "F'",
            Mouvement.BPrime => "B'",

            Mouvement.rPrime => "r'",
            Mouvement.lPrime => "l'",
            Mouvement.uPrime => "u'",
            Mouvement.dPrime => "d'",
            Mouvement.fPrime => "f'",
            Mouvement.bPrime => "b'",

            Mouvement.INIT => "INIT",

            // Pour tous les autres (R, L, U, r, l, etc.), le nom de l'enum convient déjà !
            _ => mvt.ToString()
        };
    }
}