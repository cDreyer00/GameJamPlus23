using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sources.Systems
{
    public sealed class WaveManager : MonoBehaviour
    {
        public int               currentWave;
        public WaveInformation[] wavesInformation;
    }

    [Serializable]
    public struct WaveInformation
    {
        public int           waveNumber;
        public int           enemyCount;
        public MonoBehaviour prefab;
    }
}