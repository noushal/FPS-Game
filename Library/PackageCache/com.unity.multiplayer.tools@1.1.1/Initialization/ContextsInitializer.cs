// Uncomment the line below to remove initialization and tear-down logs.
// #define UNITY_MP_TOOLS_CONTEXT_TRACE_CALLS

// Uncomment the line bellow to simulate a Build environment, so then you can check whether
// Runtime Contexts are correctly running or if Editor Only Contexts are disabled as expected.
// #define UNITY_MP_TOOLS_SIMULATE_BUILD

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Multiplayer.Tools.Common;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Unity.Multiplayer.Tools.Context
{
    /// <summary>
    /// Main entry point initializing, enabling and disabling all Tools-specific Contexts.
    /// This class is automatically initialized in Editor and at Runtime.
    /// No other class or assembly should reference this class.
    /// </summary>
#if UNITY_EDITOR && !UNITY_MP_TOOLS_SIMULATE_BUILD
    [UnityEditor.InitializeOnLoad]
#endif
    static class ContextsInitializer
    {
        static readonly IContext[] s_Contexts;

        static ContextsInitializer()
        {
            TraceCall();

            Application.quitting += DisableRuntimeContexts;

            s_Contexts = ContextsDefinition.Get();

#if UNITY_EDITOR && !UNITY_MP_TOOLS_SIMULATE_BUILD
            EnableEditorContexts();
#endif
        }

        static void EnableEditorContexts()
        {
            TraceCall();

            foreach (var context in s_Contexts)
            {
                if (context is IEditorSetupHandler editorContext)
                {
                    editorContext.EditorSetup();
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void EnableRuntimeContexts()
        {
            TraceCall();

            foreach (var context in s_Contexts)
            {
                if (context is IRuntimeSetupHandler runtimeContext)
                {
                    runtimeContext.RuntimeSetup();
                }
            }
        }

        static void DisableRuntimeContexts()
        {
            TraceCall();

            foreach (var context in s_Contexts)
            {
                if (context is IRuntimeSetupHandler runtimeContext)
                {
                    runtimeContext.RuntimeTeardown();
                }
            }
        }

        [Conditional("UNITY_MP_TOOLS_CONTEXT_TRACE_CALLS")]
        static void TraceCall([CallerMemberName] string methodName = "")
        {
            Debug.Log($"{nameof(ContextsInitializer)}.{methodName}");
        }
    }
}
