
using System;
using System.Linq;
using System.Text;

namespace WD.Core.TextGenerators;

public class DefaultTextGenerator : ITextGenerator
{
    /// <summary>
    /// Use | as a separator between tokens?
    /// </summary>
    public bool UseSeparator = false;

    /// <summary>
    /// How much tokens generate? (1 node = 1 token)
    /// </summary>
    public int OutputSize
    {
        get => field;
        set
        {
            if(value <= 0)
            {
                throw new ArgumentException("GeneratedTokensCount must be > 0!");
            }

            field = value;
        }
    } = 50;

    /// <summary>
    /// Algorythm will choose a random node of [value] "weightful" neighbors of current node (see <see cref="EdgeWeightInfo.EnterWeight"/>
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

    /// <summary>
    /// The chance of a random token instead of neighbor of current node
    /// </summary>
    public float RandomTokenChance
    {
        get => field;

        set => field = value;
    }

    public string Generate(IGraph<VertexWeightInfo, EdgeWeightInfo> graph)
    {
        StringBuilder sb = new();
        INode<VertexWeightInfo, EdgeWeightInfo> current = graph.GetRandom();

        for(int i = 0; i < OutputSize; i++)
        {
            sb.Append(current.Value.Value);
            if(UseSeparator) sb.Append("|");

            if(Random.Shared.NextDouble() < RandomTokenChance || !TrySelectRandomNeighbor(current, out current!))
            {
                current = graph.GetRandom();
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Try to select random neighbor of <paramref name="node"/>. If couldn't, return false and set <paramref name="neighbor"/> as null
    /// </summary>
    /// <returns> true if <paramref name="neighbor"/> has value or false</returns>
    private bool TrySelectRandomNeighbor(INode<VertexWeightInfo, EdgeWeightInfo> node, out INode<VertexWeightInfo, EdgeWeightInfo>? neighbor)
    {
        neighbor = null;
        if(node.Connections.Count <= 0) return false;

        var nodes = node.Connections.ToList();
        nodes.Sort((a, b) => a.Value.EnterWeight.CompareTo(b.Value.EnterWeight));

        var topK = NormalizeTopKey(nodes.Count);

        // Random.Next top border is exclusive so everything is normal
        neighbor = nodes[Random.Shared.Next(topK)].Key;
        return true;
    }

    private int NormalizeTopKey(int maxValue)
    {
        if(TopK > maxValue) return maxValue;
        else return TopK;
    }
}
