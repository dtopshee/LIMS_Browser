using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace LegislationTimeMachine.Models
{
    /// <summary>
    /// Represents any version-controlled element in the Canadian Legislative XML (lims).
    /// </summary>
    public class LegislativeNode
    {
        // LIMS attributes for tracking
        public string Fid { get; set; }           // Immutable Feature ID (lims:fid)
        public string VersionId { get; set; }     // Point-in-time specific ID (lims:id)
        public DateTime? InForceDate { get; set; } // lims:inforce-start-date
        public DateTime? EnactedDate { get; set; } // lims:enacted-date
        
        // Structural properties
        public string ElementName { get; set; }   // Section, Subsection, Paragraph, etc.
        public string Label { get; set; }         // "1", "(a)", "253.1"
        public string MarginalNote { get; set; }
        public string Content { get; set; }       // The raw or cleaned Text element
        
        // Heirarchy
        public LegislativeNode Parent { get; set; }
        public List<LegislativeNode> Children { get; set; } = new();

        // Metadata
        public List<string> HistoricalNotes { get; set; } = new();

        /// <summary>
        /// Generates a semantic hash of the content and structure to detect 
        /// changes even if the FID/ID remain identical (edge cases).
        /// </summary>
        public string GetContentHash() 
        {
            // Simple implementation for demo; in production use a SHA256 of normalized text
            return $"{Label}|{MarginalNote}|{Content}".GetHashCode().ToString();
        }
    }

    /// <summary>
    /// Represents a change detected between two points in time for a single FID.
    /// </summary>
    public enum ChangeType { Added, Modified, Removed, Unchanged }

 
}
