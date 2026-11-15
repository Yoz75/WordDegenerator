using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace WD.Core.Extensions;

public static class INodeExtensions
{
    extension<T, U>(INode<T, U> node)
    {
        /// <summary>
        /// Convert node's connections to arrays of nodes and edge infos
        /// </summary>
        /// <param name="length">length of both nodes and edge infos. Equals to node.Connections.Count</param>
        /// <returns>true if converted connections to arrays and false if one of arrays doesn't have enough space</returns>
        public bool TryConvertConenctionsToArrays(INode<T, U>[] nodes, U[] edgeInfos, out int length)
        {
            var connectionsCount = node.Connections.Count;
            length = connectionsCount;

            if(nodes.Length < connectionsCount || edgeInfos.Length < connectionsCount) 
                return false;

            int i = 0;
            foreach(var pair in node.Connections)
            {
                nodes[i] = pair.Key;
                edgeInfos[i] = pair.Value;

                i++;
            }

            return true;
        }
    }
}
