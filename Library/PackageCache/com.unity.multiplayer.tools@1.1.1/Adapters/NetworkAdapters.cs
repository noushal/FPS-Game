using System;
using System.Collections.Generic;

namespace Unity.Multiplayer.Tools.Adapters
{
    delegate void UnsubscribeFromAllAdapters();

    static class NetworkAdapters
    {
        static readonly List<INetworkAdapter> s_Adapters = new List<INetworkAdapter>();

        public static IReadOnlyList<INetworkAdapter> Adapters => s_Adapters;

        public static event Action<INetworkAdapter> OnAdapterAdded;
        public static event Action<INetworkAdapter> OnAdapterRemoved;

        public static void AddAdapter(INetworkAdapter adapter)
        {
            if (!s_Adapters.Contains(adapter))
            {
                s_Adapters.Add(adapter);
                OnAdapterAdded?.Invoke(adapter);
            }
        }

        public static void RemoveAdapter(INetworkAdapter adapter)
        {
            if (s_Adapters.Contains(adapter))
            {
                s_Adapters.Remove(adapter);
                OnAdapterRemoved?.Invoke(adapter);
            }
        }

        /// <summary>
        /// A method that handles subscribing and unsubscribing from all existing
        /// and future adapters, no matter the relative order of initialization, by:
        /// <list type="number">
        ///     <item>
        ///     Running subscribe on all existing adapters.
        ///     </item>
        ///     <item>
        ///     Running subscribe on adapters that are added in future.
        ///     </item>
        ///     <item>
        ///     Running unsubscribe on adapters that are removed in future.
        ///     </item>
        ///     <item>
        ///     Running unsubscribe on all remaining subscribed adapters when
        ///     the return value action is called.
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="subscribeToAdapter">
        /// is run immediately on all existing adapters, and is then run
        /// on each new adapter that is added, until the return value action
        /// is called to unsubscribe from all adapters.
        /// </param>
        /// <param name="unsubscribeFromAdapter">
        /// is run on each adapter that is removed, until the return value action
        /// is called, at which point <paramref name="unsubscribeFromAdapter"/> is run on all
        /// remaining subscribed adapters.
        /// </param>
        /// <returns>
        /// An action that can be called to unsubscribe from all remaining adapters and remove
        /// <paramref name="subscribeToAdapter"/> and <paramref name="unsubscribeFromAdapter"/>
        /// from the <see cref="OnAdapterAdded"/> and <see cref="OnAdapterRemoved"/>
        /// events.
        /// </returns>
        public static UnsubscribeFromAllAdapters SubscribeToAll(
            Action<INetworkAdapter> subscribeToAdapter,
            Action<INetworkAdapter> unsubscribeFromAdapter)
        {
            foreach (var adapter in s_Adapters)
            {
                subscribeToAdapter(adapter);
            }
            OnAdapterAdded += subscribeToAdapter;
            OnAdapterRemoved += unsubscribeFromAdapter;

            void UnsubscribeFromAllAdapters()
            {
                foreach (var adapter in s_Adapters)
                {
                    unsubscribeFromAdapter(adapter);
                }
                OnAdapterAdded -= subscribeToAdapter;
                OnAdapterRemoved -= unsubscribeFromAdapter;
            };
            return UnsubscribeFromAllAdapters;
        }
    }
}