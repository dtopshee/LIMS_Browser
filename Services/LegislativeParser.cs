using System.Xml.Linq;
using LegislationTimeMachine.Models;

namespace LegislationTimeMachine.Services
{
    public class LegislationParser
    {
        private static readonly DateTime StatuteBaseline = new DateTime(2003, 1, 1);
        private static readonly XNamespace lims = "http://justice.gc.ca/lims";

        public List<LegislativeNode> ParseToTree(XDocument doc)
        {
            var body = doc.Descendants("Body").FirstOrDefault();
            return body == null ? new List<LegislativeNode>() : ParseLevel(body.Elements());
        }

        private List<LegislativeNode> ParseLevel(IEnumerable<XElement> elements)
        {
            var nodes = new List<LegislativeNode>();
            XNamespace lims = "http://justice.gc.ca/lims";
        
            foreach (var el in elements)
            {
                // Don't skip these—we need to extract their values below
                var elementName = el.Name.LocalName;
                if (elementName == "Label" || elementName == "Text" || elementName == "MarginalNote") 
                    continue;
        
                var node = new LegislativeNode
                {
                    Fid = el.Attribute(lims + "fid")?.Value ?? el.Attribute(lims + "id")?.Value,
                    ElementName = elementName,
                    Label = el.Element("Label")?.Value,
                    MarginalNote = el.Element("MarginalNote")?.Value, // This is the fix for the notes
                    Content = el.Element("Text")?.Value,
                    
                    // Capture the dates for Time Travel
                    InForceDate = DateTime.TryParse(el.Attribute(lims + "inforce-start-date")?.Value, out var start) ? start : StatuteBaseline,
                    RepealDate = DateTime.TryParse(el.Attribute(lims + "inforce-end-date")?.Value, out var end) ? end : (DateTime?)null
                };
        
                if (el.Elements().Any())
                {
                    node.Children = ParseLevel(el.Elements());
                }
        
                nodes.Add(node);
            }
            return nodes;
        }

        public static List<LegislativeNode> GetVersionAtDate(List<LegislativeNode> masterNodes, int targetYear)
        {
            var filtered = new List<LegislativeNode>();
            var targetDate = new DateTime(targetYear, 12, 31);

            foreach (var node in masterNodes)
            {
                if (node.InForceDate <= targetDate)
                {
                    var clone = new LegislativeNode {
                        Fid = node.Fid,
                        Label = node.Label,
                        Content = node.Content,
                        ElementName = node.ElementName,
                        InForceDate = node.InForceDate,
                        Children = GetVersionAtDate(node.Children, targetYear)
                    };
                    filtered.Add(clone);
                }
            }
            return filtered;
        }
    }
}
