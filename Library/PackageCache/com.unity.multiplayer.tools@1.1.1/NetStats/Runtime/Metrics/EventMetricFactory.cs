using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStats
{
    class EventMetricFactory : IMetricFactory
    {
        public static bool TryGetFactoryTypeName(Type type, out FixedString128Bytes typeName) => k_TypeNames.TryGetValue(type, out typeName);

        interface IEventMetricFactory
        {
            IMetric Construct(MetricId id);
        }

        class EventMetricFactoryImpl<T> : IEventMetricFactory where T : unmanaged
        {
            public IMetric Construct(MetricId id)
            {
                return new EventMetric<T>(id);
            }
        }

        static readonly Dictionary<FixedString128Bytes, IEventMetricFactory> k_FactoriesByName = new Dictionary<FixedString128Bytes, IEventMetricFactory>();
        static readonly Dictionary<Type, FixedString128Bytes> k_TypeNames = new Dictionary<Type, FixedString128Bytes>();

        static EventMetricFactory()
        {
            TypeRegistration.RunIfNeeded();
        }
        
        internal static void RegisterType<T>() where T : unmanaged
        {
            if(k_TypeNames.ContainsKey(typeof(T)))
            {
                return;
            }

            FixedString128Bytes uniqueName = typeof(T).FullName;
            k_FactoriesByName.Add(uniqueName, new EventMetricFactoryImpl<T>());
            k_TypeNames.Add(typeof(T), uniqueName);
        }

        public bool TryConstruct(MetricHeader header, out IMetric metric)
        {
            if (!k_FactoriesByName.TryGetValue(header.EventFactoryTypeName, out var factory))
            {
                Debug.LogError("Failed to find factory for event type " + header.EventFactoryTypeName);
                metric = default;
                return false;
            }

            metric = factory.Construct(header.MetricId);
            return true;
        }
    }
}
