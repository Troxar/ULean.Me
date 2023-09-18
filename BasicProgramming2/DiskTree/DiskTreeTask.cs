using System;
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

    public IEnumerable<string> GetFormattedNodes(int prefixLength = -1)
    {
        if (prefixLength >= 0)
        {
            var prefix = new string(' ', prefixLength);
            yield return prefix + Name;
        }

        foreach (var node in Nodes.OrderBy(x => x.Key, StringComparer.Ordinal))
            foreach (var str in node.Value.GetFormattedNodes(prefixLength + 1))
                yield return str;
    }
}