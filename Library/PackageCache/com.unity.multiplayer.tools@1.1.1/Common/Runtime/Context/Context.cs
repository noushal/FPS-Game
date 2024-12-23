using UnityEngine;

namespace Unity.Multiplayer.Tools.Common
{
    /// <summary>
    /// Defines a Tool-specific context, to serve as entry point for dependency wiring.
    /// Base interface definition for <see cref="IEditorSetupHandler"/> and <see cref="IRuntimeSetupHandler"/>.
    /// </summary>
    interface IContext
    {
    }

    /// <summary>
    /// Defines a Context to receive enable callback at edit time.
    /// Editor Contexts are automatically disposed during Domain Reload.
    /// </summary>
    interface IEditorSetupHandler : IContext
    {
        void EditorSetup();
    }

    /// <summary>
    /// Defines a Context to receive enable and disable callbacks at runtime.
    /// </summary>
    interface IRuntimeSetupHandler : IContext
    {
        void RuntimeSetup();
        void RuntimeTeardown();
    }

    /// <summary>
    /// Tool-specific context, to serve as entry point for dependency wiring.
    /// Initialized in Editor while in Unity or at Runtime in a Build.
    /// </summary>
    abstract class SetupHandler : IEditorSetupHandler, IRuntimeSetupHandler
    {
        protected enum ContextStatus
        {
            Disabled,
            EnabledInEditor,
            EnabledInRuntime
        }
        
        protected ContextStatus Status { get; private set; }

        void IEditorSetupHandler.EditorSetup()
        {
            Debug.Assert(Status == ContextStatus.Disabled);
            Status = ContextStatus.EnabledInEditor;
            Setup();
        }

        void IRuntimeSetupHandler.RuntimeSetup()
        {
            // In a build, this will be always false:
            if (Status == ContextStatus.EnabledInEditor)
            {
                return;
            }

            Debug.Assert(Status == ContextStatus.Disabled);
            Status = ContextStatus.EnabledInRuntime;
            Setup();
        }

        void IRuntimeSetupHandler.RuntimeTeardown()
        {
            // If the class was initialized in Editor, it doesn't disable at runtime.
            if (Status == ContextStatus.EnabledInEditor)
            {
                return;
            }

            Debug.Assert(Status != ContextStatus.Disabled);
            EnsureTeardown();
        }

        protected abstract void Setup();

        protected abstract void Teardown();

        void EnsureTeardown()
        {
            try
            {
                Teardown();
            }
            finally
            {
                Status = ContextStatus.Disabled;
            }
        }
    }

    /// <summary>
    /// Tool-specific entry point, initialized only in editor and not initialized in a build.
    /// </summary>
    abstract class EditorOnlyContext : IEditorSetupHandler
    {
        void IEditorSetupHandler.EditorSetup()
        {
            Setup();
        }

        protected abstract void Setup();
    }

    /// <summary>
    /// Tool-specific entry point, initialized only at runtime either in Unity or in a build.
    /// </summary>
    abstract class RuntimeOnlyContext : IRuntimeSetupHandler
    {
        void IRuntimeSetupHandler.RuntimeSetup()
        {
            Setup();
        }

        public void RuntimeTeardown()
        {
            Teardown();
        }

        protected abstract void Setup();

        protected abstract void Teardown();
    }
}
