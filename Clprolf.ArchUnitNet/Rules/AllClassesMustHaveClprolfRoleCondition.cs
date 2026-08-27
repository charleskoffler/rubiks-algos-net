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
    internal sealed class AllClassesMustHaveClprolfRoleCondition : ICondition<Class>
    {
        public string Description => "declare a Clprolf role";

        public IEnumerable<ConditionResult> Check(
            IEnumerable<Class> objects,
            Architecture architecture)
        {
            foreach (var clazz in objects)
            {
               
                bool hasRole =
                    clazz.IsAgent()
                    || clazz.IsWorker()
                    || clazz.IsSystem()
                    || clazz.IsDraft();

                yield return new ConditionResult(
                    clazz,
                    hasRole,
                    $"{clazz.FullName} should declare a Clprolf role: [ClAgent], [ClWorker], [ClSystem], or [ClDraft]");
            }
        }

        public bool CheckEmpty() => true;
    }
}