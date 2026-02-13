namespace WD.Core.Extensions;

public static class GraphUtils
{
    extension(Graph<VertexWeightInfo, EdgeWeightInfo> graph)
    {
        /// <summary>
        /// Update connection between two nodes. If connection exists, increment EnterWeight by 1. Otherwise, create new connection with EnterWeight = 1.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public void UpdateConnection(Node<VertexWeightInfo, EdgeWeightInfo> first, Node<VertexWeightInfo, EdgeWeightInfo> second)
        {
            if(first.Connections.TryGetValue(second, out var value))
            {
                value.EnterWeight++;
                first.Connections[second] = value;
            }
            else
            {
                EdgeWeightInfo info = new()
                {
                    EnterWeight = 1
                };

                graph.AddOrientedEdge(first, second, info);
            }
        }
    }
}