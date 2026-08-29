using System;
using System.Collections.Generic;
using System.Text;

namespace RubiksAlgosNet.Enums
{
    public enum Mouvement : int
    {
        INIT = 999, // Représente l'état initial (avant tout mouvement)

        R = 0, RPrime = 1,
        L = 2, LPrime = 3,
        U = 4, UPrime = 5,
        D = 6, DPrime = 7,
        F = 8, FPrime = 9,
        B = 10, BPrime = 11,
        r = -100, rPrime = -101,
        l = -102, lPrime = -103,
        u = -104, uPrime = -105,
        d = -106, dPrime = -107,
        f = -108, fPrime = -109,
        b = -110, bPrime = -111,
        M = 12, MPrime = 13,
        x = 18,
        xPrime = 19,
        y = 20,
        yPrime = 21,
        z = 22,
        zPrime = 23,
        x2 = 24,
        y2 = 25,
        z2 = 26
    }  
}
