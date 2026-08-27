using System;
using System.Collections.Generic;
using System.Text;

namespace RubiksAlgos.Enums
{
    public enum OrientationRoot
{
    // --- Groupe 1 : Face Haut d'origine (Blanche) en HAUT ---
    INIT = 0,     // Identité (aucun mouvement)
    Y,            // 90° à droite
    Y2,           // 180°
    Y3,           // 270° (ou 90° à gauche)

    // --- Groupe 2 : Face Bas d'origine (Jaune) en HAUT ---
    X2,           // Tête en bas
    X2_Y,
    X2_Y2,
    X2_Y3,

    // --- Groupe 3 : Face Avant d'origine (Bleue) en HAUT ---
    X,            // Basculé vers l'arrière
    X_Y,
    X_Y2,
    X_Y3,

    // --- Groupe 4 : Face Arrière d'origine (Verte) en HAUT ---
    X3,           // Basculé vers l'avant (ou X')
    X3_Y,
    X3_Y2,
    X3_Y3,

    // --- Groupe 5 : Face Droite d'origine (Rouge) en HAUT ---
    Z3,           // Penché sur la gauche (ou Z')
    Z3_Y,
    Z3_Y2,
    Z3_Y3,

    // --- Groupe 6 : Face Gauche d'origine (Orange) en HAUT ---
    Z,            // Penché sur la droite
    Z_Y,
    Z_Y2,
    Z_Y3
}
}
