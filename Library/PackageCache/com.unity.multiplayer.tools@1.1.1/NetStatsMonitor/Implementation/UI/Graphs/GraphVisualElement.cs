// RNSM Implementation compilation boilerplate
// All references to UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED should be defined in the same way,
// as any discrepancies are likely to result in build failures
// ---------------------------------------------------------------------------------------------------------------------
#if UNITY_EDITOR || ((DEVELOPMENT_BUILD && !UNITY_MP_TOOLS_NET_STATS_MONITOR_DISABLED_IN_DEVELOP) || (!DEVELOPMENT_BUILD && UNITY_MP_TOOLS_NET_STATS_MONITOR_ENABLED_IN_RELEASE))
    #define UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED
#endif
// ---------------------------------------------------------------------------------------------------------------------

#if UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED

using System;
using System.Collections.Generic;

using UnityEngine.UIElements;

using Unity.Multiplayer.Tools.Common;
using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Implementation
{
    class GraphVisualElement : VisualElement
    {
        // Fields from configuration
        // --------------------------------------------------------------------
        List<MetricId> m_Stats;
        int m_SampleCount;
        public SampleRate SampleRate { get; private set; }

        GraphXAxisType m_XAxisType = GraphXAxisType.Samples;

        // Fields computed from configuration
        // --------------------------------------------------------------------
        BaseUnits m_YAxisUnits;
        bool m_YAxisDisplayAsPercentage;

        // Runtime data
        // --------------------------------------------------------------------
        MinAndMax m_PlotRange;
        MinAndMax m_LastYValues;
        double m_LastTimeSpan = Single.MinValue;

        // Visual element children
        // --------------------------------------------------------------------
        readonly Label m_Label = new();

        readonly VisualElement m_GraphAndYAxis = new();
        readonly GraphContent m_Content = new();
        readonly GraphAxisLabels m_YAxisLabels = new();

        readonly GraphAxisLabels m_XAxisLabels = new();

        readonly GraphLegend m_Legend = new();

        internal GraphVisualElement()
        {
            AddToClassList(UssClassNames.k_DisplayElement);
            AddToClassList(UssClassNames.k_Graph);

            m_Label.AddToClassList(UssClassNames.k_DisplayElementLabel);

            m_GraphAndYAxis.AddToClassList(UssClassNames.k_GraphAndYAxis);
            m_Content.AddToClassList(UssClassNames.k_GraphContents);
            m_XAxisLabels.AddToClassList(UssClassNames.k_GraphXAxis);
            m_YAxisLabels.AddToClassList(UssClassNames.k_GraphYAxis);

            Add(m_Label);

            m_GraphAndYAxis.Add(m_Content);
            m_GraphAndYAxis.Add(m_YAxisLabels);
            Add(m_GraphAndYAxis);

            Add(m_XAxisLabels);
            Add(m_Legend);

            m_XAxisLabels.MaxLabelMarginRight = m_YAxisLabels.contentRect.width;
            m_YAxisLabels.RegisterCallback((GeometryChangedEvent geometryChangeEvent) =>
            {
                // Although typically we don't use inline styling, and although it may be possible
                // to do this with USS, this is much more straightforward
                var newWidth = geometryChangeEvent.newRect.width;
                m_XAxisLabels.MaxLabelMarginRight = newWidth;
            });
        }

        internal void UpdateConfiguration(DisplayElementConfiguration config)
        {
            var details = config.GraphConfiguration;

            m_Stats = new List<MetricId>(config.Stats);
            m_SampleCount = details.SampleCount;
            SampleRate = details.SampleRate;

            m_XAxisType = details.XAxisType;

            m_Label.text = config.Label;

            // Although in CSS it's possible, in USS it's not possible to have a selector based on an attribute value,
            // such as an empty label, so we need to add a class instead
            m_Label.EnableInClassList(UssClassNames.k_DisplayElementLabelEmpty, String.IsNullOrWhiteSpace(config.Label));

            m_Content.UpdateConfiguration(config);

            m_YAxisUnits = MetricsUtils.GetUnits(m_Stats, m_Label.text);
            m_YAxisDisplayAsPercentage =
                MetricsUtils.ShouldDisplayAsPercentage(m_Stats, m_Label.text);

            m_YAxisLabels.MinLabel = $"0 {m_YAxisUnits}";

            switch (m_XAxisType)
            {
                case GraphXAxisType.Samples:
                    m_XAxisLabels.SetLabels($"-{m_SampleCount}", "0");
                    break;
                case GraphXAxisType.Time:
                    m_XAxisLabels.MaxLabel = "0 s";
                    break;
                default:
                    throw new ArgumentException($"Unhandled {nameof(GraphXAxisType)} {m_XAxisType}");
            }

            m_Legend.UpdateConfiguration(config);
        }

        /// Returns the YAxis bound display string and value
        (string Label, float Value) ComputeYAxisBound(float plotBound, float currentValue, string currentStringValue)
        {
            var mantissaAndExponent = GraphScalingUtils.NextLargestRoundNumber(plotBound);
            var mantissa = mantissaAndExponent.Mantissa;
            var value = mantissaAndExponent.GetValue(exponentBase: 10);

            if (value == currentValue)
            {
                return (currentStringValue, value);
            }

            var displayString = NumericUtils.Base10ToDisplayNotation(
                mantissaAndExponent,
                significantDigits: mantissa == MathF.Floor(mantissa) ? 1 : 2,
                m_YAxisUnits,
                m_YAxisDisplayAsPercentage);
            return (displayString, value);
        }

        internal void UpdateDisplayData(MultiStatHistory history)
        {
            // The minimum and maximum axis bounds can be computed using the same method without
            // special case handling because m_PlotRange.Min <= 0 and m_PlotRange.Max >= 0,
            // and so the next round number of greater magnitude works as an axis bound in both cases
            var (yAxisMinLabel, minPlotValue) = ComputeYAxisBound(m_PlotRange.Min, m_LastYValues.Min, m_YAxisLabels.MinLabel);
            var (yAxisMaxLabel, maxPlotValue) = ComputeYAxisBound(m_PlotRange.Max, m_LastYValues.Max ,m_YAxisLabels.MaxLabel);

            m_LastYValues.Min = minPlotValue;
            m_LastYValues.Max = maxPlotValue;

            m_PlotRange = m_Content.UpdateDisplayData(history, m_Stats, SampleRate, minPlotValue, maxPlotValue);

            m_YAxisLabels.SetLabels(yAxisMinLabel, yAxisMaxLabel);

            if (m_XAxisType == GraphXAxisType.Time)
            {
                var timeSpan = history.TimeSpanOfLastNSamples(SampleRate, m_SampleCount);
                if (!NumericUtils.Approximately(timeSpan, m_LastTimeSpan, 1E-3))
                {
                    m_LastTimeSpan = timeSpan;
                    m_XAxisLabels.MinLabel = $"-{timeSpan:0.00} s";
                }
            }
        }
    }
}
#endif
