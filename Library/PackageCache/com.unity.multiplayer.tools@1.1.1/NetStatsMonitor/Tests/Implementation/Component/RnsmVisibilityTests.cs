using System.Collections;
using NUnit.Framework;
using Unity.Multiplayer.Tools.NetStatsMonitor.Implementation;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Tests
{
    static class RnsmTestExtensions
    {
        internal static UIDocument GetUiDoc(this RuntimeNetStatsMonitor rnsm)
            => rnsm.Implementation?.UiDoc;

        internal static VisualElement GetRootVisualElement(this RuntimeNetStatsMonitor rnsm)
            => rnsm.Implementation?.UiDoc.rootVisualElement;

        internal static RnsmVisualElement GetRnsmVisualElement(this RuntimeNetStatsMonitor rnsm)
            => rnsm.GetRootVisualElement()?.Q<RnsmVisualElement>();

        internal static bool ImplementationExists(this RuntimeNetStatsMonitor rnsm)
            => rnsm.Implementation != null;

        internal static bool UiDocExists(this RuntimeNetStatsMonitor rnsm)
            => rnsm.GetUiDoc() != null;

        internal static bool RootVisualElementExists(this RuntimeNetStatsMonitor rnsm)
            => rnsm.GetRootVisualElement() != null;

        internal static bool RnsmVisualElementExists(this RuntimeNetStatsMonitor rnsm)
            => rnsm.GetRnsmVisualElement() != null;

        internal static bool RnsmVisualElementVisible(this RuntimeNetStatsMonitor rnsm)
            => rnsm.GetRnsmVisualElement()?.visible ?? false;
    }

    class RnsmVisibilityTests
    {
        [Test]
        public void EnsureVisibilityIsAppliedToTheRootVisualElement()
        {
            var go = new GameObject();
            var rnsm = go.AddComponent<RuntimeNetStatsMonitor>();
            rnsm.Setup();

            rnsm.Visible = false;
            TestVisibility(rnsm, false);

            rnsm.Visible = true;
            TestVisibility(rnsm, true);

            rnsm.Visible = false;
            TestVisibility(rnsm, false);

            rnsm.Visible = true;
            TestVisibility(rnsm, true);

            rnsm.Teardown();
            Object.DestroyImmediate(go);
        }

        internal static void TestVisibility(RuntimeNetStatsMonitor rnsm, bool expectedVisibility)
        {
            Assert.IsTrue(rnsm.ImplementationExists(), "rnsm.ImplementationExists()");
            Assert.IsTrue(rnsm.UiDocExists(), "rnsm.UiDocExists()");
            Assert.IsTrue(rnsm.RootVisualElementExists(), "rnsm.RootVisualElementExists()");
            Assert.IsTrue(rnsm.RnsmVisualElementExists(), "rnsm.RnsmVisualElementExists()");
            if (expectedVisibility)
            {
                Assert.IsTrue(rnsm.Visible, "rnsm.Visible");
                Assert.IsTrue(rnsm.RnsmVisualElementVisible(), "rnsm.RnsmVisualElementVisible()");
            }
            else
            {
                Assert.IsFalse(rnsm.Visible, "rnsm.Visible");
                Assert.IsFalse(rnsm.RnsmVisualElementVisible(), "rnsm.RnsmVisualElementVisible()");
            }
        }
    }
}