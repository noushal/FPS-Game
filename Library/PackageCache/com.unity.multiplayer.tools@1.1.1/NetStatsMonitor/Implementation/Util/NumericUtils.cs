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
using System.Globalization;
using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Implementation
{
    /// Alternative representation to reduce/avoid floating point error
    struct MantissaAndExponent
    {
        public float Mantissa { get; set; }
        public int Exponent { get; set; }

        public float GetValue(float exponentBase) => Mantissa * MathF.Pow(exponentBase, Exponent);
    }

    static class NumericUtils
    {
        const char k_SmallSpace = '\u2009';
        const char k_DivisionSlash = '/';

        public static MantissaAndExponent ToBase10(float value)
        {
            if (value == 0)
            {
                // The exponent would be -inf, which we can't represent with an int,
                // so return a mantissa of 0
                return new MantissaAndExponent();
            }

            // Operate on the absolute value and store the sign for later
            var sign = MathF.Sign(value);
            value = MathF.Abs(value);

            var exponentOf10 = MathF.Floor(MathF.Log10(value));

            var powerOf10 = MathF.Pow(10, exponentOf10);

            var mantissa = value / powerOf10;

            return new MantissaAndExponent
            {
                Mantissa = sign * mantissa,
                Exponent = (int)exponentOf10,
            };
        }

        public static MantissaAndExponent ToBase10(double value)
        {
            if (value == 0)
            {
                // The exponent would be -inf, which we can't represent with an int,
                // so return a mantissa of 0
                return new MantissaAndExponent();
            }

            // Operate on the absolute value and store the sign for later
            var sign = Math.Sign(value);
            value = Math.Abs(value);

            var exponentOf10 = Math.Floor(Math.Log10(value));

            var powerOf10 = Math.Pow(10, exponentOf10);

            var mantissa = value / powerOf10;

            return new MantissaAndExponent
            {
                Mantissa = sign * (float)mantissa,
                Exponent = (int)exponentOf10,
            };
        }

        /// Converts a MantissaAndExponent from base 10 to base 1000
        /// for use in engineering notation: https://en.wikipedia.org/wiki/Engineering_notation
        public static MantissaAndExponent Base10ToBase1000(MantissaAndExponent inputBase10)
        {
            var exponentBase1000 = (int)MathF.Floor(inputBase10.Exponent / 3f);
            var remainderExponent = (inputBase10.Exponent % 3 + 3) % 3;
            var remainderMultiplier =
                remainderExponent == 2 ? 100 :
                remainderExponent == 1 ? 10 :
                1;
            return new MantissaAndExponent
            {
                Mantissa = inputBase10.Mantissa * remainderMultiplier,
                Exponent = exponentBase1000,
            };
        }

        public static float RoundToSignificantDigits(
            float input,
            int significantDigits,
            int inputDigitsAboveDecimal)
        {
            // The reason for this slightly DIY rounding function is two-fold:
            // 1. We already know the number of digits above the decimal point so can avoid recomputing this
            // 2. At the time of writing, MathF.Round(float x, int digits) has a bug where it raises an error
            //    when digits > 5 with a message falsely indicating that it supports 15 digits of precision.
            //    This incorrect error message may be a copy-paste error from the implementation of
            //    Math.Round(double x, int digits). Even so, 32-bit floating point is precise to 7.2 decimal
            //    digits so this limit of 5 digits is unnecessarily strict
            var result = input;
            var multiplier = MathF.Pow(10, significantDigits - inputDigitsAboveDecimal);
            result *= multiplier;
            result = MathF.Round(result);
            result /= multiplier;
            return result;
        }

        public static (string leadingNumber, string prefixSymbol) GetLeadingNumberAndPrefixSymbol(
            MantissaAndExponent inputBase1000,
            float roundedValue,
            int digitsBelowDecimal)
        {
            // Metric prefixes are powers of 1000, so we can convert directly this way
            var metricPrefix = (MetricPrefix)inputBase1000.Exponent;

            if (metricPrefix < MetricPrefix.Min)
            {
                // Special case handling for exponents too small to have a metric prefix
                return ("0", "");
            }
            if (metricPrefix > MetricPrefix.Max)
            {
                // Special case handling for exponents too large to have a metric prefix
                return (inputBase1000.Mantissa >= 0 ? "+∞" : "-∞", "");
            }
            var prefixSymbol = metricPrefix.GetSymbol();
            
            var leadingNumber = roundedValue.ToString("N" + digitsBelowDecimal, CultureInfo.CurrentCulture);

            return (leadingNumber, prefixSymbol);
        }
        
        /// Returns (string LeadingNumber, string PrefixSymbol)
        public static (string leadingNumber, string prefixSymbol) GetLeadingNumberAndPrefixSymbol(
            MantissaAndExponent inputBase1000,
            int significantDigits)
        {
            if (Math.Abs(inputBase1000.Mantissa - 1e3f) < MathF.Pow(10, 3 - significantDigits))
            {
                // If we would round up to one-thousand, re-normalize the input,
                // so that we display something like 1 rather than 1,000 milli
                inputBase1000.Mantissa *= 1e-3f;
                inputBase1000.Exponent += 1;
            }

            var digitsAboveDecimal = GetDigitsAboveDecimal(inputBase1000, false);
            var digitsBelowDecimal = GetDigitsBelowDecimal(significantDigits, digitsAboveDecimal);
            
            var roundedValue = RoundToSignificantDigits(
                inputBase1000.Mantissa,
                significantDigits,
                digitsAboveDecimal);

            return GetLeadingNumberAndPrefixSymbol(inputBase1000, roundedValue, digitsBelowDecimal);
        }

        public static string Base1000ToEngineeringNotation(
            MantissaAndExponent inputBase1000,
            BaseUnits units,
            float roundedValue,
            int digitsBelowdecimal)
        {
            var (leadingNumber, prefixSymbol) = GetLeadingNumberAndPrefixSymbol(inputBase1000,
                roundedValue,
                digitsBelowdecimal);
            var (unitsNumerator, unitsDenominator) = units.NumeratorAndDenominatorDisplayStrings;
            return leadingNumber
                + (unitsNumerator == "" ? prefixSymbol : $"{k_SmallSpace}{prefixSymbol}{unitsNumerator}")
                + (unitsDenominator == "" ? "" : $"{k_DivisionSlash}{unitsDenominator}");
        }
        
        public static string Base1000ToEngineeringNotation(
            MantissaAndExponent inputBase1000,
            int significantDigits,
            BaseUnits units)
        {
            var (leadingNumber, prefixSymbol) = GetLeadingNumberAndPrefixSymbol(inputBase1000, significantDigits);
            var (unitsNumerator, unitsDenominator) = units.NumeratorAndDenominatorDisplayStrings;
            return leadingNumber
                   + (unitsNumerator == "" ? prefixSymbol : $"{k_SmallSpace}{prefixSymbol}{unitsNumerator}")
                   + (unitsDenominator == "" ? "" : $"{k_DivisionSlash}{unitsDenominator}");
        }

        public static string Base10ToPercentageNotation(
            MantissaAndExponent inputBase10,
            int significantDigits,
            BaseUnits units,
            float roundedValue)
        {
            var percentage = 100 * roundedValue * MathF.Pow(10, inputBase10.Exponent);

            var significantDigitsAboveDecimal = Math.Max(inputBase10.Exponent + 3, 0);

            var significantDigitsBelowDecimal =
                Math.Max(0, significantDigits - significantDigitsAboveDecimal);

            var leadingNumber = percentage.ToString(
                "N" + significantDigitsBelowDecimal,
                CultureInfo.CurrentCulture);

            // Given that this is being displayed as a percentage there really shouldn't
            // be any units in the numerator. There may, however, be units in the denominator
            // as in the example `Progress Rate: 0.37% / s`
            var (unitsNumerator, unitsDenominator) = units.NumeratorAndDenominatorDisplayStrings;

            return $"{leadingNumber}%"
                + (unitsNumerator   == "" ? "" : $"{k_SmallSpace}{unitsNumerator}")
                + (unitsDenominator == "" ? "" : $"{k_DivisionSlash}{unitsDenominator}");
        }
        
        public static string Base10ToPercentageNotation(
            MantissaAndExponent inputBase10,
            int significantDigits,
            BaseUnits units)
        {
            var roundedValue = RoundToSignificantDigits(
                inputBase10.Mantissa,
                significantDigits,
                inputDigitsAboveDecimal: 1);

            return Base10ToPercentageNotation(
                inputBase10,
                significantDigits,
                units,
                roundedValue);
        }

        public static string Base10ToDisplayNotation(
            MantissaAndExponent inputBase10,
            int significantDigits,
            BaseUnits units,
            bool displayAsPercentage)
        {
            return displayAsPercentage
                ? Base10ToPercentageNotation(
                    inputBase10,
                    significantDigits,
                    units)
                : Base1000ToEngineeringNotation(
                    Base10ToBase1000(inputBase10),
                    significantDigits,
                    units);
        }

        public static bool Approximately(double a, double b, double tolerance)
        {
            return Math.Abs(a - b) < tolerance;
        }
        
        public static int GetDigitsAboveDecimal(MantissaAndExponent inputBase1000, bool displayAsPercentage)
        {
            return displayAsPercentage ? 1 : 
                inputBase1000.Mantissa >= 100f ? 3 :
                inputBase1000.Mantissa >=  10f ? 2 :
                1;
        }

        public static int GetDigitsBelowDecimal(int significantDigits, int digitsAboveDecimal)
        {
            return Math.Max(significantDigits - digitsAboveDecimal, 0);
        }
    }
}
#endif
