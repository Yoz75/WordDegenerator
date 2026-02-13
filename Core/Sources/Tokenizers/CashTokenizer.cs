
namespace WD.Core.Tokenizers;

/// <summary>
/// A tokenizer that uses cash and won't do tokenization if input is same
/// </summary>
public abstract class CashTokenizer : ITokenizer
{
    private string? CashedInput;

    // TODO: Actually, user can modify this by modifying graph, returned by Tokenize method. Fix it
    private IGraph<VertexWeightInfo, EdgeWeightInfo>? CashedGraph;

    /// <summary>
    /// Clear the cash
    /// </summary>
    public void ClearCash()
    {
        CashedInput = null;
        CashedGraph = null;
    }

    public IGraph<VertexWeightInfo, EdgeWeightInfo> Tokenize(string text)
    {
        if(CashedInput is null || CashedGraph is null || CashedInput.GetHashCode() != text.GetHashCode())
        {
            CashedInput = text;
            CashedGraph = InternalTokenize(text);
        }

        return CashedGraph;
    }

    /// <summary>
    /// Force new tokenization and update cash. This method never returns cashed value
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public IGraph<VertexWeightInfo, EdgeWeightInfo> TokenizeForced(string text)
    {
        CashedInput = text;
        CashedGraph = InternalTokenize(text);

        return CashedGraph;
    }

    protected abstract IGraph<VertexWeightInfo, EdgeWeightInfo> InternalTokenize(string text);
}
