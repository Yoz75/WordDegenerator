
using System;

namespace WD.Core;

/// <summary>
/// Something, that converts text to <see cref="IGraph{VertexWeightInfo, EdgeWeightInfo}"/>
/// </summary>
public interface ITokenizer
{
    /// <summary>
    /// Convert text to <see cref="IGraph{VertexWeightInfo, EdgeWeightInfo}"/>
    /// </summary>
    /// <param name="text">the text you need to process</param>
    /// <param name="random">the random to be used</param>
    /// <returns>a newly allocated graph</returns>
    public IGraph<VertexWeightInfo, EdgeWeightInfo> Tokenize(string text, Random random);
}
