using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Multiplayer.Tools.Common
{
    static class TaskExtensions
    {
        /// <summary>
        /// Observes the task to avoid the task fail silently.
        /// </summary>
        public static void Forget(this Task task)
        {
            if (!task.IsCompleted || task.IsFaulted)
            {
                _ = ForgetAwaited(task);
            }

            static async Task ForgetAwaited(Task task, bool logCanceledTask = false)
            {
                try
                {
                    await task.ConfigureAwait(false);
                }
                catch (TaskCanceledException exception)
                {
                    if (logCanceledTask)
                    {
                        Debug.LogException(exception);
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }
    }
}
