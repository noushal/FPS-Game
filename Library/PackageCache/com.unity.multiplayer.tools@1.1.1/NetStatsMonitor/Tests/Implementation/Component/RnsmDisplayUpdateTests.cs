using System;
using NUnit.Framework;

using Unity.Multiplayer.Tools.Adapters;
using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Tests
{
    class RnsmDisplayUpdateTests
    {
        MockNgo1Adapter m_MockNgo1Adapter;
        [OneTimeSetUp]
        public void Setup()
        {
            m_MockNgo1Adapter = new MockNgo1Adapter();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            m_MockNgo1Adapter?.Uninitialize();
            m_MockNgo1Adapter = null;
        }

        [Test]
        public void EnsureRnsmDisplayOnlyUpdatesWhenNewDataIsReceived()
        {
            RnsmTestHarness rnsmTestHarness = new RnsmTestHarness(rnsmRefreshRate: 1f);

            // T == 0
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 0f,
                generateNewData: false,
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.NoDataReceived);

            // T == 1
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 1f,
                generateNewData: false,
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.NoDataReceived);

            // T == 2
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 1f,
                generateNewData: false,
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.NoDataReceived);

            // T == 3
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 1f,
                generateNewData: true,
                updateRnsm: false,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.UpdateNotCalled);

            // T == 4
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 1f,
                generateNewData: false,
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.DataReceivedTimeElapsedUpdateCalled);

            // T == 5
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 1f,
                generateNewData: false,
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.NoDataReceived);

            rnsmTestHarness.Teardown();
        }

        [Test]
        public void EnsureRnsmDisplayOnlyUpdatesWhenEnoughTimeHasElapsed()
        {
            RnsmTestHarness rnsmTestHarness = new RnsmTestHarness(rnsmRefreshRate: 1f);

            // T == 0
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 0f,
                generateNewData: false,
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.NoDataReceived);

            // T == 0
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 0f,
                generateNewData: true,
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.DataReceivedNoTimeElapsedUpdateCalledFirstTime);

            // T == 0
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 0f,
                generateNewData: true,
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.NoTimeElapsed);

            // T == 0.2
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 0.2f,
                generateNewData: true,
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.NotEnoughTimeElapsed);

            // T == 0.8
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 0.6f,
                generateNewData: true,
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.NotEnoughTimeElapsed);

            // T == 0.95
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 0.15f,
                generateNewData: true,
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.NotEnoughTimeElapsed);

            // T == 1.05
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 0.1f,
                generateNewData: true,
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.DataReceivedTimeElapsedUpdateCalled);

            // T == 1.75
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 0.7f,
                generateNewData: true,
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.NotEnoughTimeElapsed);

            // T == 1.85
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 0.1f,
                generateNewData: true,
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.NotEnoughTimeElapsed);

            // T == 2.2
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 0.35f,
                generateNewData: true,
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.DataReceivedTimeElapsedUpdateCalled);

            // T == 2.4
            rnsmTestHarness.SimulateFrameAndVerifyDisplayUpdate(
                timeElapsed: 0.2f,
                generateNewData: true,
                updateRnsm: true,
                expectedUpdateStatus: RnsmDisplayUpdateStatus.NotEnoughTimeElapsed);

            rnsmTestHarness.Teardown();
        }
    }

    class MockNgo1Adapter : INetworkAdapter, IMetricCollectionEvent
    {
        public MockNgo1Adapter()
        {
            MetricEvents.MetricEventPublisher.OnMetricsReceived += OnMetricsReceived;
            NetworkAdapters.AddAdapter(this);
        }

        public void Uninitialize()
        {
            MetricEvents.MetricEventPublisher.OnMetricsReceived -= OnMetricsReceived;
            NetworkAdapters.RemoveAdapter(this);
        }

        void OnMetricsReceived(MetricCollection metricCollection)
        {
            MetricCollectionEvent?.Invoke(metricCollection);
        }

        public AdapterMetadata Metadata { get; }
        public T GetComponent<T>() where T : class, IAdapterComponent
        {
            return this as T;
        }

        public event Action<MetricCollection> MetricCollectionEvent;
    }
}