using ArchUnitNET.Fluent;
using Clprolf.ArchUnitNet.Rules;
using System.Text.RegularExpressions;
using static ArchUnitNET.Fluent.ArchRuleDefinition;
using static Clprolf.ArchUnitNet.Rules.ClprolfArchRules;

namespace Clprolf.ArchUnitNet.Rules;

public static class ClprolfStrictArchRules
{
    public static IArchRule AllClassesShouldHaveClprolfRole(string targetNamespace)
    {
        return Classes()
            .That()
            .ResideInNamespaceMatching(NamespaceAndChildren(targetNamespace))
            .Should()
            .FollowCustomCondition(new AllClassesMustHaveClprolfRoleCondition())
            .WithoutRequiringPositiveResults();
    }

    public static IArchRule AllInterfacesShouldHaveClprolfRole(string targetNamespace)
    {
        return Interfaces()
            .That()
            .ResideInNamespaceMatching(NamespaceAndChildren(targetNamespace))
            .Should()
            .FollowCustomCondition(new AllInterfacesMustHaveClprolfRoleCondition())
            .WithoutRequiringPositiveResults();
    }

    public static IArchRule ClassShouldNotImplementTraitDirectly(string targetNamespace)
    {
        return Classes()
            .That()
            .ResideInNamespaceMatching(NamespaceAndChildren(targetNamespace))
            .Should()
            .FollowCustomCondition(new ClassShouldNotImplementTraitDirectlyCondition())
            .WithoutRequiringPositiveResults();
    }

    public static IArchRule ClassMustImplementOnlyOneFamilyInterface(string targetNamespace)
    {
        return Classes()
            .That()
            .ResideInNamespaceMatching(NamespaceAndChildren(targetNamespace))
            .Should()
            .FollowCustomCondition(new ClassMustImplementOnlyOneFamilyCondition())
            .WithoutRequiringPositiveResults();
    }
}
