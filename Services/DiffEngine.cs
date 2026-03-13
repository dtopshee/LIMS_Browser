using LegislationTimeMachine.Models;

namespace LegislationTimeMachine.Services
{
    public static class DiffEngine
    {
        public static Dictionary<string, LegislativeDelta> Compare(List<LegislativeNode> oldVersion, List<LegislativeNode> newVersion)
        {
            var results = new Dictionary<string, LegislativeDelta>();

            // Flatten the trees into dictionaries for easier lookup by FID
            var oldFlat = Flatten(oldVersion);
            var newFlat = Flatten(newVersion);

            // Check for additions and modifications
            foreach (var newNode in newFlat)
            {
                if (!oldFlat.TryGetValue(newNode.Key, out var oldNode))
                {
                    // It exists in the new version but not the old one
                    results[newNode.Key] = new LegislativeDelta { Fid = newNode.Key, Type = ChangeType.Added };
                }
                else if (oldNode.Content != newNode.Value.Content || oldNode.Label != newNode.Value.Label)
                {
                    // It exists in both, but the text or structure has changed
                    results[newNode.Key] = new LegislativeDelta { Fid = newNode.Key, Type = ChangeType.Modified };
                }
            }

            // Check for removals
            foreach (var oldNode in oldFlat)
            {
                if (newNode.Value.RepealDate.HasValue && newNode.Value.RepealDate <= targetDate)
                {
                    results[newNode.Key] = new LegislativeDelta { Fid = newNode.Key, Type = ChangeType.Removed };
                }
            }

            return results;
        }

        private static Dictionary<string, LegislativeNode> Flatten(List<LegislativeNode> nodes)
        {
            var dict = new Dictionary<string, LegislativeNode>();
            foreach (var node in nodes)
            {
                if (!string.IsNullOrEmpty(node.Fid))
                {
                    dict[node.Fid] = node;
                }
                // Recursively flatten children
                var children = Flatten(node.Children);
                foreach (var child in children)
                {
                    dict[child.Key] = child.Value;
                }
            }
            return dict;
        }
    }
}
