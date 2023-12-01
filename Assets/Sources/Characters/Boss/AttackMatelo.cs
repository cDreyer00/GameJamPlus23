using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class AttackMatelo : MonoBehaviour
{
    [SerializeField] MMFeedbacks marteloFeel;
    public void Martelo()
    {
        marteloFeel?.PlayFeedbacks();
    }
}
