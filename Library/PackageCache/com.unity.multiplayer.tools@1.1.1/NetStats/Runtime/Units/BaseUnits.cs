using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStats
{
    readonly struct BaseUnits
    {
        internal sbyte BytesExponent { get; }
        internal sbyte SecondsExponent { get; }

        public BaseUnits(
            sbyte bytesExponent = 0,
            sbyte secondsExponent = 0)
        {
            BytesExponent = bytesExponent;
            SecondsExponent = secondsExponent;
        }

        public BaseUnits WithSeconds(sbyte seconds)
        {
            return new BaseUnits(
                bytesExponent: BytesExponent,
                secondsExponent: seconds);
        }

        public bool Equals(BaseUnits other)
        {
            return BytesExponent == other.BytesExponent &&
                   SecondsExponent == other.SecondsExponent;
        }

        public override bool Equals(object obj)
        {
            return obj is BaseUnits other && Equals(other);
        }

        public override int GetHashCode()
        {
            // Note: System.Hashcode functionality isn't available in Unity < 2021.2,
            // so this custom hashing can be replaced when versions < 2021.2 no longer
            // need to be supported

            // By virtue of the fact that all four fields combined are less than 32 bits
            // we can pack them all in a 32 bit HashCode, and don't even need to hash.
            return
                ((byte)BytesExponent << 8) |
                (byte)SecondsExponent;
        }

        internal sbyte GetExponent(BaseUnit unit)
        {
            switch (unit)
            {
                case BaseUnit.Byte:
                    return BytesExponent;
                case BaseUnit.Second:
                    return SecondsExponent;
                default:
                    throw new ArgumentException($"Unhandled BaseUnit {unit}");
            }
        }

        static readonly char[] k_Superscripts = new char[]
            { '⁰', '¹', '²', '³', '⁴', '⁵', '⁶', '⁷', '⁸', '⁹', };

        /// The numerator and denominator display strings
        /// If there are no units in the numerator, the numerator returned will be an empty string.
        /// Likewise, if there are no units in the denominator, the denominator returned will be an empty string.
        internal (string, string) NumeratorAndDenominatorDisplayStrings
        {
            get
            {
                var numerator = "";
                var denominator = "";

                void AddUnit(BaseUnit unit, sbyte exponent, ref string str)
                {
                    str += unit.GetSymbol();
                    if (exponent <= 1)
                    {
                        return;
                    }
                    if (exponent >= 100)
                    {
                        str += k_Superscripts[exponent / 100];
                        exponent %= 100;
                    }
                    if (exponent >= 10)
                    {
                        str += k_Superscripts[exponent / 10];
                        exponent %= 10;
                    }
                    str += k_Superscripts[exponent / 10];
                }

                for (var unit = (BaseUnit)0;
                    (int)unit < BaseUnitConstants.k_BaseUnitCount;
                    ++unit)
                {
                    var exponent = GetExponent(unit);
                    if (exponent > 0)
                    {
                        AddUnit(unit, exponent, ref numerator);
                    }
                    else if (exponent < 0)
                    {
                        AddUnit(unit, Math.Abs(exponent), ref denominator);
                    }
                }
                return (numerator, denominator);
            }
        }

        internal string DisplayString
        {
            get
            {
                var (numerator, denominator) = NumeratorAndDenominatorDisplayStrings;
                return numerator + (denominator == "" ? "" : "/" + denominator);
            }
        }

        public override string ToString()
            => DisplayString;
    }
}