using System;
using System.Collections.Generic;
using System.Linq;

namespace Delegates.TreeTraversal
{
    public static class Traversal
    {
        public static IEnumerable<Product> GetProducts(ProductCategory root)
        {
            return Traverse<ProductCategory, IEnumerable<Product>>(
                root,
                node => node.Products != null && node.Products.Count > 0,
                node => node.Products,
                node => node.Categories)
                .SelectMany(x => x);
        }

        public static IEnumerable<Job> GetEndJobs(Job root)
        {
            return Traverse(
                root,
                node => node.Subjobs != null && node.Subjobs.Count == 0,
                node => node,
                node => node.Subjobs);
        }

        public static IEnumerable<T> GetBinaryTreeValues<T>(BinaryTree<T> root)
        {
            return Traverse(
                root,
                node => node.Left is null && node.Right is null,
                node => node.Value,
                node => new[] { node.Left, node.Right });
        }

        private static IEnumerable<TResult> Traverse<TNode, TResult>(
            TNode node,
            Func<TNode, bool> resultFilter,
            Func<TNode, TResult> resultSelector,
            Func<TNode, IEnumerable<TNode>> childrenSelector)
        {
            if (node == null)
                yield break;

            if (resultFilter(node))
                yield return resultSelector(node);

            foreach (TNode child in childrenSelector(node))
                foreach (TResult result in Traverse(child, resultFilter,
                    resultSelector, childrenSelector))
                    yield return result;
        }
    }
}