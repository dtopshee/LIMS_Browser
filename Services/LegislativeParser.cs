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
            foreach (var el in elements)
            {
                var node = new LegislativeNode
                {
                    Fid = el.Attribute(lims + "fid")?.Value,
                    ElementName = el.Name.LocalName,
                    Label = el.Element("Label")?.Value,
                    Content = el.Element("Text")?.Value,
                    InForceDate = DateTime.TryParse(el.Attribute(lims + "inforce-start-date")?.Value, out var d) ? d : StatuteBaseline
                };
                
                if (el.Elements().Any(e => e.Name.LocalName != "Label" && e.Name.LocalName != "Text"))
                {
                    node.Children = ParseLevel(el.Elements().Where(e => e.Name.LocalName != "Label" && e.Name.LocalName != "Text"));
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
