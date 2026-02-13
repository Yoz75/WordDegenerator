
using System;
using System.Collections.Generic;
using System.Text;
using WD.Core.Extensions;

namespace WD.Core.Tokenizers;

/// <summary>
/// Tokenizer that creates a graph where each node represents a block of text of fixed length.
/// </summary>
public sealed class BlockTokenizer : CashTokenizer
{
    /// <summary>
    /// If input text is not a multiple of <see cref="BlockSize"/>, text will be increased with this character
    /// </summary>
    public char FillChar = ' ';

    public int BlockSize
    {
        get => field;

        set
        {
            if(value < 0) throw new ArgumentException("Block size must be > 0!");

            field = value;
        }
    } = 3;

    protected override IGraph<VertexWeightInfo, EdgeWeightInfo> InternalTokenize(string text)
    {
        NormalizeText(ref text);

        Graph<VertexWeightInfo, EdgeWeightInfo> result = [];
        Dictionary<string, Node<VertexWeightInfo, EdgeWeightInfo>> nodesMap = new(StringComparer.Ordinal);
        Node<VertexWeightInfo, EdgeWeightInfo>? previous = null;

        for(int i = 0; i < text.Length - BlockSize; i += BlockSize)
        {
            Node<VertexWeightInfo, EdgeWeightInfo> current;
            var block = text.Substring(i, BlockSize);

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
        }

        return result;
    }

    private void NormalizeText(ref string text)
    {
        if(text.Length % BlockSize != 0)
        {
            var paddingLength = text.Length % BlockSize;
            StringBuilder sb = new();

            /// Functional style would be very ugly
            for(int i = 0; i < paddingLength; i++)
            {
                sb.Append(FillChar);
            }

            text += sb.ToString();
        }
    }
}
