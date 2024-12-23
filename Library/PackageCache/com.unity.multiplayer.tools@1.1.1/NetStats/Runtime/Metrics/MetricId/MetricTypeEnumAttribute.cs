using System;

namespace Unity.Multiplayer.Tools.NetStats
{
    /// <summary>
    /// Use this attribute to declare an enum that represents
    /// custom metrics for use with the Multiplayer Tools package.
    /// In particular, this attribute can be used to declare
    /// custom metrics that can be displayed in the <see cref="RuntimeNetStatsMonitor"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum)]
    public class MetricTypeEnumAttribute : Attribute
    {
        /// <summary>
        /// The custom display name to use for the metric enum.
        /// This can be set to something different than the name of the enum
        /// to provide a nicer display name in UIs like the inspector.
        /// </summary>
        public string DisplayName { get; set; }
    }

    /// Attribute internal fields cannot be referenced when applying attributes.
    /// As an alternative, this second, internal attribute can be used to hide metrics
    /// from the inspector that are only intended for internal MP Tools testing.
    [AttributeUsage(AttributeTargets.Enum)]
    internal class MetricTypeEnumHideInInspectorAttribute : Attribute {}
}