using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Battle : ScriptableObject
{
    [SerializeField] Wave[] waves;
    public Wave[] Waves => waves;
}