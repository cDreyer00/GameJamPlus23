using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamageCollider : MonoBehaviour
{
    public float        damage;
    public List<string> ignoreList;

    public void IgnoreTeam(string team)
    {
        ignoreList ??= new List<string>();
        ignoreList.Add(team);
    }
    void OnTriggerEnter(Collider other)
    {
        var character = other.GetComponent<Character>();
        if (character) {
            if (ignoreList?.Contains(character.team) is true) return;
            character.Events.OnTakeDamage(damage);
        }
    }
}