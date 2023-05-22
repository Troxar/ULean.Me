using System;
using System.Collections;
using System.Collections.Generic;

namespace BinaryTrees;

public class BinaryTree<T> : IEnumerable<T> where T : IComparable
{
    private Node _root;

    public void Add(T key)
    {
        if (_root is null)
            _root = new Node(key);
        else
            AddToTree(key);
    }

    private void AddToTree(T key)
    {
        var node = _root;
        while (true)
        {
            node.Count++;
            var comparison = node.Key.CompareTo(key);
            if (comparison > 0 && node.Left is null)
            {
                node.Left = new Node(key);
                break;
            }
            else if (comparison > 0)
                node = node.Left;
            else if (comparison <= 0 && node.Right is null)
            {
                node.Right = new Node(key);
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
        internal int Count;

        internal Node(T key)
        {
            Key = key;
            Count = 1;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        return Enumerate(_root).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private IEnumerable<T> Enumerate(Node node)
    {
        if (node is null)
            yield break;

        foreach (var key in Enumerate(node.Left))
            yield return key;

        yield return node.Key;

        foreach (var key in Enumerate(node.Right))
            yield return key;
    }

    public T this[int i]
    {
        get
        {
            return GetNodeByIndex(i).Key;
        }
        private set => throw new NotImplementedException();
    }

    private Node GetNodeByIndex(int i)
    {
        if (i < 0 || i >= (_root?.Count ?? 0))
            throw new IndexOutOfRangeException();

        var node = _root;
        while (true)
        {
            var leftCount = node.Left?.Count ?? 0;
            if (i < leftCount)
                node = node.Left;
			else if (i > leftCount)
			{
				i -= leftCount + 1;
                node = node.Right;
			}
			else
				return node;
        }
    }
}