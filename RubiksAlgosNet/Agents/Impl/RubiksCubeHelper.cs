using Clprolf.ArchUnitNet.Attributes;
using System;
using System.Collections.Generic;
using System.Numerics;
using RubiksAlgos.Enums;

namespace RubiksAlgos.Agents.Impl
{
    [ClAgent]
    public class RubiksCubeHelper: IRubiksCubeHelper
    {
        private RubiksCubeHelper() { }

        //Méthodes helper public static

        /// <summary>
        /// Simplifie une séquence de mouvements en annulant les coups inverses
        /// et en sautant par-dessus les mouvements sur faces opposées (ex: U' D D U -> D D).
        /// </summary>
        public static List<Mouvement> SimplifierMouvements(IEnumerable<Mouvement> mouvements)
        {
            var resultat = new List<Mouvement>();

            foreach (var mvt in mouvements)
            {
                // On ignore le coup de départ
                if (mvt == Mouvement.INIT) continue;

                bool annule = false;

                // On remonte la liste 'resultat' de la fin vers le début
                for (int i = resultat.Count - 1; i >= 0; i--)
                {
                    var precedent = resultat[i];

                    // CAS 1 : Même face
                    if (MemeFace(precedent, mvt))
                    {
                        if (SontInverses(precedent, mvt))
                        {
                            resultat.RemoveAt(i); // U et U' s'annulent
                            annule = true;
                        }
                        else if (precedent == mvt)
                        {
                            // On vérifie si on a DÉJÀ 2 mouvements identiques consécutifs avant celui-ci (ex: U, U + le nouveau U)
                            if (i > 0 && resultat[i - 1] == mvt)
                            {
                                // 3 fois le même mouvement (U U U) -> devient l'inverse (U') !
                                resultat.RemoveAt(i);     // On retire le 2e U
                                resultat[i - 1] = ObtenirInverse(mvt); // Le 1er U devient U'
                                annule = true;
                            }
                            // Sinon, c'est juste le 2e U (ex: U U) -> on le laisse s'ajouter normalement
                        }

                        break; // On a fini de traiter cette face
                    }

                    // CAS 2 : Face adjacente (ex: F, R, L, B pour la face U)
                    if (!SontOpposees(precedent, mvt))
                    {
                        break; // Blocage : impossible d'annuler à travers une face adjacente !
                    }

                    // CAS 3 : Face opposée (ex: D par rapport à U)
                    // On ne fait rien et la boucle continue (i--) pour sauter par-dessus !
                }

                // Si le mouvement n'a pas annulé un coup précédent, on l'ajoute
                if (!annule)
                {
                    resultat.Add(mvt);
                }
            }

            return resultat;
        }

        // Méthodes privées

        private static bool MemeFace(Mouvement m1, Mouvement m2)
        {
            return ObtenirLettreFace(m1) == ObtenirLettreFace(m2);
        }

        private static bool SontInverses(Mouvement m1, Mouvement m2)
        {
            return MemeFace(m1, m2) && m1 != m2;
        }

        private static bool SontOpposees(Mouvement m1, Mouvement m2)
        {
            char axe1 = ObtenirAxe(m1);
            char axe2 = ObtenirAxe(m2);
            return axe1 == axe2 && !MemeFace(m1, m2);
        }

        private static char ObtenirAxe(Mouvement m) => m switch
        {
            Mouvement.R or Mouvement.RPrime or
            Mouvement.L or Mouvement.LPrime => 'X',

            Mouvement.U or Mouvement.UPrime or
            Mouvement.D or Mouvement.DPrime => 'Y',

            Mouvement.F or Mouvement.FPrime or
            Mouvement.B or Mouvement.BPrime => 'Z',

            _ => throw new NotImplementedException()
        };

        private static char ObtenirLettreFace(Mouvement m) => m.ToString()[0];

        private static Mouvement ObtenirInverse(Mouvement m) => m switch
        {
            Mouvement.R => Mouvement.RPrime,
            Mouvement.RPrime => Mouvement.R,

            Mouvement.L => Mouvement.LPrime,
            Mouvement.LPrime => Mouvement.L,

            Mouvement.U => Mouvement.UPrime,
            Mouvement.UPrime => Mouvement.U,

            Mouvement.D => Mouvement.DPrime,
            Mouvement.DPrime => Mouvement.D,

            Mouvement.F => Mouvement.FPrime,
            Mouvement.FPrime => Mouvement.F,

            Mouvement.B => Mouvement.BPrime,
            Mouvement.BPrime => Mouvement.B,

            _ => m // Si c'est INIT ou inconnu, on ne touche à rien
        };

    }
}