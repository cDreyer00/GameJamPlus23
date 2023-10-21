using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CDreyer
{
    public interface ISavable<T>
    {
        string FileName { get; }
        void Save();

        T Load();
        T GetBase();
    }
}