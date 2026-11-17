
using System;
using System.Buffers;
using System.Linq;

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
    } = 0.35f;

    protected override bool TryGetNextToken(IGraph<VertexWeightInfo, EdgeWeightInfo> graph, INode<VertexWeightInfo, EdgeWeightInfo> node, out INode<VertexWeightInfo, EdgeWeightInfo>? next, Random random)
    {
        // Maximal size of nodes pool that can be allocated on stack to avoid potential stack overflow
        // (actually it can oevrflow even with limit because we don't know how many space is remaining on the stack)
        const int maxStackAllocatedPoolSize = 256;

        next = null;

        var connectionsCount = node.Connections.Count;
        if(connectionsCount <= 0) return false;

        var connections = node.Connections.ToArray();        
        Array.Sort(connections, (a, b) => b.Value.AppearancesCount.CompareTo(a.Value.AppearancesCount));

        int minimalEntrencesCount = (int) Math.Round(connections.Sum((connection) => connection.Value.AppearancesCount) * TopP);

        int entrencesCount = 0;
        int poolLength = 0;

        for(int i = 0; i < connections.Length; i++)
        {
            entrencesCount += connections[i].Value.AppearancesCount;
            poolLength = i;
            if(entrencesCount >= minimalEntrencesCount)
            {
                break;
            }
        }

        next = connections[random.Next(poolLength)].Key;
        return true;
    }
}