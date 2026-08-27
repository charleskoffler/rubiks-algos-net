using ArchUnitNET.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clprolf.ArchUnitNet.Rules;

public static class ClassPredicatesExtensions
{
    /// <summary>
    /// Renvoie uniquement les interfaces directement déclarées par la classe,
    /// en excluant celles héritées par transitivité via d'autres interfaces.
    /// </summary>
    public static IEnumerable<Interface> GetDirectlyImplementedInterfaces(this Class clazz)
    {
        var allInterfaces = clazz.ImplementedInterfaces.ToList();

        var inheritedInterfaces = allInterfaces
            .SelectMany(i => i.ImplementedInterfaces)
            .ToHashSet();

        // Utilisation de Cast<Interface>() pour convertir proprement la séquence
        return allInterfaces
            .Where(i => !inheritedInterfaces.Contains(i))
            .Cast<Interface>();
    }
}
