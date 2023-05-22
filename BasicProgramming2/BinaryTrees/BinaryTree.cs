using System;

namespace BinaryTrees;

public class BinaryTree<T> where T : IComparable
{
    private Node _root;

    public void Add(T key)
    {
        if (_root is null)
            _root = new Node { Key = key };
        else
            Add(_root, key);
    }

    private void Add(Node node, T key)
    {
        if (node.Key.CompareTo(key) > 0)
        {
            if (node.Left is null)
                node.Left = new Node { Key = key};
            else
                Add(node.Left, key);
        }
        else
        {
            if (node.Right is null)
                node.Right = new Node { Key = key};
            else
                Add(node.Right, key);
        }
    }

    public bool Contains(T key)
    {
        return Contains(_root, key);
    }

    private bool Contains(Node node, T key)
    {
        if (node is null)
            return false;

        var comparison = node.Key.CompareTo(key);
        if (comparison > 0)
            return Contains(node.Left, key);
        else if (comparison < 0)
            return Contains(node.Right, key);
        else
            return true;
    }
	
	private class Node
    {
        internal T Key;
        internal Node Left;
        internal Node Right;
    }
}