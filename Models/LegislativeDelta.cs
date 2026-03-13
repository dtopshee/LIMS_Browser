using System;
using LegislationTimeMachine.Models;

namespace LegislationTimeMachine.Models
{
    /// <summary>
    /// Represents the difference between two versions of a legislative element.
    /// </summary>
    public class LegislativeDelta
    {
        public string Fid { get; set; } = string.Empty;
        public ChangeType Type { get; set; }
        
        // Optional: Store the actual nodes for deeper inspection (like side-by-side text diffs)
        public LegislativeNode? OldVersion { get; set; }
        public LegislativeNode? NewVersion { get; set; }
    }

    /// <summary>
    /// Defines the nature of the change for a specific FID.
    /// </summary>
    public enum ChangeType
    {
        Added,      // Node exists in the new version but not the old
        Removed,    // Node exists in the old version but not the new
        Modified,   // Node exists in both, but content or label (renumbering) has changed
        Unchanged   // Node is identical in both snapshots
    }
}
