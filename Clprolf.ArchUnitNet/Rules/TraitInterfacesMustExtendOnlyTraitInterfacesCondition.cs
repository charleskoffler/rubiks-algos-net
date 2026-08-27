using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent.Conditions;
using Clprolf.ArchUnitNet.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace Clprolf.ArchUnitNet.Rules;

[ClAgent]
internal sealed class TraitInterfacesMustExtendOnlyTraitInterfacesCondition : ICondition<Interface>
{
    public string Description => "extend only trait interfaces";

    public IEnumerable<ConditionResult> Check(IEnumerable<Interface> objects, Architecture architecture)
    {
        foreach (var interf in objects)
        {
            if (!interf.IsTrait())
            {
                yield return new ConditionResult(
                    interf,
                    true,
                    $"{interf.FullName} is ignored (Not a [ClTrait] interface)");
                continue;
            }

            if (interf.ImplementedInterfaces.IsNullOrEmpty())
            {
                yield return new ConditionResult(
                    interf,
                    true,
                    $"{interf.FullName} has no parent interface");
                continue;
            }

            bool allParentsAreTraitsOrExternal = interf.ImplementedInterfaces.All(parent =>
                interf.HasInterfaceBypass()
                || parent.IsTrait()
                || !parent.IsClprolf()
            );

            var faultyParent = interf.ImplementedInterfaces.FirstOrDefault(parent =>
                !interf.HasInterfaceBypass()
                && !parent.IsTrait()
                && parent.IsClprolf()
            );

            yield return new ConditionResult(
                interf,
                allParentsAreTraitsOrExternal,
                allParentsAreTraitsOrExternal
                    ? $"All parent interfaces of {interf.FullName} are traits or external third-party interfaces."
                    : $"{interf.FullName} is a trait interface but it inherits from {faultyParent?.FullName} which is NEITHER a trait interface NOR an external system interface");
        }
    }

    public bool CheckEmpty() => true;
}