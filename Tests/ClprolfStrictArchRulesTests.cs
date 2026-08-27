using ArchUnitNET.xUnit;
using Clprolf.ArchUnitNet.Attributes;
using Clprolf.ArchUnitNet.Rules;

using static Clprolf.ArchUnitNet.Tests.ClprolfArchRulesTests;

namespace Clprolf.ArchUnitNet.Tests;

[ClAgent]
public sealed class ClprolfStrictArchRulesTests
{
    [Fact]
    public void AllClassesShouldHaveClprolfRole()
    {
        ClprolfStrictArchRules.AllClassesShouldHaveClprolfRole(TargetNamespace)
            .Check(TestArchitecture.Architecture);
    }

    [Fact]
    public void AllInterfacesShouldHaveClprolfRole()
    {
        ClprolfStrictArchRules.AllInterfacesShouldHaveClprolfRole(TargetNamespace)
            .Check(TestArchitecture.Architecture);
    }

    [Fact]
    public void ClassShouldNotImplementTraitDirectly()
    {
        ClprolfStrictArchRules.ClassShouldNotImplementTraitDirectly(TargetNamespace)
            .Check(TestArchitecture.Architecture);
    }

    [Fact]
    public void ClassMustImplementOnlyOneFamilyInterface()
    {
        ClprolfStrictArchRules.ClassMustImplementOnlyOneFamilyInterface(TargetNamespace)
            .Check(TestArchitecture.Architecture);
    }
}