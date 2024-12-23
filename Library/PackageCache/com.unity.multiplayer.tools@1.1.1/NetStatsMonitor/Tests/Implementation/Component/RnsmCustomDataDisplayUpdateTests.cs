using System.Collections.Generic;
using NUnit.Framework;
using Unity.Multiplayer.Tools.NetStats;
using Unity.Multiplayer.Tools.NetStats.Tests;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Tests
{
    /// These tests are virtually identical to RnsmDisplayUpdateTests only using the custom data API
    /// rather than the TestDataGenerator
    class RnsmCustomDataDisplayUpdateTests
    {
        static readonly List<MetricId> k_CustomStatsDisplayed = new List<MetricId>
        {
            MetricId.Create(TestMetric.BytesCounter),
            MetricId.Create(TestMetric.BytesCounter3),
            MetricId.Create(TestMetric.BytesCounter4),
        };

        [Test]
        public void EnsureRnsmDisplayOnlyUpdatesWhenNewDataIsReceived()
        {
            RnsmTestHarness rnsmTestHarness = new RnsmTestHarness(
                rnsmRefreshRate: 1f,
                statsDisplayed: k_CustomStatsDisplayed);

            // T == 0
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 0f,
                customData: new Dictionary<MetricId, float>
                {
                    // BytesGauge is not configured to be displayed so will not
                    // trigger a display update
                    { MetricId.Create(TestMetric.BytesGauge), 17 },
                },
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.NoDataReceived);

            // T == 1
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 1f,
                customData: null,
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.NoDataReceived);

            // T == 2
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 1f,
                customData: new Dictionary<MetricId, float>
                {
                    // SecondsCounter and BytesCounter2 are not configured to be displayed so will not
                    // trigger a display update
                    { MetricId.Create(TestMetric.BytesCounter2), 87623 },
                    { MetricId.Create(TestMetric.SecondsCounter), 224 },
                },
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.NoDataReceived);

            // T == 3
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 1f,
                customData: new Dictionary<MetricId, float>
                {
                    // BytesCounter is configured to be displayed so will
                    // trigger a display update
                    { MetricId.Create(TestMetric.BytesCounter), 419 },
                    { MetricId.Create(TestMetric.SecondsGauge), 117 },
                },
                updateRnsm: false,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.UpdateNotCalled);

            // T == 4
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 1f,
                customData: null,
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.DataReceivedTimeElapsedUpdateCalled);

            // T == 5
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 1f,
                customData: null,
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.NoDataReceived);

            rnsmTestHarness.Teardown();
        }

        [Test]
        public void EnsureRnsmDisplayOnlyUpdatesWhenEnoughTimeHasElapsed()
        {
            RnsmTestHarness rnsmTestHarness = new RnsmTestHarness(
                rnsmRefreshRate: 1f,
                statsDisplayed: k_CustomStatsDisplayed);

            // T == 0
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 0f,
                customData: null,
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.NoDataReceived);

            // T == 0
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 0f,
                customData: new Dictionary<MetricId, float>
                {
                    // BytesCounter is configured to be displayed so will
                    // trigger a display update
                    { MetricId.Create(TestMetric.BytesCounter), 419 },
                },
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.DataReceivedNoTimeElapsedUpdateCalledFirstTime);

            // T == 0
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 0f,
                customData: new Dictionary<MetricId, float>
                {
                    // BytesCounter3 is configured to be displayed so will
                    // trigger a display update
                    { MetricId.Create(TestMetric.BytesCounter3), 419 },
                },
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.NoTimeElapsed);

            // T == 0.2
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 0.2f,
                customData: new Dictionary<MetricId, float>
                {
                    // BytesCounter4 is configured to be displayed so will
                    // trigger a display update
                    { MetricId.Create(TestMetric.BytesCounter4), 419 },
                },
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.NotEnoughTimeElapsed);

            // T == 0.8
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 0.6f,
                customData: new Dictionary<MetricId, float>
                {
                    // BytesCounter3 is configured to be displayed so will
                    // trigger a display update
                    { MetricId.Create(TestMetric.BytesCounter3), 419 },
                },
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.NotEnoughTimeElapsed);

            // T == 0.95
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 0.15f,
                customData: new Dictionary<MetricId, float>
                {
                    // BytesCounter is configured to be displayed so will
                    // trigger a display update
                    { MetricId.Create(TestMetric.BytesCounter), 419 },
                },
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.NotEnoughTimeElapsed);

            // T == 1.05
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 0.1f,
                customData: new Dictionary<MetricId, float>
                {
                    // BytesCounter is configured to be displayed so will
                    // trigger a display update
                    { MetricId.Create(TestMetric.BytesCounter), 419 },
                    { MetricId.Create(TestMetric.SecondsGauge), 117 },
                },
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.DataReceivedTimeElapsedUpdateCalled);

            // T == 1.75
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 0.7f,
                customData: new Dictionary<MetricId, float>
                {
                    // BytesCounter is configured to be displayed so will
                    // trigger a display update
                    { MetricId.Create(TestMetric.BytesCounter), 419 },
                    { MetricId.Create(TestMetric.SecondsGauge), 117 },
                },
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.NotEnoughTimeElapsed);

            // T == 1.85
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 0.1f,
                customData: new Dictionary<MetricId, float>
                {
                    // BytesCounter is configured to be displayed so will
                    // trigger a display update
                    { MetricId.Create(TestMetric.BytesCounter), 419 },
                    { MetricId.Create(TestMetric.SecondsGauge), 117 },
                },
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.NotEnoughTimeElapsed);

            // T == 2.2
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 0.35f,
                customData: new Dictionary<MetricId, float>
                {
                    // BytesCounter is configured to be displayed so will
                    // trigger a display update
                    { MetricId.Create(TestMetric.BytesCounter), 419 },
                    { MetricId.Create(TestMetric.SecondsGauge), 117 },
                },
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.DataReceivedTimeElapsedUpdateCalled);

            // T == 2.4
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 0.2f,
                customData: new Dictionary<MetricId, float>
                {
                    // BytesCounter is configured to be displayed so will
                    // trigger a display update
                    { MetricId.Create(TestMetric.BytesCounter), 419 },
                    { MetricId.Create(TestMetric.SecondsGauge), 117 },
                },
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.NotEnoughTimeElapsed);

            rnsmTestHarness.Teardown();
        }

    }
}