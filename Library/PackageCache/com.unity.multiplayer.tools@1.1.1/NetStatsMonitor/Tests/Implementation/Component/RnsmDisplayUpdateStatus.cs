using System;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Tests
{
    enum RnsmDisplayUpdateStatus
    {
        /// No new data received; display should not be updated
        NoDataReceived,

        /// No time has elapsed since last update; display should not be updated
        NoTimeElapsed,

        /// Not enough time has elapsed; display should not be updated
        NotEnoughTimeElapsed,

        /// Update was not called; display should not be updated
        UpdateNotCalled,

        /// The RNSM display is refreshed the first time it receives data, even if no time has elapsed
        DataReceivedNoTimeElapsedUpdateCalledFirstTime,

        // New data received, time elapsed, and RNSM update has been called; display should be updated
        DataReceivedTimeElapsedUpdateCalled,
    }

    static class RnsmDisplayUpdateStatusExtensions
    {
        internal static bool UpdateExpected(this RnsmDisplayUpdateStatus status)
        {
            switch (status)
            {
                case RnsmDisplayUpdateStatus.DataReceivedNoTimeElapsedUpdateCalledFirstTime:
                case RnsmDisplayUpdateStatus.DataReceivedTimeElapsedUpdateCalled:
                    return true;
                default:
                    return false;
            }
        }
        internal static string AssertMessage(this RnsmDisplayUpdateStatus status)
        {
            switch (status)
            {
                case RnsmDisplayUpdateStatus.NoDataReceived:
                    return "No new data received; display should not be updated";

                case RnsmDisplayUpdateStatus.NoTimeElapsed:
                    return "No time has elapsed since last update; display should not be updated";

                case RnsmDisplayUpdateStatus.NotEnoughTimeElapsed:
                    return "Not enough time has elapsed; display should not be updated";

                case RnsmDisplayUpdateStatus.UpdateNotCalled:
                    return "Update was not called; display should not be updated";

                case RnsmDisplayUpdateStatus.DataReceivedNoTimeElapsedUpdateCalledFirstTime:
                    return "The RNSM display is refreshed the first time it receives data, even if no time has elapsed";

                case RnsmDisplayUpdateStatus.DataReceivedTimeElapsedUpdateCalled:
                    return
                        "New data received, time elapsed, and RNSM update has been called; display should be updated";
                default:
                    throw new ArgumentException($"Unhandled {nameof(RnsmDisplayUpdateStatus)} {status}");
            }
        }
    }
}