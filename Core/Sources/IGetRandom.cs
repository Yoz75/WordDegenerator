
using System;
using System.Collections.Generic;

namespace WD.Core;

/// <summary>
/// Some collection that allows you to get random element
/// </summary>
public interface IGetRandom<T> : IEnumerable<T>
{
    /// <summary>
    /// Get random element of collection. 
    /// </summary>
    /// <param name="random">the random to be used. null means default random</param>
    /// <returns>a random element of the collection</returns>
    T GetRandom(Random? random = null);
}