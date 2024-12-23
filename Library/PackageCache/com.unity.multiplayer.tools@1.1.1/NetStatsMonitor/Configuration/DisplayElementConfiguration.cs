using System;
using System.Collections.Generic;

using UnityEngine;
using Unity.Multiplayer.Tools.Common;
using Unity.Multiplayer.Tools.NetStats;
using Unity.Multiplayer.Tools.NetStatsMonitor.Configuration;

namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
    /// <summary>
    /// Configuration class used by <see cref="NetStatsMonitorConfiguration"/> to be displayed at runtime
    /// by <see cref="RuntimeNetStatsMonitor"/>.
    /// </summary>
    [Serializable]
    public sealed class DisplayElementConfiguration : ISerializationCallbackReceiver
    {
        /// This provides a method of determining whether the fields have been
        /// correctly initialized to their default values, as if they have been
        /// zero initialized then this will be false. This zero initialization
        /// can occur when a new DisplayElement is added to an empty list in
        /// the inspector.
        [field: HideInInspector]
        [field: SerializeField]
        internal bool FieldsInitialized { get; private set; } = true;

        /// <summary>
        /// The <see cref="DisplayElementType"/> of the display element.
        /// The label to display for this visual element.
        /// For graphs this field is optional, as the variables displayed in the
        /// graph are shown in the legend. Consider leaving this field blank for
        /// graphs if you would like to make them more compact.
        /// </summary>
        [Tooltip(
            "The label to display for this visual element in the on-screen display. " +
            "For graphs this field is optional, as the variables displayed in the " +
            "graph are shown in the legend. Consider leaving this field blank for " +
            "graphs if you would like to make them more compact."
        )]
        [field: SerializeField]
        public DisplayElementType Type { get; set; }

        /// <summary>
        /// The label of the display element.
        /// </summary>
        [field: SerializeField]
        public string Label { get; set; } = "";

        /// <summary>
        /// The list of stats represented by <see cref="MetricId"/> to display.
        /// </summary>
        [field: SerializeField]
        public List<MetricId> Stats { get; set; } = new();

        /// <summary>
        /// Counter configuration if <see cref="Type"/> is set to Counter.
        /// </summary>
        [field: SerializeField]
        public CounterConfiguration CounterConfiguration { get; set; } = new();

        /// <summary>
        /// Graph configuration if <see cref="Type"/> is set to LineGraph or StackedAreaGraph.
        /// </summary>
        [field: SerializeField]
        public GraphConfiguration GraphConfiguration { get; set; } = new();

        /// The number of historical values that need to be stored for this DisplayElement
        internal int SampleCount
        {
            get
            {
                switch (Type)
                {
                    case DisplayElementType.Counter:
                    {
                        return CounterConfiguration.SampleCount;
                    }
                    case DisplayElementType.LineGraph:
                    case DisplayElementType.StackedAreaGraph:
                    {
                        return GraphConfiguration.SampleCount;
                    }
                    default:
                        throw new NotSupportedException(
                            $"Unhandled {nameof(DisplayElementType)} {Type}");
                }
            }
        }

        /// The sample rate of this visual element
        internal SampleRate SampleRate
        {
            get
            {
                switch (Type)
                {
                    case DisplayElementType.Counter:
                    {
                        return CounterConfiguration.SampleRate;
                    }
                    case DisplayElementType.LineGraph:
                    case DisplayElementType.StackedAreaGraph:
                    {
                        return GraphConfiguration.SampleRate;
                    }
                    default:
                        throw new NotSupportedException(
                            $"Unhandled {nameof(DisplayElementType)} {Type}");
                }
            }
        }

        /// The HalfLife required for the Continuous Exponential Moving Average
        /// of this DisplayElement (if any)
        internal double? HalfLife
        {
            get
            {
                switch (Type)
                {
                    case DisplayElementType.Counter:
                    {
                        var smoothingMethod = CounterConfiguration.SmoothingMethod;
                        switch (smoothingMethod)
                        {
                            case SmoothingMethod.ExponentialMovingAverage:
                                return CounterConfiguration.ExponentialMovingAverageParams.HalfLife;
                            case SmoothingMethod.SimpleMovingAverage:
                                return null;
                            default:
                                throw new NotSupportedException(
                                    $"Unhandled {nameof(SmoothingMethod)} {smoothingMethod}");
                        }
                    }
                    case DisplayElementType.LineGraph:
                    case DisplayElementType.StackedAreaGraph:
                        return null;
                    default:
                        throw new NotSupportedException(
                            $"Unhandled {nameof(DisplayElementType)} {Type}");
                }
            }
        }

        /// The DecayConstant required for the Continuous Exponential Moving Average
        /// of this DisplayElement (if any)
        internal double? DecayConstant => HalfLife.HasValue
            ? ContinuousExponentialMovingAverage.GetDecayConstantForHalfLife(HalfLife.Value)
            : null;

        internal void OnValidate()
        {
            RefreshGenerateLabel();
            ValidateColors();
        }

        int m_PreviousStatsHash = 0;
        string m_PreviousGeneratedLabel = "";
        void RefreshGenerateLabel()
        {
            if (Type != DisplayElementType.Counter)
            {
                // We don't do label generation for graphs because:
                // 1. Unlike counters they don't need a label to understood (they have a legend)
                // 2. They're more compact without labels
                // So users can enter labels for them, but we don't generate them automatically
                return;
            }
            var currentStatsHash = ComputeStatsHashCode();
            if (m_PreviousStatsHash == 0)
            {
                m_PreviousStatsHash = currentStatsHash;
                m_PreviousGeneratedLabel = LabelGeneration.GenerateLabel(Stats);
                return;
            }
            if (currentStatsHash == m_PreviousStatsHash)
            {
                return;
            }
            m_PreviousStatsHash = currentStatsHash;

            // The stats have changed
            var newGeneratedLabel = LabelGeneration.GenerateLabel(Stats);
            if (Label == m_PreviousGeneratedLabel)
            {
                Label = newGeneratedLabel;
            }
            m_PreviousGeneratedLabel = newGeneratedLabel;
        }

        void ValidateColors()
        {
            // A new element in a Reordable list will either be copied from the previous element
            // or zero initialized if it's the first
            // In those scenarios, if all the elements have a black color (r=0, g=0, b=0)
            // And the alpha is also 0, we assume these are new custom colors and we set the alpha to 1

            var variableColors = GraphConfiguration?.VariableColors;

            if (variableColors == null)
            {
                return;
            }

            var areAllColorsZeroInitialized = true;
            for (int j = 0; j < variableColors.Count; ++j)
            {
                var graphConfigurationVariableColor = variableColors[j];
                if (graphConfigurationVariableColor.a != 0f ||
                    graphConfigurationVariableColor.r != 0f ||
                    graphConfigurationVariableColor.g != 0f ||
                    graphConfigurationVariableColor.b != 0f)
                {
                    areAllColorsZeroInitialized = false;
                    break;
                }
            }

            if (areAllColorsZeroInitialized)
            {
                for (int j = 0; j < variableColors.Count; ++j)
                {
                    var graphConfigurationVariableColor = variableColors[j];
                    graphConfigurationVariableColor.a = 1f;
                    variableColors[j] = graphConfigurationVariableColor;
                }
            }
        }

        // Custom Serialization of Stats as Strings, so that reordering does not break configuration
        // ----------------------------------------------------------------------------------------
        [Serializable]
        struct SerializedStat
        {
            [field: HideInInspector]
            [field: SerializeField]
            public string TypeName { get; set; }

            [field: HideInInspector]
            [field: SerializeField]
            public string ValueName { get; set; }
        }

        [field: HideInInspector]
        [field: SerializeField]
        List<SerializedStat> SerializedStats { get; set; } = new();

        bool m_SerializedStatsLoaded = false;

        /// <summary>
        /// For internal use.
        /// Implementation for ISerializationCallbackReceiver.
        /// Called before Unity serialize the object.
        /// This allow to keep the configuration details when reloading assemblies
        /// or making change in the code.
        /// </summary>
        public void OnBeforeSerialize()
        {
            var statCount = Stats.Count;
            SerializedStats.Resize(statCount);
            for (var i = 0; i < statCount; ++i)
            {
                var stat = Stats[i];
                SerializedStats[i] = new SerializedStat
                {
                    TypeName = stat.EnumType.AssemblyQualifiedName,
                    ValueName = stat.Name,
                };
            }
        }

        /// <summary>
        /// For internal use.
        /// Implementation for ISerializationCallbackReceiver.
        /// Called after Unity deserialize the object.
        /// This allow to keep the configuration details when reloading assemblies
        /// or making change in the code.
        /// </summary>
        public void OnAfterDeserialize()
        {
            if (m_SerializedStatsLoaded)
            {
                // NOTE: Only load these values once per assembly reload!
                // I don't know why, but in response to changes in the inspector, Unity calls
                // OnAfterDeserialize before calling OnBeforeSerialize. Without this check, this
                // results in the newly modified Stats being overwritten by the previously saved
                // SerializedStats, before the SerializedStats are recomputed from the overwritten
                // Stats.
                //
                // In pseudocode, this is a bit like the following:
                //     Stats = Deserialize(SerializedStats);
                //     SerializedStats = Serialize(Stats);
                //
                // As a result of this ordering the stats can never change, as it's always loading
                // the previously saved value. To avoid this, I'm only allowing it to load once
                // per assembly reload. We only need to load once per assembly reload, as the
                // SerializedStats are only needed to correct for reordering of enum values
                // which can only occur with a code change and assembly reload.
                return;
            }
            m_SerializedStatsLoaded = true;

            var statCount = SerializedStats.Count;
            Stats.Resize(statCount);

            for (int i = 0; i < statCount; ++i)
            {
                var serializedStat = SerializedStats[i];
                var serializedType = System.Type.GetType(serializedStat.TypeName);
                if (serializedType == null)
                {
                    continue;
                }
                var typeIndex = MetricIdTypeLibrary.GetTypeIndex(serializedType);
                var enumNames = MetricIdTypeLibrary.GetEnumNames(typeIndex);
                var serializedName = serializedStat.ValueName;
                var valueIndex = enumNames.IndexOf(serializedName);
                if (valueIndex == -1)
                {
                    continue;
                }
                var enumValues = MetricIdTypeLibrary.GetEnumValues(typeIndex);
                var enumValue = enumValues[valueIndex];
                Stats[i] = new MetricId(typeIndex: typeIndex, enumValue: enumValue);
            }
        }

#if UNITY_2021_2_OR_NEWER // HashCode isn't defined in Unity < 2021.2
        internal int ComputeStatsHashCode()
        {
            var hash = 0;
            foreach (var stat in Stats)
            {
                hash = HashCode.Combine(hash, stat);
            }
            return hash;
        }

        internal int ComputeHashCode()
        {
            int hash = HashCode.Combine(Type, Label, ComputeStatsHashCode());
            switch (Type)
            {
                case DisplayElementType.Counter:
                    hash = HashCode.Combine(hash, CounterConfiguration.ComputeHashCode());
                    break;
                case DisplayElementType.LineGraph:
                case DisplayElementType.StackedAreaGraph:
                    hash = HashCode.Combine(hash, GraphConfiguration.ComputeHashCode());
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknow {nameof(DisplayElementType)} {Type}");
            }
            return hash;
        }
#endif
    }
}