using ArchUnitNET.Domain;
using ArchUnitNET.Fluent.Conditions;
using Clprolf.ArchUnitNet.Attributes;
using System.Collections.Generic;

namespace Clprolf.ArchUnitNet.Rules;

[ClAgent]
internal sealed class ClprolfInterfacesMustHaveTargetRoleCondition : ICondition<Interface>
{
    public string Description => "have a valid target role [ClAgent], [ClWorker] and/or [ClSystem]";

    public IEnumerable<ConditionResult> Check(IEnumerable<Interface> objects, Architecture architecture)
    {
        foreach (var interf in objects)
        {
            bool isFamily = interf.IsFamily();
            bool isTrait = interf.IsTrait();

            if (!isFamily && !isTrait)
            {
                yield return new ConditionResult(
                interf,
                true,
                $"{interf.FullName} is not a family, nor a trait");
                continue;
            }

            bool hasAgent = interf.IsAgent();
            bool hasWorker = interf.IsWorker();
            bool hasSystem = interf.IsSystem();

            bool ok;
            if (isFamily)
            {
                // For a Family: EXACTLY one target role activated (3-input XOR)
                int targetRoleCount = 0;
                if (hasAgent) targetRoleCount++;
                if (hasWorker) targetRoleCount++;
                if (hasSystem) targetRoleCount++;

                ok = targetRoleCount == 1;
            }
            else
            {
                // For a Trait: AT LEAST one activated target role
                ok = hasAgent || hasWorker || hasSystem;
            }

            yield return new ConditionResult(
                interf,
                ok,
                $"{interf.FullName} must have a valid target role: [ClFamily] requires exactly one of [ClAgent], [ClWorker], or [ClSystem]; [ClTrait] requires at least one of [ClAgent], [ClWorker], or [ClSystem]");
        }
    }

    public bool CheckEmpty() => true;
}