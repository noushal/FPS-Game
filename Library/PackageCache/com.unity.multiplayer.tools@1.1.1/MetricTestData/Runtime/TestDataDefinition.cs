#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using Unity.Multiplayer.Tools.MetricTestData.Definitions;
using Random = System.Random;

namespace Unity.Multiplayer.Tools.MetricTestData
{
    // Words from https://github.com/dariusk/corpora CC0 license
    class TestDataDefinition
    {
        readonly IReadOnlyList<string> m_VariableTypes = new[] { "count", "size", "state" };

        readonly Random m_Random;

        public TestDataDefinition(int seed)
        {
            m_Random = new Random(seed);
        }

        public string GenerateGameObjectName()
        {
            return $"{Capitalize(GetRandomValue(Adjectives.Values))} {Capitalize(GetRandomValue(Adjectives.Values))} {Capitalize(GetRandomValue(Nouns.Values))}";
        }

        public string GenerateComponentName()
        {
            return $"{Capitalize(GetRandomValue(Nouns.Values))}{Capitalize(GetRandomValue(Verbs.Values))}Component";
        }

        public string GenerateVariableName()
        {
            return $"{Capitalize(GetRandomValue(Nouns.Values))}{Capitalize(GetRandomValue(Nouns.Values))}{Capitalize(GetRandomValue(m_VariableTypes))}";
        }

        public string GenerateNamedMessageName()
        {
            return $"{Capitalize(GetRandomValue(Verbs.Values))}{Capitalize(GetRandomValue(Nouns.Values))}";
        }

        public string GenerateRpcName()
        {
            return GenerateNamedMessageName();
        }

        public long GenerateByteCount()
        {
            // Generate mostly byte-sized, a few kilo-sized and sometimes mega-sized
            var magnitudeSelector = m_Random.Next(0, 10);
            if (magnitudeSelector == 10)
            {
                return m_Random.Next(1000000, 2000000);
            }

            if (magnitudeSelector > 7)
            {
                return m_Random.Next(1000, 999999);
            }

            return m_Random.Next(1, 999);
        }


        public string GenerateSceneName()
        {
            return $"{Capitalize(GetRandomValue(Adjectives.Values))}{Capitalize(GetRandomValue(Adjectives.Values))}Scene";
        }

        string GetRandomValue(IReadOnlyList<string> collection)
        {
            return !collection.Any()
                ? string.Empty
                : collection[m_Random.Next(0, collection.Count)];
        }

        static string Capitalize(string input)
        {
            return string.IsNullOrEmpty(input)
                ? string.Empty
                : $"{char.ToUpper(input[0])}{input.Substring(1)}";
        }
    }
}

#endif