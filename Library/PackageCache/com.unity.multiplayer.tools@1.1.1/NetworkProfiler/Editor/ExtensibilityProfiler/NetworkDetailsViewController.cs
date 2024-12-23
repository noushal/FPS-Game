#if UNITY_2021_2_OR_NEWER
using Unity.Profiling.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    class NetworkDetailsViewController : ProfilerModuleViewController
    {
        readonly string m_TabName;

        readonly INetworkProfilerDataProvider m_NetworkProfilerDataProvider;
        readonly NetworkProfilerDetailsView m_NetworkProfilerDetailsView;

        public NetworkDetailsViewController(ProfilerWindow profilerWindow, string tabName)
            : base(profilerWindow)
        {
            m_TabName = tabName;

            m_NetworkProfilerDataProvider = new NetworkProfilerDataProvider();
            m_NetworkProfilerDetailsView = new NetworkProfilerDetailsView();

            ProfilerWindow.SelectedFrameIndexChanged += OnSelectedFrameChanged;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            ProfilerWindow.SelectedFrameIndexChanged -= OnSelectedFrameChanged;

            base.Dispose(true);
        }

        protected override VisualElement CreateView()
        {
            m_NetworkProfilerDetailsView.ShowTab(m_TabName);
            m_NetworkProfilerDetailsView.PopulateView(m_NetworkProfilerDataProvider.GetDataForFrame(ProfilerWindow.selectedFrameIndex));

            return m_NetworkProfilerDetailsView;
        }

        void OnSelectedFrameChanged(long selectedFrameIndex)
        {
            m_NetworkProfilerDetailsView?.PopulateView(m_NetworkProfilerDataProvider.GetDataForFrame(ProfilerWindow.selectedFrameIndex));
        }
    }
}
#endif