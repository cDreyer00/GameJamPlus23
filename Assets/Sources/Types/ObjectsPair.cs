using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ObjectsPair<T1, T2>
{
    [SerializeField] T1 A;
    [SerializeField] T2 B;

    void Test()
    {
        KeyValuePair<string, int> si = new("2", 3);

    }
}