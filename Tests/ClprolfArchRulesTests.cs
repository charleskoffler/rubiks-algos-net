using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Clprolf.ArchUnitNet.Attributes;
using Clprolf.ArchUnitNet.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clprolf.ArchUnitNet.Tests
{
    [ClAgent]
    public class ClprolfArchRulesTests
    {
        public static string TargetNamespace = "RubiksAlgosNet";

        [Fact]
        public void ClprolfClassesMustNotMixAgentAndWorker()
        {
            ClprolfArchRules.ClprolfClassesMustNotMixAgentAndWorker(TargetNamespace)
                .Check(TestArchitecture.Architecture);
        }
        [Fact]
        public void AgentWorkerInheritanceMustNotMix()
        {
            ClprolfArchRules.AgentWorkerInheritanceMustNotMix(TargetNamespace)
                .Check(TestArchitecture.Architecture);
        }

        [Fact]
        public void FamilyInterfaceRoleMustMatchImplementation()
        {
            ClprolfArchRules.FamilyInterfaceRoleMustMatchImplementation(TargetNamespace)
                .Check(TestArchitecture.Architecture);
        }
        [Fact]
        public void TraitInterfacesMustExtendOnlyTraitInterfaces()
        {
            ClprolfArchRules.TraitInterfacesMustExtendOnlyTraitInterfaces(TargetNamespace)
                .Check(TestArchitecture.Architecture);
        }
        [Fact]
        public void ClprolfInterfacesMustHaveTargetRole()
        {
            ClprolfArchRules.ClprolfInterfacesMustHaveTargetRole(TargetNamespace)
                .Check(TestArchitecture.Architecture);
        }

        [Fact]
        public void InheritingInterfaceRoleMustMatchTraitInterfaceTargetRole()
        {
            ClprolfArchRules.InheritingInterfaceRoleMustMatchTraitInterfaceTargetRole(TargetNamespace).Check(TestArchitecture.Architecture);
        }

        [Fact]
        public void FamilyInterfaceTargetRoleMustMatchInheritedFamilyInterface()
        {
            ClprolfArchRules.FamilyInterfaceTargetRoleMustMatchInheritedFamilyInterface(TargetNamespace)
                .Check(TestArchitecture.Architecture);
        }

        [Fact]
        public void TraitInterfaceRoleMustMatchDirectImplementation()
        {
            ClprolfArchRules.TraitInterfaceRoleMustMatchDirectImplementation(TargetNamespace)
                 .Check(TestArchitecture.Architecture);
        }
    }
}
