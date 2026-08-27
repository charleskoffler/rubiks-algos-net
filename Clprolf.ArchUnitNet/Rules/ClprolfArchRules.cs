using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Fluent.Conditions;
using ArchUnitNET.Fluent.Syntax.Elements.Types;
using ArchUnitNET.Fluent.Syntax.Elements.Types.Classes;
using Clprolf.ArchUnitNet.Attributes;
using System.Text.RegularExpressions;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Clprolf.ArchUnitNet.Rules;

[ClAgent]
public static class ClprolfArchRules
{
    internal static string NamespaceAndChildren(string rootNamespace)
    {
        return $"{Regex.Escape(rootNamespace)}(\\..*)?";
    }

    public static IArchRule ClprolfClassesMustNotMixAgentAndWorker(string targetNamespace) {
        return Classes()
            .That().ResideInNamespaceMatching(NamespaceAndChildren(targetNamespace)).Should()
            .FollowCustomCondition(new ClassesMustNotMixAgentAndWorkerCondition())
            .WithoutRequiringPositiveResults();
    }

    public static IArchRule AgentWorkerInheritanceMustNotMix(string targetNamespace)
    {
        return Classes()
            .That().ResideInNamespaceMatching(NamespaceAndChildren(targetNamespace))
            .Should()
            .FollowCustomCondition(new AgentWorkerInheritanceMustNotMixCondition())
            .WithoutRequiringPositiveResults();
    }

    public static IArchRule FamilyInterfaceRoleMustMatchImplementation(string targetNamespace)
    {
        return Classes()
            .That().ResideInNamespaceMatching(NamespaceAndChildren(targetNamespace))
            .Should()
            .FollowCustomCondition(new FamilyInterfaceRoleMustMatchImplementationCondition())
            .WithoutRequiringPositiveResults(); // Mandatory, unless the case is valid (otherwise, an “no matching items” error will be displayed)
    }

    public static IArchRule TraitInterfacesMustExtendOnlyTraitInterfaces(string targetNamespace)
    {
       return Interfaces()
        .That()
        .ResideInNamespaceMatching(NamespaceAndChildren(targetNamespace))
        .And()
        .HaveAnyAttributes(typeof(ClTraitAttribute))
        .Should()
        .FollowCustomCondition(new TraitInterfacesMustExtendOnlyTraitInterfacesCondition())
        .WithoutRequiringPositiveResults();
    }

    public static IArchRule ClprolfInterfacesMustHaveTargetRole(string targetNamespace)
    {
        return Interfaces()
        .That()
        .ResideInNamespaceMatching(NamespaceAndChildren(targetNamespace))
        .Should()
        .FollowCustomCondition(new ClprolfInterfacesMustHaveTargetRoleCondition())
        .WithoutRequiringPositiveResults();
    }

    public static IArchRule InheritingInterfaceRoleMustMatchTraitInterfaceTargetRole(string targetNamespace)
    {
       return Interfaces()
        .That()
        .ResideInNamespaceMatching(NamespaceAndChildren(targetNamespace))
        .Should()
        .FollowCustomCondition(new InheritingInterfaceRoleMustMatchTraitInterfaceTargetRoleCondition())
        .WithoutRequiringPositiveResults();
    }

    public static IArchRule FamilyInterfaceTargetRoleMustMatchInheritedFamilyInterface(string targetNamespace)
    {
        return Interfaces()
            .That()
            .ResideInNamespaceMatching(NamespaceAndChildren(targetNamespace))
            .And()
            .HaveAnyAttributes(typeof(ClFamilyAttribute))
            .Should()
            .FollowCustomCondition(new FamilyInterfaceTargetRoleMustMatchInheritedFamilyInterfaceCondition())
            .WithoutRequiringPositiveResults();
    }

    public static IArchRule TraitInterfaceRoleMustMatchDirectImplementation(string targetNamespace)
    {
       return Classes()
            .That()
            .ResideInNamespaceMatching(NamespaceAndChildren(targetNamespace))
            .Should()
            .FollowCustomCondition(new TraitInterfaceRoleMustMatchDirectImplementationCondition())
            .WithoutRequiringPositiveResults();
    }
}