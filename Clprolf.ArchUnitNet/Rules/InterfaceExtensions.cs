using ArchUnitNET.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clprolf.ArchUnitNet.Rules
{
    public static class InterfaceExtensions
    {
        public static IEnumerable<Interface> GetDirectlyInheritedInterfaces(this IType interfaceType)
        {
            var allInterfaces = interfaceType.ImplementedInterfaces.ToList();

            var inheritedInterfaces = allInterfaces
                .SelectMany(i => i.ImplementedInterfaces)
                .ToHashSet();

            return allInterfaces
                .Where(i => !inheritedInterfaces.Contains(i))
                .Cast<Interface>();
        }
    }
}
