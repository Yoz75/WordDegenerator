
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
    /// <param name="seed">the seed of random. -1 means random seed</param>
    /// <returns>a random element of the collection</returns>
    T GetRandom(int seed = -1);
}