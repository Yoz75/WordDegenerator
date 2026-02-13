
using System.Diagnostics;

namespace WD.Core;

[DebuggerDisplay("{Value}")]
public struct VertexWeightInfo
{
    public string Value;
}

[DebuggerDisplay("{EnterWeight}")]
public struct EdgeWeightInfo
{
    /// <summary>
    /// Weight of entering in this node. This value can be any number, but greater = greater chance to enter
    /// </summary>
    public float EnterWeight;
}
