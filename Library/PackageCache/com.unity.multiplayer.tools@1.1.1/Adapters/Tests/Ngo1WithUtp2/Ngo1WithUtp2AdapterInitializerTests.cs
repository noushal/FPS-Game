using System.Collections;
using System.Linq;
using NUnit.Framework;
using Unity.Multiplayer.Tools.Adapters.Utp2;
using Unity.Multiplayer.Tools.Adapters.Ngo1WithUtp2;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.TestTools;

namespace Unity.Multiplayer.Tools.Adapters.Tests.Ngo1WithUtp2
{
    public class Ngo1WithUtp2AdapterInitializerTests
    {
        [UnityTest]
        public IEnumerator Ngo1WithUtp2AdapterInitializer_WhenNgo1AndUtp2AreInitialized_RegistersAdapter()
        {
            Ngo1WithUtp2AdapterInitializer.InitializeAdapter();

            var gameObject = new GameObject();
            var networkManager = gameObject.AddComponent<NetworkManager>();
            var transport = gameObject.AddComponent<UnityTransport>();

            networkManager.NetworkConfig = new NetworkConfig
            {
                NetworkTransport = transport,
            };

            networkManager.StartHost();

            yield return null;

            var adapter = Ngo1WithUtp2AdapterInitializer.s_Adapters.Values.FirstOrDefault();
            Assert.IsNotNull(adapter);
            Assert.IsInstanceOf<Utp2Adapter>(adapter);
        }
    }
}