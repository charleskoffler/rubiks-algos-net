using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent.Conditions;
using Clprolf.ArchUnitNet.Attributes;


namespace Clprolf.ArchUnitNet.Rules;

[ClAgent]
internal sealed class TraitInterfaceRoleMustMatchDirectImplementationCondition : ICondition<Class>
{
    public string Description => "implement only matching [ClTrait] role";

    public IEnumerable<ConditionResult> Check(IEnumerable<Class> objects, Architecture architecture)
    {
        foreach (var clazz in objects)
        {
            // 1. Global exclusion cases for the class
            if (clazz.IsDraft())
            {
                yield return new ConditionResult(
                    clazz,
                    true,
                    $"{clazz.FullName} is a draft");
                continue;
            }

            if (!clazz.IsAgent() && !clazz.IsWorker() && !clazz.IsSystem())
            {
                yield return new ConditionResult(
                    clazz,
                    true,
                    $"{clazz.FullName} is not an agent, worker, or system");
                continue;
            }

            if (clazz.ImplementedInterfaces.IsNullOrEmpty())
            {
                yield return new ConditionResult(
                    clazz,
                    true,
                    $"{clazz.FullName} has not implemented interfaces");
                continue;
            }

            // Only interfaces that are [ClTrait] are extracted.
            var traitInterfaces = clazz.ImplementedInterfaces.Where(i => i.IsTrait()).ToList();

            // 2. If the class does not implement any traits, it is automatically valid.
            if (!traitInterfaces.Any())
            {
                yield return new ConditionResult(
                    clazz,
                    true,
                    $"{clazz.FullName} does not implement any [ClTrait] interface");
                continue;
            }

            // 3. Overall validation: ALL Trait-type interfaces must be compatible
            bool allTraitsAreCompatible = traitInterfaces.All(interf =>
                clazz.HasBypass()
                || (clazz.IsAgent() && interf.IsAgent())
                || (clazz.IsWorker() && interf.IsWorker())
                || (clazz.IsSystem() && interf.IsSystem())
            );

            // We isolate the first faulty line for the error report
            var faultyTrait = traitInterfaces.FirstOrDefault(interf => !(
                clazz.HasBypass()
                || (clazz.IsAgent() && interf.IsAgent())
                || (clazz.IsWorker() && interf.IsWorker())
                || (clazz.IsSystem() && interf.IsSystem())
            ));

            yield return new ConditionResult(
                clazz,
                allTraitsAreCompatible,
                allTraitsAreCompatible
                    ? $"All implemented trait interfaces are compatible with {clazz.FullName}."
                    : $"{clazz.FullName} implements {faultyTrait?.FullName} but class role and [ClTrait] target role do not match");
        }
    }

    public bool CheckEmpty() => true;
}