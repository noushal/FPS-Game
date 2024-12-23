using System;
using System.Collections.Generic;
using Unity.Multiplayer.Tools.MetricTypes;
using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Configuration
{
    static class LabelGeneration
    {
        // Returns the name without any direction and the direction separate
        public static (string, NetworkDirection) SeparateDirectionFromName(string name)
        {
            const string k_Sent = "sent";
            const string k_Received = "received";
            var comparison = StringComparison.InvariantCultureIgnoreCase;

            NetworkDirection direction = NetworkDirection.None;
            var directionlessName = name;
            {
                var sentIndex = directionlessName.IndexOf(k_Sent, comparison);
                if (sentIndex > 0)
                {
                    direction |= NetworkDirection.Sent;
                    directionlessName = directionlessName.Remove(sentIndex, k_Sent.Length);
                }
            }
            {
                var receivedIndex = directionlessName.IndexOf(k_Received, comparison);
                if (receivedIndex > 0)
                {
                    direction |= NetworkDirection.Received;
                    directionlessName = directionlessName.Remove(receivedIndex, k_Received.Length);
                }
            }
            directionlessName = directionlessName.Trim();
            return (directionlessName, direction);
        }

        public static string GenerateLabel(List<MetricId> stats)
        {
            var statCount = stats.Count;
            switch (statCount)
            {
                case 0:
                    return "";
                case 1:
                    return stats[0].DisplayName;
                case 2:
                {
                    var name0 = stats[0].DisplayName;
                    var name1 = stats[1].DisplayName;

                    if (name0 == name1)
                    {
                        // Edge case to get out of the way early, though the fact that each new value
                        // in the stats list in the inspector is copied from the one before makes this
                        // much more likely to occur
                        return $"2 × {name0}";
                    }

                    var (name0NoDirection, direction0) = SeparateDirectionFromName(name0);
                    var (name1NoDirection, direction1) = SeparateDirectionFromName(name1);

                    if (name0NoDirection == name1NoDirection)
                    {
                        // Its the same underlying stat, differing only in direction
                        switch (direction0, direction1)
                        {
                        case (NetworkDirection.Sent, NetworkDirection.Received):
                        case (NetworkDirection.Received, NetworkDirection.Sent):
                            return $"{name0NoDirection} Sent and Received";
                        }
                    }

                    // Different underlying stat, but they may be going in the same direction
                    switch (direction0, direction1)
                    {
                    case (NetworkDirection.Sent, NetworkDirection.Sent):
                        return $"{name0NoDirection} and {name1NoDirection} Sent";
                    case (NetworkDirection.Received, NetworkDirection.Received):
                        return $"{name0NoDirection} and {name1NoDirection} Received";
                    }

                    // Two different stats without a common direction, no redundancy to reduce here
                    return $"{name0} and {name1}";
                }
                default:
                    // Any more than two stats and we give up trying to generate a label
                    // that explains the common thread of all these variables.
                    // Will need to leave that up to the user to fill in.
                    return "";
            }
        }
    }
}