using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;


public class FeedbacksFeel : MonoBehaviour
{
    [SerializeField] MMFeedbacks cameraShake, freeze;
    public void HitPlayer()
    {
        cameraShake?.PlayFeedbacks();
    }

    public void HitEnemy(MMFeedbacks enemy)
    {
        enemy?.PlayFeedbacks();
    }

    public void Freeze()
    {
        freeze?.PlayFeedbacks();
    }
}
