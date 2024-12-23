using System;

namespace Unity.Multiplayer.Tools.NetStats
{
    internal enum MetricPrefix : sbyte
    {
        /// Metric prefix for 1000^-6
        Atto  = -6,
        /// Metric prefix for 1000^-5
        Femto = -5,
        /// Metric prefix for 1000^-4
        Pico  = -4,
        /// Metric prefix for 1000^-3
        Nano  = -3,
        /// Metric prefix for 1000^-2
        Micro = -2,
        /// Metric prefix for 1000^-1
        Milli = -1,

        None = 0,

        /// Metric prefix for 1000^1
        Kilo = 1,
        /// Metric prefix for 1000^2
        Mega = 2,
        /// Metric prefix for 1000^3
        Giga = 3,
        /// Metric prefix for 1000^4
        Tera = 4,
        /// Metric prefix for 1000^5
        Peta = 5,
        /// Metric prefix for 1000^6
        Exa  = 6,

        /// The smallest metric prefix available
        Min = Atto,
        /// The largest metric prefix available
        Max = Exa,
    }

    internal static class UnitPrefixExtensions
    {
        public static string GetSymbol(this MetricPrefix prefix)
        {
            switch (prefix)
            {
                case MetricPrefix.Atto:  return "a";
                case MetricPrefix.Femto: return "f";
                case MetricPrefix.Pico:  return "p";
                case MetricPrefix.Nano:  return "n";
                case MetricPrefix.Micro: return "μ";
                case MetricPrefix.Milli: return "m";

                case MetricPrefix.None:  return "";

                case MetricPrefix.Kilo:  return "k";
                case MetricPrefix.Mega:  return "M";
                case MetricPrefix.Giga:  return "G";
                case MetricPrefix.Tera:  return "T";
                case MetricPrefix.Peta:  return "P";
                case MetricPrefix.Exa:   return "E";

                default:
                    throw new ArgumentException($"Unhandled {nameof(MetricPrefix)} {prefix}");
            }
        }

        public static float GetValueFloat(this MetricPrefix prefix)
        {
            switch (prefix)
            {
                case MetricPrefix.Atto:  return 1e-18f;
                case MetricPrefix.Femto: return 1e-15f;
                case MetricPrefix.Pico:  return 1e-12f;
                case MetricPrefix.Nano:  return 1e-9f;
                case MetricPrefix.Micro: return 1e-6f;
                case MetricPrefix.Milli: return 1e-3f;

                case MetricPrefix.None: return 1;

                case MetricPrefix.Kilo: return 1e3f;
                case MetricPrefix.Mega: return 1e6f;
                case MetricPrefix.Giga: return 1e9f;
                case MetricPrefix.Tera: return 1e12f;
                case MetricPrefix.Peta: return 1e15f;
                case MetricPrefix.Exa:  return 1e18f;

                default:
                    throw new ArgumentException($"Unhandled {nameof(MetricPrefix)} {prefix}");
            }
        }

    }
}