using System;
using System.Collections;
using System.Collections.Generic;

namespace BinaryTrees;

public class BinaryTree<T> : IEnumerable<T> where T : IComparable
{
    private bool _hasKey;
    private T _key;
    private BinaryTree<T> _left;
    private BinaryTree<T> _right;
    private int _count;

    public void Add(T key)
    {
        _count++;
        if (!_hasKey)
        {
            _hasKey = true;
            _key = key;
        }
        else
        {
            var comparison = _key.CompareTo(key);
            if (comparison > 0 && _left is null)
                _left = new BinaryTree<T>(key);
            else if (comparison > 0)
                _left.Add(key);
            else if (comparison <= 0 && _right is null)
                _right = new BinaryTree<T>(key);
            else
                _right.Add(key);
        }
    }

    public BinaryTree()
    {
        
    }

    private BinaryTree(T key)
    {
        _key = key;
        _hasKey = true;
        _count = 1;
    }

    public bool Contains(T key)
    {
        if (!_hasKey)
            return false;
        var comparison = _key.CompareTo(key);
        if (comparison > 0)
            return _left?.Contains(key) ?? false;
        else if (comparison < 0)
            return _right?.Contains(key) ?? false;
        else
            return true;
    }

    public IEnumerator<T> GetEnumerator()
    {
        if (!_hasKey)
            yield break;

        if (_left is not null)
            foreach (var key in _left)
                yield return key;

        yield return _key;

        if (_right is not null)
            foreach (var key in _right)
                yield return key;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public T this[int i]
    {
        get
        {
            return GetKeyByIndex(i);
        }
        private set => throw new NotImplementedException();
    }

    private T GetKeyByIndex(int i)
    {
        if (i < 0 || i >= _count)
            throw new IndexOutOfRangeException();

        var leftCount = _left?._count ?? 0;
        if (i < leftCount)
            return _left[i];
        else if (i > leftCount)
            return _right[i - leftCount - 1];
        else
            return _key;
    }
}