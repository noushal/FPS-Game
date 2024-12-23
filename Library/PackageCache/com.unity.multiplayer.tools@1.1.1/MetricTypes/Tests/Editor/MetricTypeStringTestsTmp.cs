using System;
using NUnit.Framework;
using Unity.Collections;

namespace Unity.Multiplayer.Tools.MetricTypes.Tests
{
    class MetricTypeStringTestsTmp
    {
        const string k_ValidString = "abc";

        static readonly string k_LongString = "65CharString65CharString65CharString65CharString65CharString65Cha";
        static readonly FixedString64Bytes k_LongStringExpected = k_LongString.Substring(0, FixedString64Bytes.UTF8MaxLengthInBytes);

        static void AssertLongStringCase<T>(
            string parameterName,
            Func<string, T> constructor,
            Func<T, FixedString64Bytes> publicAccessor)
        {
            T obj = default;
            Assert.DoesNotThrow(() => obj = constructor(k_LongString), $"Exception thrown when constructing {typeof(T).Name} with a very long string sent to parameter {parameterName}");
            var publicValue = publicAccessor(obj);
            Assert.AreEqual(k_LongStringExpected, publicValue);
        }

        static void AssertEmptyStringCase<T>(
            string parameterName,
            Func<string, T> constructor,
            Func<T, FixedString64Bytes> publicAccessor)
        {
            T obj = default;
            Assert.DoesNotThrow(()=> obj = constructor(string.Empty), $"Exception thrown when constructing {typeof(T).Name} with an empty string sent to parameter {parameterName}");
            var publicValue = publicAccessor(obj);
            Assert.AreEqual(0, publicValue.Length);
        }

        static void AssertNullStringCase<T>(
            string parameterName,
            Func<string, T> constructor,
            Func<T, FixedString64Bytes> publicAccessor)
        {
            T obj = default;
            Assert.DoesNotThrow(()=> obj = constructor(null), $"Exception thrown when constructing {typeof(T).Name} with a null string sent to parameter {parameterName}");
            var publicValue = publicAccessor(obj);
            Assert.AreEqual(0, publicValue.Length);
        }

        static void AssertRegularStringCase<T>(
            string parameterName,
            Func<string, T> constructor,
            Func<T, FixedString64Bytes> publicAccessor)
        {
            const string regularString = "RegularString";
            T obj = default;
            Assert.DoesNotThrow(()=> obj = constructor(regularString), $"Exception thrown when constructing {typeof(T).Name} with a regular string sent to parameter {parameterName}");
            var publicValue = publicAccessor(obj);
            Assert.AreEqual(regularString, publicValue);
        }
        
        static void AssertStringParameterCorrectness<T>(
            string parameterName,
            Func<string, T> constructor,
            Func<T, FixedString64Bytes> publicAccessor)
        {
            AssertLongStringCase(parameterName, constructor, publicAccessor);
            AssertEmptyStringCase(parameterName, constructor, publicAccessor);
            AssertNullStringCase(parameterName, constructor, publicAccessor);
            AssertRegularStringCase(parameterName, constructor, publicAccessor);
        }

        [Test]
        public void RpcEvent_RpcName()
        {
            AssertStringParameterCorrectness(
                nameof(RpcEvent.Name),
                str => new RpcEvent(default, default, str, k_ValidString, default),
                evt => evt.Name);
        }
        
        [Test]
        public void RpcEvent_BehaviourName()
        {
            AssertStringParameterCorrectness(
                nameof(RpcEvent.NetworkBehaviourName),
                str => new RpcEvent(default, default, k_ValidString, str, default),
                evt => evt.NetworkBehaviourName);
        }
        
        
        [Test]
        public void NamedMessageEvent_Name()
        {
            AssertStringParameterCorrectness(
                nameof(NamedMessageEvent.Name),
                str => new NamedMessageEvent(default, str, default),
                evt => evt.Name);
        }
        
        [Test]
        public void NetworkMessageEvent_Name()
        {
            AssertStringParameterCorrectness(
                nameof(NetworkMessageEvent.Name),
                str => new NetworkMessageEvent(default, str, default),
                evt => evt.Name);
        }
        
        [Test]
        public void NetworkObjectIdentifier_Name()
        {
            AssertStringParameterCorrectness(
                nameof(NetworkObjectIdentifier.Name),
                str => new NetworkObjectIdentifier(str, default),
                obj => obj.Name);
        }

        [Test]
        public void NetworkVariableEvent_VariableName()
        {
            AssertStringParameterCorrectness(
                nameof(NetworkVariableEvent.Name),
                str => new NetworkVariableEvent(default, default, str, k_ValidString, default),
                evt => evt.Name);
        }
        
        [Test]
        public void NetworkVariableEvent_BehaviourName()
        {
            AssertStringParameterCorrectness(
                nameof(NetworkVariableEvent.NetworkBehaviourName),
                str => new NetworkVariableEvent(default, default, k_ValidString, str, default),
                evt => evt.NetworkBehaviourName);
        }
        
        [Test]
        public void SceneEvent_SceneEventType()
        {
            AssertStringParameterCorrectness(
                nameof(SceneEventMetric.SceneEventType),
                str => new SceneEventMetric(default, str, k_ValidString, default),
                evt => evt.SceneEventType);
        }
        
        [Test]
        public void SceneEvent_SceneName()
        {
            AssertStringParameterCorrectness(
                nameof(SceneEventMetric.SceneName),
                str => new SceneEventMetric(default, k_ValidString, str, default),
                evt => evt.SceneName);
        }
    }
}