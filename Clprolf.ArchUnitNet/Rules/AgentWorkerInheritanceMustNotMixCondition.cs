using ArchUnitNET.Domain;
using ArchUnitNET.Fluent.Conditions;
using Clprolf.ArchUnitNet.Attributes;
using Clprolf.ArchUnitNet.Rules;

namespace Clprolf.ArchUnitNet.Rules;

[ClAgent]
internal sealed class AgentWorkerInheritanceMustNotMixCondition : ICondition<Class>
{
    public string Description => "inherit only from same Clprolf role unless [ClBypass]";

    public IEnumerable<ConditionResult> Check(IEnumerable<Class> objects, Architecture architecture)
    {
        foreach (var clazz in objects)
        {
            // Trying to include all the cases in the IEnumerable<ConditionResult>, even the correct ones,
            // to not have the "no matching element exception".
            if (clazz.IsDraft() || (!clazz.IsAgent() && !clazz.IsWorker() && !clazz.IsSystem()) || clazz.HasBypass())
            {
                yield return new ConditionResult(clazz, true, "Ignored or bypassed");
                continue;
            }

            var parent = clazz.BaseClass;

            if (parent is null || parent.FullName == typeof(object).FullName)
            {
                yield return new ConditionResult(clazz, true, "No valid parent to check");
                continue;
            }

            // Evaluation of the updated inheritance rule for ClSystem
            // Inheritance must strictly preserve the role (Agent->Agent, Worker->Worker, System->System)
            bool ok = clazz.IsAgent() && parent.IsAgent()
                      || clazz.IsWorker() && parent.IsWorker()
                      || clazz.IsSystem() && parent.IsSystem()
                      || parent.IsDraft()
                      || (!parent.IsAgent() && !parent.IsWorker() && !parent.IsSystem()); // External frameworks classes are allowed

            yield return new ConditionResult(
                clazz,
                ok,
                ok ? "Inheritance role preserved"
                   : $"{clazz.FullName} cannot extend {parent.FullName} because Clprolf inheritance must preserve the role");
        }
    }

    public bool CheckEmpty() => true;
}