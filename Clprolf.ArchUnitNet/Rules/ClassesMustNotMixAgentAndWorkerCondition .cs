using ArchUnitNET.Domain;
using ArchUnitNET.Fluent.Conditions;
using Clprolf.ArchUnitNet.Attributes;

namespace Clprolf.ArchUnitNet.Rules;

[ClAgent]
internal sealed class ClassesMustNotMixAgentAndWorkerCondition : ICondition<Class>
{
    public string Description => "not mix Clprolf roles ([ClAgent], [ClWorker], [ClSystem])";

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

            // How many roles has this class activated at the same time?
            int roleCount = 0;
            if (clazz.IsAgent()) roleCount++;
            if (clazz.IsWorker()) roleCount++;
            if (clazz.IsSystem()) roleCount++;

            // The rule is followed if and only if only one role is active
            bool ok = roleCount == 1;

            yield return new ConditionResult(
                clazz,
                ok,
                ok ? "Class roles are distinct"
                   : $"{clazz.FullName} cannot mix Clprolf roles, it must choose exactly one between [ClAgent], [ClWorker], and [ClSystem]");
        }
    }

    public bool CheckEmpty() => true;
}