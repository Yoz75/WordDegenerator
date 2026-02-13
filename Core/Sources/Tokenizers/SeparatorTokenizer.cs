using System;
using System.Collections.Generic;
using WD.Core.Extensions;

namespace WD.Core.Tokenizers;

/// <summary>
/// Tokenizer that splits text based on specified separator.
/// </summary>
public sealed class SeparatorTokenizer : CashTokenizer
{
    public char Separator = ' ';
    protected override IGraph<VertexWeightInfo, EdgeWeightInfo> InternalTokenize(string text)
    {
        string[] tokens = text.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

        Graph<VertexWeightInfo, EdgeWeightInfo> result = [];
        Dictionary<string, Node<VertexWeightInfo, EdgeWeightInfo>> nodesMap = new(StringComparer.Ordinal);

        Node<VertexWeightInfo, EdgeWeightInfo>? previous = null;
        foreach(var token in tokens)
        {
            if(!nodesMap.TryGetValue(token, out var current))
            {
                VertexWeightInfo weight = new() { Value = token + Separator };
                current = new Node<VertexWeightInfo, EdgeWeightInfo>(weight);

                nodesMap[token] = current;
            }

            if(previous is not null)
            {
                result.UpdateConnection(previous, current);
            }

            previous = current;
        }

        return result;
    }
}
