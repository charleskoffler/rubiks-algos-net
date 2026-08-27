using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent.Conditions;
using Clprolf.ArchUnitNet.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace Clprolf.ArchUnitNet.Rules;

[ClAgent]
internal sealed class FamilyInterfaceTargetRoleMustMatchInheritedFamilyInterfaceCondition : ICondition<Interface>
{
    public string Description => "inherit only family interfaces with compatible target roles";

    public IEnumerable<ConditionResult> Check(IEnumerable<Interface> objects, Architecture architecture)
    {
        foreach (var interf in objects)
        {
            // 1. If the interface does not inherit from anything, it is automatically valid
            if (interf.ImplementedInterfaces.IsNullOrEmpty())
            {
                yield return new ConditionResult(
                    interf,
                    true,
                    $"{interf.FullName} has no parent interface");
                continue;
            }

            // Only parent interfaces of type “Family” are extracted.
            var parentFamilyInterfaces = interf.ImplementedInterfaces.Where(p => p.IsFamily()).ToList();

            // 2. If no parent is a Family interface, it is automatically valid
            if (!parentFamilyInterfaces.Any())
            {
                yield return new ConditionResult(
                    interf,
                    true,
                    $"{interf.FullName} does not inherit from any [ClFamily] interface");
                continue;
            }

            // 3. Overall validation: ALL parent Family interfaces must be compatible
            bool allParentsAreCompatible = parentFamilyInterfaces.All(parent =>
                interf.HasBypass()
                || (interf.IsAgent() && parent.IsAgent())
                || (interf.IsWorker() && parent.IsWorker())
                || (interf.IsSystem() && parent.IsSystem())
            );

            // We retrieve the first parent node at fault to generate a great, targeted error message
            var faultyParent = parentFamilyInterfaces.FirstOrDefault(parent => !(
                interf.HasBypass()
                || (interf.IsAgent() && parent.IsAgent())
                || (interf.IsWorker() && parent.IsWorker())
                || (interf.IsSystem() && parent.IsSystem())
            ));

            yield return new ConditionResult(
                interf,
                allParentsAreCompatible,
                allParentsAreCompatible
                    ? $"All inherited family interfaces are compatible with {interf.FullName}."
                    : $"{interf.FullName} cannot inherit family interface {faultyParent?.FullName} because their target roles are incompatible");
        }
    }

    public bool CheckEmpty() => true;
}