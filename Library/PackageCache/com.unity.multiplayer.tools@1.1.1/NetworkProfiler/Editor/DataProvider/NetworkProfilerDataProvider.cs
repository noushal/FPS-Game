using System;
using Unity.Multiplayer.Tools.NetStats;
using Unity.Multiplayer.Tools.NetworkProfiler.Runtime;
using UnityEditorInternal;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    internal interface INetworkProfilerDataProvider
    {
        MetricCollection GetDataForFrame(long frameIndex);
    }

    internal class NetworkProfilerDataProvider : INetworkProfilerDataProvider
    {
        readonly NetStatSerializer m_NetStatSerializer = new NetStatSerializer();

        public MetricCollection GetDataForFrame(long frameIndex)
        {
            using var frameData = ProfilerDriver.GetRawFrameDataView(Convert.ToInt32(frameIndex), 0);
            if (frameData == null || !frameData.valid)
            {
                return null;
            }

            var bytes = frameData.GetFrameMetaData<byte>(
                FrameInfo.NetworkProfilerGuid,
                FrameInfo.NetworkProfilerDataTag);
            return bytes.Length > 0
                ? m_NetStatSerializer.Deserialize(bytes)
                : null;
        }
    }
}