using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using RubiksAlgos;
using RubiksAlgos.Agents.Impl;

namespace Clprolf.ArchUnitNet.Tests
{
    /// <summary>
    /// TODO Please change the typeof in the Architecture, with a type from your project you want to check. 
    /// And write the correct namespace in ClprolfArchRulesTests
    /// </summary>
    public static class TestArchitecture
    {
        public static readonly Architecture Architecture = new ArchLoader()
           // .LoadAssemblies(System.Reflection.Assembly.GetExecutingAssembly())
           .LoadAssemblies(typeof(RubiksCube).Assembly)
            .Build();
    }
}
