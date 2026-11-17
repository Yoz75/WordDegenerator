
using System;
using System.Linq;
using System.Text;

namespace WD.Core.TextGenerators;

/// <summary>
/// A text generator, that generates tokens one by one or random if can't continue
/// </summary>
public abstract class SequentalGenerator : ITextGenerator
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
        get;
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
    /// The chance of a random token instead of neighbor of current node
    /// </summary>
    public float RandomTokenChance
    {
        get;
        set;
    }

    /// <summary>
    /// The seed of generator. Same seed for same generator always produces same result.
    /// Value -1 means random seed every generation
    /// </summary>
    public int Seed = -1;

    public string Generate(IGraph<VertexWeightInfo, EdgeWeightInfo> graph)
    {
        var random = Seed == -1 ? new Random() : new Random(Seed);
        StringBuilder sb = new();
        INode<VertexWeightInfo, EdgeWeightInfo> current = graph.GetRandom(random);

        for(int i = 0; i < OutputSize; i++)
        {
            sb.Append(current.Value.Value);
            if(UseSeparator) sb.Append('|');

            var isRandom = random.NextDouble() < RandomTokenChance;
            var isChoseNext = TryGetNextToken(graph, current, out var next, random);
            if(isRandom || !isChoseNext)
            {
                current = graph.GetRandom(random);
                continue;
            }

            current = next!;
        }

        return sb.ToString();
    }

    protected abstract bool TryGetNextToken(IGraph<VertexWeightInfo, EdgeWeightInfo> graph, 
        INode<VertexWeightInfo, EdgeWeightInfo> node, out INode<VertexWeightInfo, EdgeWeightInfo>? next, Random random);
}