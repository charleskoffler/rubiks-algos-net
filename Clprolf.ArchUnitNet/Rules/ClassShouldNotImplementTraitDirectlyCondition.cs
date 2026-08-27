using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent.Conditions;
using Clprolf.ArchUnitNet.Attributes;

namespace Clprolf.ArchUnitNet.Rules
{
    [ClAgent]
    internal sealed class ClassShouldNotImplementTraitDirectlyCondition : ICondition<Class>
    {
        public string Description => "avoid direct implementation of [ClTrait]";

        public IEnumerable<ConditionResult> Check(
            IEnumerable<Class> objects,
            Architecture architecture)
        {
            foreach (var clazz in objects)
            {
                // 1. Global exclusion cases for the class
                if (clazz.IsDraft() || (!clazz.IsAgent() && !clazz.IsWorker() && !clazz.IsSystem()))
                {
                    yield return new ConditionResult(
                        clazz,
                        true,
                        $"{clazz.FullName} is ignored (Draft or neither Agent, Worker, nor System)");
                    continue;
                }

                // 2. If the class has no interfaces, it is automatically valid.
                if (clazz.ImplementedInterfaces.IsNullOrEmpty())
                {
                    yield return new ConditionResult(
                        clazz,
                        true,
                        $"{clazz.FullName} does not implement any interface");
                    continue;
                }

                // 3. Comprehensive validation of all implemented interfaces
                // We ensure that NO interface is a Trait (unless it's a Bypass)
                bool allInterfacesAreValid = clazz.ImplementedInterfaces.All(interf =>
                    !interf.IsTrait() || clazz.HasInterfaceBypass()
                );

                // We retrieve the first Trait that's causing the problem to get a great error message
                var faultyTrait = clazz.ImplementedInterfaces.FirstOrDefault(interf =>
                    interf.IsTrait() && !clazz.HasInterfaceBypass()
                );

                yield return new ConditionResult(
                    clazz,
                    allInterfacesAreValid,
                    allInterfacesAreValid
                        ? $"{clazz.FullName} correctly avoids direct implementation of [ClTrait]."
                        : $"{clazz.FullName} directly implements trait {faultyTrait?.FullName}; prefer family_interface -> trait_interface");
            }
        }

        public bool CheckEmpty() => true;
    }
}