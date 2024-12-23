using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Multiplayer.Tools.NetworkSimulator.Runtime;

namespace Unity.Multiplayer.Tools.NetworkSimulator.Editor.UI
{
    static class NetworkScenarioTypesLibrary
    {
        internal static IList<Type> Types { get; private set; }

        static NetworkScenarioTypesLibrary()
        {
            RefreshTypes();
        }

        internal static NetworkScenario GetInstanceForTypeName(string typeName)
        {
            var scenario = Types.First(x => x.Name == typeName);
            return (NetworkScenario)Activator.CreateInstance(scenario);
        }

        static void RefreshTypes()
        {
            if (Types != null)
            {
                return;
            }
            
            Types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(TypeIsValidNetworkScenario)
                .ToList();
        }

        static bool TypeIsValidNetworkScenario(Type type)
        {
            return type.IsClass && type.IsAbstract == false && typeof(NetworkScenario).IsAssignableFrom(type);
        }
    }
}
