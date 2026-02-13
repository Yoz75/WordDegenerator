
using System;
using System.Collections.Generic;
using WD.Core.Extensions;

namespace WD.Core.Tokenizers;

/// <summary>
/// Tokenizer that creates a graph where each node represents a block of text of fixed length between Min and Max.
/// </summary>
public sealed class RandomBlockTokenizer : CashTokenizer
{
    /// <summary>
    /// If input text is not a multiple of <see cref="BlockSize"/>, text will be increased with this character
    /// </summary>
    public char FillChar = ' ';

    public int Min
    {
        get => field;

        set
        {
            if(value < 0) throw new ArgumentException("Min size must be > 0!");

            field = value;
        }
    } = 1;

    public int Max
    {
        get => field;

        set
        {
            if(value < 0) throw new ArgumentException("Max size must be > 0!");

            field = value;
        }
    } = 3;

    protected override IGraph<VertexWeightInfo, EdgeWeightInfo> InternalTokenize(string text)
    {
        Graph<VertexWeightInfo, EdgeWeightInfo> result = [];
        Dictionary<string, Node<VertexWeightInfo, EdgeWeightInfo>> nodesMap = new(StringComparer.Ordinal);
        Node<VertexWeightInfo, EdgeWeightInfo>? previous = null;

        int currentSize = Random.Shared.Next(Min, Max + 1);
        for(int i = 0; i < text.Length - currentSize; i += currentSize)
        {
            Node<VertexWeightInfo, EdgeWeightInfo> current;
            var block = text.Substring(i, currentSize);

            if(!nodesMap.TryGetValue(block, out current!))
            {
                VertexWeightInfo info = new();
                info.Value = block;

                current = new(info);
                nodesMap[block] = current;
            }

            if(previous is not null)
            {
                result.UpdateConnection(previous, current);
            }

            previous = current;
            currentSize = Random.Shared.Next(Min, Max + 1);

            if(i + currentSize > text.Length)
            {
                currentSize = text.Length - i;
            }
        }

        return result;
    }
}
