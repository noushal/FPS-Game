using System;

namespace Unity.Multiplayer.Tools.NetStats
{
    static class UnitExtensions
    {
        internal static BaseUnits GetBaseUnits(this Units units)
        {
            switch (units)
            {
                case Units.None:
                    return new BaseUnits();

                case Units.Bytes:
                    return new BaseUnits(bytesExponent: 1);

                case Units.BytesPerSecond:
                    return new BaseUnits(bytesExponent: 1, secondsExponent: -1);

                case Units.Seconds:
                    return new BaseUnits(secondsExponent: 1);

                case Units.Hertz:
                    return new BaseUnits(secondsExponent: -1);

                default:
                    throw new ArgumentOutOfRangeException(nameof(units), units, null);
            }
        }
    }
}