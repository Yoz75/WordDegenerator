using System;
using System.Linq;

namespace WD.Core.TextGenerators;

public class TopKGenerator : SequentalGenerator
{

    /// <summary>
    /// Algorythm will choose a random node of [value] "weightful" neighbors of current node (see <see cref="EdgeWeightInfo.AppearancesCount"/>
    /// </summary>
    public int TopK
    {
        get => field;

        set
        {
            if(value <= 0)
            {
                throw new ArgumentException("TopK must be > 0!");
            }

            field = value;
        }
    } = 5;

    private int NormalizeTopKey(int maxValue)
    {
        if(TopK > maxValue) return maxValue;
        else return TopK;
    }

    protected override bool TryGetNextToken(IGraph<VertexWeightInfo, EdgeWeightInfo> graph, INode<VertexWeightInfo, EdgeWeightInfo> node, out INode<VertexWeightInfo, EdgeWeightInfo> next, Random random)
    {
        next = null;
        if(node.Connections.Count <= 0) return false;

        var nodes = node.Connections.ToList();
        nodes.Sort((a, b) => a.Value.AppearancesCount.CompareTo(b.Value.AppearancesCount));

        var topK = NormalizeTopKey(nodes.Count);

        // Random.Next top border is exclusive so everything is normal
        next = nodes[random.Next(topK)].Key;
        return true;
    }
}
