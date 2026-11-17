using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WD.Core;

[DebuggerDisplay("Value = {Value}")]
public sealed class Node<TVertexWeight, TEdgeWeight> : INode<TVertexWeight, TEdgeWeight>
{
    public Dictionary<INode<TVertexWeight, TEdgeWeight>, TEdgeWeight> Connections
    {
        get;
        set;
    } = [];

    public TVertexWeight Value { get; set; }

    public Node(TVertexWeight value)
    {
        Value = value;
    }

    public void AddNeighbor(INode<TVertexWeight, TEdgeWeight> neighbor, TEdgeWeight edgeWeight) => Connections[neighbor] = edgeWeight;
}

public sealed class Graph<TVertexWeight, TEdgeWeight> : IGraph<TVertexWeight, TEdgeWeight>
{
    private readonly HashSet<INode<TVertexWeight, TEdgeWeight>> Nodes = [];

    public int Count => Nodes.Count;

    public bool IsReadOnly => throw new NotImplementedException();

    public void Add(INode<TVertexWeight, TEdgeWeight> node)
    {
        Nodes.Add(node);
    }

    public void AddOrientedEdge(INode<TVertexWeight, TEdgeWeight> from, INode<TVertexWeight, TEdgeWeight> to, TEdgeWeight weight)
    {
        Nodes.Add(from);
        Nodes.Add(to);

        from.AddNeighbor(to, weight);
    }

    public void AddUnorientedEdge(INode<TVertexWeight, TEdgeWeight> first, INode<TVertexWeight, TEdgeWeight> second, TEdgeWeight weightFirst, TEdgeWeight weightSecond)
    {
        Nodes.Add(first);
        Nodes.Add(second);

        first.AddNeighbor(second, weightFirst);
        second.AddNeighbor(first, weightSecond);
    }

    public IEnumerator<INode<TVertexWeight, TEdgeWeight>> GetEnumerator()
    {
        return Nodes.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public INode<TVertexWeight, TEdgeWeight> GetRandom(Random? random = null)
    {
        random = (random is null) ? new Random() : random;
        return Nodes.ElementAt(random.Next(Nodes.Count));
    }

    public void Clear()
    {
        Nodes.Clear();
    }

    public bool Contains(INode<TVertexWeight, TEdgeWeight> item)
    {
        return Nodes.Contains(item);
    }

    public void CopyTo(INode<TVertexWeight, TEdgeWeight>[] array, int arrayIndex)
    {
        Nodes.CopyTo(array, arrayIndex);
    }

    public bool Remove(INode<TVertexWeight, TEdgeWeight> item)
    {
        if(Nodes.TryGetValue(item, out var node))
        {
            foreach(var neighbor in node.Connections)
            {
                neighbor.Key.Connections.Remove(item);
            }

            node.Connections.Clear();
        }

        return false;
    }
}
