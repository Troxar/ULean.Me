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
            AddToTree(key);
    }

    private void AddToTree(T key)
    {
        var node = _root;
        while (true)
        {
            var comparison = node.Key.CompareTo(key);
            if (comparison > 0 && node.Left is null)
            {
                node.Left = new Node { Key = key };
                break;
            }
            else if (comparison > 0)
                node = node.Left;
            else if (comparison <= 0 && node.Right is null)
            {
                node.Right = new Node { Key = key };
                break;
            }
            else
                node = node.Right;
        }
    }

    public bool Contains(T key)
    {
        var node = _root;
        while (true)
        {
            if (node is null)
                return false;
            var comparison = node.Key.CompareTo(key);
            if (comparison > 0)
                node = node.Left;
            else if (comparison < 0)
                node = node.Right;
            else
                return true;
        }
    }
	
    private class Node
    {
        internal T Key;
        internal Node Left;
        internal Node Right;
    }
}