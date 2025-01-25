using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static partial class Extensions
{
    /// <summary>
    /// returns a random element from the collection. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T RandomElement<T> (this IEnumerable<T> list, System.Random random)
    {
        return list.ElementAt(random.Next(0, list.Count())); 
    }

    public static T RandomElement<T>(this IEnumerable<T> list)
    {
        return list.ElementAt(UnityEngine.Random.Range(0, list.Count()));
    }
}

