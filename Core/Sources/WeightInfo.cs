
using System.Diagnostics;

namespace WD.Core;

[DebuggerDisplay("{Value}")]
public struct VertexWeightInfo
{
    public string Value;
}

[DebuggerDisplay("{AppearancesCount}")]
public struct EdgeWeightInfo
{
    /// <summary>
    /// Count of times this edge appeares
    /// </summary>
    public int AppearancesCount;
}
