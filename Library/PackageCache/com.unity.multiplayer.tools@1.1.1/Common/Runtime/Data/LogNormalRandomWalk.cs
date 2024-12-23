using System;
using UnityEngine;
using Random = System.Random;

namespace Unity.Multiplayer.Tools.Common
{
    /// Log Normal Random Walks, also called Geometric Random Walks, are used to simulate stock prices over time.
    /// For our purposes, they are useful for creating fake data with a trend for testing the RNSM.
    [Serializable]
    internal class LogNormalRandomWalk
    {
        [field: SerializeField]
        public float Rate { get; set; } = 1f;

        [field: SerializeField]
        public float Min { get; set; } = 1e-2f;

        [field: SerializeField]
        public float Max { get; set; } = 10f;

        public float Value { get; private set; } = 1f;

        public float NextFloat(Random random)
        {
            var exponent = Rate * (float)(random.NextDouble() - 0.5);
            var multiplier = Mathf.Exp(exponent);
            Value *= multiplier;
            Value = Mathf.Clamp(Value, Min, Max);
            return Value;
        }

        public int NextInt(Random random)
        {
            var x = NextFloat(random);
            return (int)Mathf.Round(x);
        }

        /// Performs the given action N times, where N is distributed according to a log-normal random walk
        public void Repeat(Random random, Action action)
        {
            var n = NextInt(random);
            for (var i = 0; i < n; ++i)
            {
                action();
            }
        }
    }
}