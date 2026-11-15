namespace WD.Core.Extensions;

public static class IGraphExtensions
{
    extension(IGraph<VertexWeightInfo, EdgeWeightInfo> graph)
    {
        /// <summary>
        /// Update connection between two nodes. If connection exists, increment EnterWeight by 1. Otherwise, create new connection with EnterWeight = 1.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public void UpdateConnection(INode<VertexWeightInfo, EdgeWeightInfo> first, INode<VertexWeightInfo, EdgeWeightInfo> second)
        {
            if(first.Connections.TryGetValue(second, out var value))
            {
                value.AppearancesCount++;
                first.Connections[second] = value;
            }
            else
            {
                EdgeWeightInfo info = new()
                {
                    AppearancesCount = 1
                };

                graph.AddOrientedEdge(first, second, info);
            }
        }
    }
}