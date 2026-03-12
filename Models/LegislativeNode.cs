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

    public class LegislativeDelta
    {
        public string Fid { get; set; }
        public ChangeType Type { get; set; }
        public LegislativeNode OldVersion { get; set; }
        public LegislativeNode NewVersion { get; set; }
    }


public class LegislativeParser
{
    private static readonly XNamespace lims = "http://justice.gc.ca/lims";

    public LegislativeNode ParseToTree(XElement root)
    {
        var node = new LegislativeNode
        {
            ElementName = root.Name.LocalName,
            Fid = root.Attribute(lims + "fid")?.Value,
            VersionId = root.Attribute(lims + "id")?.Value,
            Label = root.Element("Label")?.Value,
            MarginalNote = root.Element("MarginalNote")?.Value,
            Content = root.Element("Text")?.Value,
            InForceDate = ParseLimsDate(root.Attribute(lims + "inforce-start-date")?.Value)
        };

        // Capture Historical Notes if they exist at this level
        var hist = root.Element("HistoricalNote");
        if (hist != null)
        {
            foreach (var item in hist.Elements("HistoricalNoteSubItem"))
                node.HistoricalNotes.Add(item.Value);
        }

        // Recursively build children for nested structures
        foreach (var childElement in root.Elements())
        {
            // We only care about versioned children for the 'Time Machine' logic
            if (childElement.Attribute(lims + "fid") != null)
            {
                var childNode = ParseToTree(childElement);
                childNode.Parent = node;
                node.Children.Add(childNode);
            }
        }

        return node;
    }

    private DateTime? ParseLimsDate(string dateStr)
    {
        if (DateTime.TryParse(dateStr, out DateTime dt)) return dt;
        return null;
    }
}

  
}
