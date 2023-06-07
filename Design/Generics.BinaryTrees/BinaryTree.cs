using System;
using System.Collections;
using System.Collections.Generic;

namespace Generics.BinaryTrees
{
    public class BinaryTree<T> : IEnumerable<T>
        where T : IComparable
    {
        public bool HasValue;

        private T _value;
        public T Value
        {
            get { return _value; }
            private set
            {
                _value = value;
                HasValue = true;
            }
        }

        public BinaryTree<T> Left { get; private set; }
        public BinaryTree<T> Right { get; private set; }

        public BinaryTree() { }

        public BinaryTree(T value)
        {
            Value = value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (!HasValue)
                yield break;

            if (Left != null)
                foreach (T value in Left)
                    yield return value;

            yield return Value;

            if (Right != null)
                foreach (T value in Right)
                    yield return value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T value)
        {
            if (!HasValue)
                Value = value;
            else if (value.CompareTo(Value) <= 0)
            {
                if (Left is null)
                    Left = new BinaryTree<T>(value);
                else
                    Left.Add(value);
            }
            else
            {
                if (Right is null)
                    Right = new BinaryTree<T>(value);
                else
                    Right.Add(value);
            }
        }
    }

    public static class BinaryTree
    {
        public static BinaryTree<T> Create<T>(params T[] values)
            where T : IComparable
        {
            var tree = new BinaryTree<T>();
            foreach (T value in values)
                tree.Add(value);
            return tree;
        }
    }
}