
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using WD.Core.Extensions;

namespace WD.Core.TextGenerators;

public class TopPGenerator : SequentalGenerator
{

    /// <summary>
    /// The minimal cumulative probability of potential next nodes. Should work better than top K
    /// </summary>
    public float TopP
    {
        get;
        set
        {
            if(value < 0f || value > 1f)
            {
                throw new ArgumentException("TopP must be in bounds [0, 1]!");
            }

            field = value;
        }
    } = 0.75f;

    protected override bool TryGetNextToken(IGraph<VertexWeightInfo, EdgeWeightInfo> graph, INode<VertexWeightInfo, EdgeWeightInfo> node, out INode<VertexWeightInfo, EdgeWeightInfo>? next)
    {
        // Maximal size of nodes pool that can be allocated on stack to avoid potential stack overflow
        // (actually it can oevrflow even with limit because we don't know how many space is remaining on the stack)
        const int maxStackAllocatedPoolSize = 256;

        next = null;

        var connectionsCount = node.Connections.Count;
        if(connectionsCount <= 0) return false;

        var neighbors = ArrayPool<INode<VertexWeightInfo, EdgeWeightInfo>>.Shared.Rent(connectionsCount);
        var edges = ArrayPool<EdgeWeightInfo>.Shared.Rent(connectionsCount);
        int neighborsLength;

        if(!node.TryConvertConenctionsToArrays(neighbors, edges, out neighborsLength))
        {
            throw new InvalidOperationException("Allocated enough space for neighbors and edges, but connections didn't fit in them!");
        }

        Array.Sort(edges, neighbors, Comparer<EdgeWeightInfo>.Create((a, b) => b.AppearancesCount.CompareTo(a.AppearancesCount)));

        // This all scheisse is needed because we can't stackalloc array of a managed type (INode)
        // Since all elements inside nextTokensPool are from neigbhors and neighbors is a valid reference,
        // GC won't free them until neighbors become invalid
        var neighborsSpan = neighbors.AsSpan();
        Span<nint> nextTokensPool = neighborsLength <= maxStackAllocatedPoolSize
            ? stackalloc nint[neighborsLength]
            : new nint[neighborsLength];
        
        var minEntrencesCount = (int)Math.Round(edges.Sum((info) => info.AppearancesCount) * TopP);
        var entrencesCount = 0;

        // nextTokensPool.Length is actually the capacity of our pool (because we obviously won't add EVERY token from neigbhors to it
        // so we need a separate length variable
        var addedElementsCount = 0;

        for(int i = 0; i < neighborsLength; i++)
        {
            entrencesCount += edges[i].AppearancesCount;

            nextTokensPool[i] = Unsafe.As<INode<VertexWeightInfo, EdgeWeightInfo>, nint>(ref neighbors[i]);
            addedElementsCount++;

            if(entrencesCount >= minEntrencesCount)
            {
                break;
            }
        }

        if(addedElementsCount <= 0)
        {
            ArrayPool<INode<VertexWeightInfo, EdgeWeightInfo>>.Shared.Return(neighbors);
            ArrayPool<EdgeWeightInfo>.Shared.Return(edges);
            return false;
        }

        var nextToken = nextTokensPool[Random.Next(addedElementsCount)];
        next = Unsafe.As<nint, INode<VertexWeightInfo, EdgeWeightInfo>>(ref nextToken);

        ArrayPool<INode<VertexWeightInfo, EdgeWeightInfo>>.Shared.Return(neighbors);
        ArrayPool<EdgeWeightInfo>.Shared.Return(edges);
        return true;
    }
}