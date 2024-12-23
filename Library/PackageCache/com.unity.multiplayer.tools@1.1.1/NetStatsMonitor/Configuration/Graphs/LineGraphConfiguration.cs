using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
    /// <summary>
    /// Configuration for Line Graph specific options.
    /// </summary>
    [Serializable]
    public sealed class LineGraphConfiguration
    {
        /// <summary>
        /// The line thickness for a line graph.
        /// By default this is set to one.
        /// </summary>
        /// <remarks>
        /// The accepted range for the line thickness is between 1 and 10.
        /// If the value goes beyond those value, it will be clamped.
        /// </remarks>
        public float LineThickness
        {
            get => m_LineThickness;
            set => m_LineThickness = Mathf.Clamp(value, ConfigurationLimits.k_GraphLineThicknessMin, ConfigurationLimits.k_GraphLineThicknessMax);
        }

        /// The line thickness for a line graph.
        /// By default this is set to one.
        [SerializeField]
        [Range(ConfigurationLimits.k_GraphLineThicknessMin, ConfigurationLimits.k_GraphLineThicknessMax)]
        float m_LineThickness = 1;

        internal int ComputeHashCode()
        {
            return LineThickness.GetHashCode();
        }
    }
}