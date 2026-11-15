
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
    /// The chance of a random token instead of neighbor of current node
    /// </summary>
    public float RandomTokenChance
    {
        get => field;

        set => field = value;
    }

    /// <summary>
    /// The seed of generator. Same seed for same generator always produces same result
    /// </summary>
    public int Seed
    {
        get;
        set
        {
            if(value == -1)
            {
                field = Random.Shared.Next();
                Random = new(field);
            }
        }
    } = -1;

    protected Random Random = new();

    public string Generate(IGraph<VertexWeightInfo, EdgeWeightInfo> graph)
    {
        StringBuilder sb = new();
        INode<VertexWeightInfo, EdgeWeightInfo> current = graph.GetRandom();

        for(int i = 0; i < OutputSize; i++)
        {
            sb.Append(current.Value.Value);
            if(UseSeparator) sb.Append('|');

            var isRandom = Random.Shared.NextDouble() < RandomTokenChance;
            var isChoseNext = TryGetNextToken(graph, current, out var next);
            if(isRandom || !isChoseNext)
            {
                current = graph.GetRandom(Seed);
                continue;
            }

            current = next!;
        }

        return sb.ToString();
    }

    protected abstract bool TryGetNextToken(IGraph<VertexWeightInfo, EdgeWeightInfo> graph, 
        INode<VertexWeightInfo, EdgeWeightInfo> node, out INode<VertexWeightInfo, EdgeWeightInfo>? next);
}