using ArchUnitNET.Domain;
using ArchUnitNET.Fluent.Conditions;
using Clprolf.ArchUnitNet.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace Clprolf.ArchUnitNet.Rules;

[ClAgent]
internal sealed class InheritingInterfaceRoleMustMatchTraitInterfaceTargetRoleCondition : ICondition<Interface>
{
    public string Description => "inherit only trait interfaces with compatible target roles";

    public IEnumerable<ConditionResult> Check(IEnumerable<Interface> objects, Architecture architecture)
    {
        foreach (var interf in objects)
        {
            // 1. Global exclusion cases for the current interface
            if (!interf.IsFamily() && !interf.IsTrait())
            {
                yield return new ConditionResult(
                    interf,
                    true,
                    $"{interf.FullName} is ignored (Neither Family nor Trait interface)");
                continue;
            }

            // Only parents that are [ClTrait] are extracted.
            var parentTraitInterfaces = interf.ImplementedInterfaces.Where(p => p.IsTrait()).ToList();

            // 2. If she does not inherit any Traits, she is automatically valid
            if (!parentTraitInterfaces.Any())
            {
                yield return new ConditionResult(
                    interf,
                    true,
                    $"{interf.FullName} does not inherit from any [ClTrait] interface");
                continue;
            }

            // 3. Global validation: ALL parent interfaces of type Trait must be compatible
            bool allTraitsAreCompatible = parentTraitInterfaces.All(parent =>
                interf.HasBypass()
                || (interf.IsAgent() && parent.IsAgent())
                || (interf.IsWorker() && parent.IsWorker())
                || (interf.IsSystem() && parent.IsSystem())
            );

            // We target the first parent responsible for the error report
            var faultyTrait = parentTraitInterfaces.FirstOrDefault(parent => !(
                interf.HasBypass()
                || (interf.IsAgent() && parent.IsAgent())
                || (interf.IsWorker() && parent.IsWorker())
                || (interf.IsSystem() && parent.IsSystem())
            ));

            yield return new ConditionResult(
                interf,
                allTraitsAreCompatible,
                allTraitsAreCompatible
                    ? $"All inherited trait interfaces are compatible with {interf.FullName}."
                    : $"{interf.FullName} cannot inherit trait interface {faultyTrait?.FullName} because their target roles are incompatible");
        }
    }

    public bool CheckEmpty() => true;
}