using ArchUnitNET.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clprolf.ArchUnitNet
{
    public static class InterfaceExtensions
    {
        public static IEnumerable<Interface> GetDirectlyInheritedInterfaces(this Interface interfaceType)
        {
            var allInterfaces = interfaceType.GetDirectlyInheritedInterfaces().ToList();

            var inheritedInterfaces = allInterfaces
                .SelectMany(i => i.GetDirectlyInheritedInterfaces())
                .ToHashSet();

            return allInterfaces
                .Where(i => !inheritedInterfaces.Contains(i))
                .Cast<Interface>();
        }
    }
}
