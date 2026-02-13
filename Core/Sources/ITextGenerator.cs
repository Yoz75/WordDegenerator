
namespace WD.Core;

/// <summary>
/// Something that makes text from a graph
/// </summary>
public interface ITextGenerator
{
    /// <summary>
    /// Generate a text from a graph
    /// </summary>
    /// <param name="graph"></param>
    /// <returns></returns>
    public string Generate(IGraph<VertexWeightInfo, EdgeWeightInfo> graph);
}
