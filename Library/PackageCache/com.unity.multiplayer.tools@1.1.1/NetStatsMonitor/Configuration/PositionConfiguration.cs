using System;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
    /// <summary>
    /// Configuration for the position of the <see cref="RuntimeNetStatsMonitor"/> on screen
    /// </summary>
    [Serializable]
    public class PositionConfiguration
    {
        /// <summary>
        /// If enabled, the position here will override the position set by the USS styling.
        /// Disable this options if you would like to use the position from the USS styling
        /// instead.
        /// </summary>
        [field: Tooltip(
        "If enabled, the position here will override the position set by the USS styling. " +
        "Disable this options if you would like to use the position from the USS styling " +
        "instead.")]
        [field: SerializeField]
        public bool OverridePosition { get; set; } = true;

        /// <summary>
        /// The position of the Net Stats Monitor from left to right in the range from 0 to 1.
        /// 0 is flush left, 0.5 is centered, and 1 is flush right.
        /// </summary>
        public float PositionLeftToRight
        {
            get => m_PositionLeftToRight;
            set => m_PositionLeftToRight = Mathf.Clamp(
                value,
                ConfigurationLimits.k_PositionMin,
                ConfigurationLimits.k_PositionMax);
        }

        /// <summary>
        /// The position of the Net Stats Monitor from left to right in the range from 0 to 1.
        /// 0 is flush left, 0.5 is centered, and 1 is flush right.
        /// </summary>
        [Tooltip(
            "The position of the Net Stats Monitor from left to right in the range from 0 to 1. " +
            "0 is flush left, 0.5 is centered, and 1 is flush right.")]
        [Range(ConfigurationLimits.k_PositionMin, ConfigurationLimits.k_PositionMax)]
        [SerializeField]
        float m_PositionLeftToRight;

        /// <summary>
        /// The position of the Net Stats Monitor from top to bottom in the range from 0 to 1.
        /// 0 is flush to the top, 0.5 is centered, and 1 is flush to the bottom.
        /// </summary>
        public float PositionTopToBottom
        {
            get => m_PositionTopToBottom;
            set => m_PositionTopToBottom = Mathf.Clamp(
                value,
                ConfigurationLimits.k_PositionMin,
                ConfigurationLimits.k_PositionMax);
        }

        /// <summary>
        /// The position of the Net Stats Monitor from top to bottom in the range from 0 to 1.
        /// 0 is flush to the top, 0.5 is centered, and 1 is flush to the bottom.
        /// </summary>
        [Tooltip(
            "The position of the Net Stats Monitor from top to bottom in the range from 0 to 1. " +
            "0 is flush to the top, 0.5 is centered, and 1 is flush to the bottom.")]
        [Range(ConfigurationLimits.k_PositionMin, ConfigurationLimits.k_PositionMax)]
        [SerializeField]
        float m_PositionTopToBottom;
    }
}