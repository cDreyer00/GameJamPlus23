using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using Sources;

public class AttackMatelo : MonoBehaviour
{
    public MMFeedbacks marteloFeel;
    public void Martelo()
    {
        marteloFeel.OrNull()?.PlayFeedbacks();
    }
}
