using System;
using System.Collections.Generic;

namespace Delegates.TreeTraversal
{
    public static class Traversal
    {
        public static IEnumerable<Product> GetProducts(ProductCategory root)
        {
            return Traverse(
                root,
                node => node.Products,
                node => node.Categories);
        }

        public static IEnumerable<Job> GetEndJobs(Job root)
        {
            return Traverse(
                root,
                node => node.Subjobs is null || node.Subjobs.Count == 0 ? new[] { node } : Array.Empty<Job>(),
                node => node.Subjobs);
        }

        public static IEnumerable<T> GetBinaryTreeValues<T>(BinaryTree<T> root)
        {
            return Traverse(
                root,
                node => node.Left is null && node.Right is null ? new[] { node.Value } : Array.Empty<T>(),
                node => new[] { node.Left, node.Right });
        }

        private static IEnumerable<TResult> Traverse<TNode, TResult>(
            TNode node,
            Func<TNode, IEnumerable<TResult>> getCurrent,
            Func<TNode, IEnumerable<TNode>> getChildren)
        {
            if (node == null)
                yield break;
            foreach (TResult result in getCurrent(node))
                yield return result;

            foreach (TNode child in getChildren(node))
                foreach (TResult result in Traverse(child, getCurrent, getChildren))
                    yield return result;
        }
    }
}