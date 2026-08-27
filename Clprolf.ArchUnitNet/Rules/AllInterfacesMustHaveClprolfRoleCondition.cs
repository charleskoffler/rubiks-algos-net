using ArchUnitNET.Domain;
using ArchUnitNET.Fluent.Conditions;
using Clprolf.ArchUnitNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clprolf.ArchUnitNet.Rules
{
    [ClAgent]
    internal sealed class AllInterfacesMustHaveClprolfRoleCondition : ICondition<Interface>
    {
        public string Description => "declare a Clprolf interface role";

        public IEnumerable<ConditionResult> Check(
            IEnumerable<Interface> objects,
            Architecture architecture)
        {
            foreach (var interf in objects)
            {
                bool hasRole =
                    interf.IsFamily()
                    || interf.IsTrait()
                    || interf.IsFree();

                yield return new ConditionResult(
                    interf,
                    hasRole,
                    $"{interf.FullName} should declare a Clprolf interface role: [ClFamily], [ClTrait], or [ClFree]");
            }
        }

        public bool CheckEmpty() => true;
    }
}
