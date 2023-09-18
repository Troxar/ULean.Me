using System;
using System.Globalization;

namespace FluentApi.Graph
{
    public class DotGraphBuilder
    {
        private readonly Graph _graph;

        private DotGraphBuilder(Graph graph)
        {
            _graph = graph;
        }

        public static DotGraphBuilder DirectedGraph(string name)
        {
            var graph = new Graph(name, true, false);
            return new DotGraphBuilder(graph);
        }

        public static DotGraphBuilder UndirectedGraph(string name)
        {
            var graph = new Graph(name, false, false);
            return new DotGraphBuilder(graph);
        }

        public NodeBuilder AddNode(string name)
        {
            return new NodeBuilder(this, _graph.AddNode(name));
        }

        public EdgeBuilder AddEdge(string sourceNode, string destinationNode)
        {
            return new EdgeBuilder(this, _graph.AddEdge(sourceNode, destinationNode));
        }

        public string Build()
        {
            return _graph.ToDotFormat();
        }
    }

    #region ItemBuilder

    public abstract class ItemBuilder<TItem, TAttributeBuilder>
    {
        protected readonly DotGraphBuilder _graphBuilder;
        protected readonly TItem _item;

        public ItemBuilder(DotGraphBuilder graphBuilder, TItem item)
        {
            _graphBuilder = graphBuilder;
            _item = item;
        }

        public NodeBuilder AddNode(string name)
        {
            return _graphBuilder.AddNode(name);
        }

        public EdgeBuilder AddEdge(string sourceNode, string destinationNode)
        {
            return _graphBuilder.AddEdge(sourceNode, destinationNode);
        }

        public string Build()
        {
            return _graphBuilder.Build();
        }

        public abstract DotGraphBuilder With(Action<TAttributeBuilder> action);
    }

    public class NodeBuilder : ItemBuilder<GraphNode, NodeAttributeBuilder>
    {
        public NodeBuilder(DotGraphBuilder graphBuilder, GraphNode item)
            : base(graphBuilder, item) { }

        public override DotGraphBuilder With(Action<NodeAttributeBuilder> action)
        {
            action(new NodeAttributeBuilder(_item));
            return _graphBuilder;
        }
    }

    public class EdgeBuilder : ItemBuilder<GraphEdge, EdgeAttributeBuilder>
    {
        public EdgeBuilder(DotGraphBuilder graphBuilder, GraphEdge item)
            : base(graphBuilder, item) { }

        public override DotGraphBuilder With(Action<EdgeAttributeBuilder> action)
        {
            action(new EdgeAttributeBuilder(_item));
            return _graphBuilder;
        }
    }

    #endregion

    #region AttributeBuilder

    public abstract class AttributeBuilder<TItem, TBuilder>
    {
        protected readonly TItem _item;

        public AttributeBuilder(TItem item)
        {
            _item = item;
        }

        protected string ConvertValue<TValue>(TValue value)
        {
            return Convert
                .ToString(value, new CultureInfo("en-US"))
                .ToLower();
        }

        protected abstract TBuilder SetAttribute<TValue>(string attribute, TValue value);

        public TBuilder Color(string color)
        {
            return SetAttribute("color", color);
        }

        public TBuilder FontSize(int size)
        {
            return SetAttribute("fontsize", size);
        }

        public TBuilder Label(string label)
        {
            return SetAttribute("label", label);
        }
    }

    public class NodeAttributeBuilder : AttributeBuilder<GraphNode, NodeAttributeBuilder>
    {
        public NodeAttributeBuilder(GraphNode node) : base(node) { }

        protected override NodeAttributeBuilder SetAttribute<TValue>(string attribute, TValue value)
        {
            _item.Attributes[attribute] = ConvertValue(value);
            return this;
        }

        public NodeAttributeBuilder Shape(NodeShape shape)
        {
            return SetAttribute("shape", shape);
        }
    }

    public class EdgeAttributeBuilder : AttributeBuilder<GraphEdge, EdgeAttributeBuilder>
    {
        public EdgeAttributeBuilder(GraphEdge edge) : base(edge) { }

        protected override EdgeAttributeBuilder SetAttribute<TValue>(string attribute, TValue value)
        {
            _item.Attributes[attribute] = ConvertValue(value);
            return this;
        }

        public EdgeAttributeBuilder Weight(double weight)
        {
            return SetAttribute("weight", weight);
        }
    }

    #endregion

    public enum NodeShape
    {
        Box = 1,
        Ellipse = 2
    }
}