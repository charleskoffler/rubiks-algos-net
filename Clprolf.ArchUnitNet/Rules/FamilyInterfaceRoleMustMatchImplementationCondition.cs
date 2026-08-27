using ArchUnitNET.Domain;
using ArchUnitNET.Fluent.Conditions;
using Clprolf.ArchUnitNet.Attributes;
using Clprolf.ArchUnitNet.Rules;
using System.Collections.Generic;
using System.Linq;

namespace Clprolf.ArchUnitNet.Rules;

[ClAgent]
internal sealed class FamilyInterfaceRoleMustMatchImplementationCondition : ICondition<Class>
{
    public string Description => "implement only matching [ClFamily] role";

    public IEnumerable<ConditionResult> Check(IEnumerable<Class> objects, Architecture architecture)
    {
        foreach (var clazz in objects)
        {
           
            if (clazz.IsDraft() || (!clazz.IsAgent() && !clazz.IsWorker() && !clazz.IsSystem()))
            {
                yield return new ConditionResult(
                    clazz,
                    true,
                    $"{clazz.FullName} is ignored (Draft or neither Agent, Worker, nor System)");
                continue;
            }

            var familyInterfaces = clazz.ImplementedInterfaces.Where(i => i.IsFamily()).ToList();

            if (!familyInterfaces.Any())
            {
                yield return new ConditionResult(
                    clazz,
                    true,
                    $"{clazz.FullName} does not implement any [ClFamily] interface");
                continue;
            }

            bool allInterfacesAreValid = familyInterfaces.All(interf =>
                clazz.HasBypass()
                || (clazz.IsAgent() && interf.IsAgent())
                || (clazz.IsWorker() && interf.IsWorker())
                || (clazz.IsSystem() && interf.IsSystem())
            );

            // 3. Identifying the faulty interface for the error message
            var faultyInterface = familyInterfaces.FirstOrDefault(interf => !(
                clazz.HasBypass()
                || (clazz.IsAgent() && interf.IsAgent())
                || (clazz.IsWorker() && interf.IsWorker())
                || (clazz.IsSystem() && interf.IsSystem())
            ));

            yield return new ConditionResult(
                clazz,
                allInterfacesAreValid,
                allInterfacesAreValid
                    ? $"Matching roles between {clazz.FullName} and all its family interfaces."
                    : $"{clazz.FullName} implements {faultyInterface?.FullName} but class role and [ClFamily] target role do not match"
            );
        }
    }

    public bool CheckEmpty() => true;
}