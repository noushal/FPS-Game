using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Multiplayer.Tools.Editor;
using UnityEditor;
using UnityEditor.Build;
using Tool = Unity.Multiplayer.Tools.Editor.Tool;

namespace Unity.Multiplayer.Tools.Tests.Editor
{
    [TestFixture]
    [Explicit("Running tests with this fixture modifies compile flags and can trigger a recompile, " +
        "so should only be run when needed. " +
        "They are also pretty simple so don't need to be run at a all times.")]
    class BuildSettingsTests
    {
        readonly Dictionary<NamedBuildTarget, string[]> m_BuildSettingsPerTarget
            = new Dictionary<NamedBuildTarget, string[]>();

        [OneTimeSetUp]
        public void SaveScriptingDefineSymbols()
        {
            m_BuildSettingsPerTarget.Clear();
            foreach (var target in BuildSettings.k_AllBuildTargets)
            {
                PlayerSettings.GetScriptingDefineSymbols(target, out string[] symbols);
                m_BuildSettingsPerTarget[target] = symbols;
            }
        }

        [OneTimeTearDown]
        public void RestoreScriptingDefineSymbols()
        {
            foreach (var (target, symbols) in m_BuildSettingsPerTarget)
            {
                PlayerSettings.SetScriptingDefineSymbols(target, symbols);
            }
        }

        [Test(Description = "This test is there to make sure that we have the correct amount of tests and testing all build symbols"
            + " for every tools.")]
        public void When_TestingToolsBuildSettings_HasCorrectCount()
        {
            const int ExpectedToolCount = 2;
            var actualToolCount = Enum.GetNames(typeof(Tool)).Length;
            Assert.AreEqual(ExpectedToolCount, actualToolCount);
        }

        [Test]
        public void When_ToolIsEnabled_ItIsEnabled()
        {
            foreach (var symbol in BuildTestsConstants.k_AllBuildSymbols)
            {
                foreach (var tool in BuildTestsConstants.k_AllTools)
                {
                    BuildSettings.AddSymbolToAllBuildTargets(
                        tool, symbol);
                    Assert.IsTrue(BuildSettings.GetEnabledForAnyBuildTargets(
                        tool, symbol));
                    Assert.IsTrue(BuildSettings.GetSymbolInAllBuildTargets(
                        tool, symbol));
                }
            }
        }

        [Test]
        public void When_ToolIsDisabled_ItIsDisabled()
        {
            foreach (var symbol in BuildTestsConstants.k_AllBuildSymbols)
            {
                foreach (var tool in BuildTestsConstants.k_AllTools)
                {
                    BuildSettings.RemoveSymbolFromAllBuildTargets(
                        tool, symbol);
                    Assert.IsFalse(BuildSettings.GetEnabledForAnyBuildTargets(
                        tool, symbol));
                    Assert.IsFalse(BuildSettings.GetSymbolInAllBuildTargets(
                        tool, symbol));
                }
            }
        }

        [Test]
        public void When_ToolIsEnabledThenDisabled_ItIsDisabled()
        {
            foreach (var symbol in BuildTestsConstants.k_AllBuildSymbols)
            {
                foreach (var tool in BuildTestsConstants.k_AllTools)
                {
                    BuildSettings.AddSymbolToAllBuildTargets(
                        tool, symbol);
                    BuildSettings.RemoveSymbolFromAllBuildTargets(
                        tool, symbol);

                    Assert.IsFalse(BuildSettings.GetEnabledForAnyBuildTargets(
                        tool, symbol));
                    Assert.IsFalse(BuildSettings.GetSymbolInAllBuildTargets(
                        tool, symbol));
                }
            }
        }

        [Test]
        public void When_ToolIsEnabledThenDisabledThenEnabled_ItIsEnabled()
        {
            foreach (var symbol in BuildTestsConstants.k_AllBuildSymbols)
            {
                foreach (var tool in BuildTestsConstants.k_AllTools)
                {
                    BuildSettings.AddSymbolToAllBuildTargets(
                        tool, symbol);
                    BuildSettings.RemoveSymbolFromAllBuildTargets(
                        tool, symbol);
                    BuildSettings.AddSymbolToAllBuildTargets(
                        tool, symbol);

                    Assert.IsTrue(BuildSettings.GetEnabledForAnyBuildTargets(
                        tool, symbol));
                    Assert.IsTrue(BuildSettings.GetSymbolInAllBuildTargets(
                        tool, symbol));
                }
            }
        }
    }
}
