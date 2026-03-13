using LegislationTimeMachine.Models;

namespace LegislationTimeMachine.Services
{
    public static class DiffEngine
    {
       public static Dictionary<string, LegislativeDelta> Compare(List<LegislativeNode> oldVersion, List<LegislativeNode> newVersion)
{
    var results = new Dictionary<string, LegislativeDelta>();
    var oldFlat = Flatten(oldVersion);
    var newFlat = Flatten(newVersion);

    // Check for additions and modifications
    foreach (var entry in newFlat) // changed 'newNode' to 'entry'
    {
        var fid = entry.Key;
        var newNode = entry.Value;

        if (!oldFlat.TryGetValue(fid, out var oldNode))
        {
            results[fid] = new LegislativeDelta { Fid = fid, Type = ChangeType.Added };
        }
        else if (oldNode.Content != newNode.Content || oldNode.Label != newNode.Label || oldNode.MarginalNote != newNode.MarginalNote)
        {
            results[fid] = new LegislativeDelta { Fid = fid, Type = ChangeType.Modified };
        }
    }

    // Check for removals
    foreach (var entry in oldFlat)
    {
        if (!newFlat.ContainsKey(entry.Key))
        {
            results[entry.Key] = new LegislativeDelta { Fid = entry.Key, Type = ChangeType.Removed };
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
