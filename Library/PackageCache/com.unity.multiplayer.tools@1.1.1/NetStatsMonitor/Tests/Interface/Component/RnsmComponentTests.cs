using NUnit.Framework;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Tests.Interface
{
    class RnsmComponentTests
    {
        [Test]
        public void ClampRefreshRateWhenBelowMinimum()
        {
            var rnsm = RnsmHelper.CreateRnsm();
            rnsm.MaxRefreshRate = -1;

            Assert.AreEqual(ConfigurationLimits.k_RefreshRateMin, rnsm.MaxRefreshRate);
        }

        [Test]
        public void NoClampingRefreshRateWhenExtremelyHighValue()
        {
            const double k_RefreshRateValue = 1000000;
            var rnsm = RnsmHelper.CreateRnsm();
            rnsm.MaxRefreshRate = k_RefreshRateValue;

            Assert.AreEqual(k_RefreshRateValue, rnsm.MaxRefreshRate);
        }
    }
}
