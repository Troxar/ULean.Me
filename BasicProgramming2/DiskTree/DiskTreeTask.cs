using System.Collections.Generic;
using System.Linq;

namespace DiskTree;

public static class DiskTreeTask
{
    public static List<string> Solve(List<string> input)
    {
        var root = new DiskTreeNode("");

        foreach (var str in input)
            root.AddNodes(str);

        return root.GetFormattedNodes().ToList();
    }
}

public class DiskTreeNode
{
    public string Name { get; private set; }
    public Dictionary<string, DiskTreeNode> Nodes { get; private set; }

    public DiskTreeNode(string name)
    {
        Name = name;
        Nodes = new();
    }

    public void AddNodes(string input)
    {
        if (string.IsNullOrEmpty(input))
            return;

        var position = input.IndexOf('\\');
        var name = position == -1 
            ? input 
            : input.Substring(0, position);

        if (!Nodes.Keys.Contains(name))
            Nodes.Add(name, new DiskTreeNode(name));

        if (position != -1)
            Nodes[name].AddNodes(input.Substring(position + 1));
    }

    public IEnumerable<string> GetFormattedNodes(string prefix = "")
    {
        var nextPrefix = prefix;
        if (!string.IsNullOrEmpty(Name))
        {
            yield return prefix + Name;
            nextPrefix = prefix + " ";
        }
        
        foreach (var node in Nodes.OrderBy(x => x.Key, StringComparer.Ordinal))
            foreach (var str in node.Value.GetFormattedNodes(nextPrefix))
                yield return str;
    }
}