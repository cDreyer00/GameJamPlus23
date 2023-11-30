using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;


public class FeedbacksFeel : MonoBehaviour
{
    [SerializeField] MMFeedbacks cameraShake, enemy, freeze;
    public void HitPlayer()
    {
        cameraShake?.PlayFeedbacks();
    }

    public void HitEnemy()
    {
        enemy?.PlayFeedbacks();
    }

    public void Freeze()
    {
        freeze?.PlayFeedbacks();
    }


}
