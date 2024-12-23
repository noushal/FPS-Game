using NUnit.Framework;
using Unity.Multiplayer.Tools.NetStatsMonitor.Implementation;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Tests.Stats
{
    class StatHistoryRequirementsTests
    {
        [Test]
        public void ExponentialMovingAverageParamsSetDoesNotContainDuplicates()
        {
            var requirements = new StatHistoryRequirements();
            var decayConstants = requirements.DecayConstants;

            decayConstants.Add(0.1);
            decayConstants.Add(0.1);
            decayConstants.Add(0.1);
            decayConstants.Add(0.25);
            decayConstants.Add(0.25);
            decayConstants.Add(0.25);
            decayConstants.Add(0.33);
            decayConstants.Add(0.33);
            decayConstants.Add(0.5);
            decayConstants.Add(0.6874);
            decayConstants.Add(0.6874);
            decayConstants.Add(0.687434);
            decayConstants.Add(0.687434);
            decayConstants.Add(0.687434);
            decayConstants.Add(0.68743453786895);
            decayConstants.Add(0.68743453786895);
            decayConstants.Add(0.68743453786895);
            decayConstants.Add(0.68743453786895);

            Assert.AreEqual(decayConstants.Count, 7);
        }
    }
}