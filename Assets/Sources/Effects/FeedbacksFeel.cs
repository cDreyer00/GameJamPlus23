using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;


public class FeedbacksFeel : MonoBehaviour
{
    [SerializeField] MMFeedbacks cameraShake;
    public void HitPlayer()
    {
        cameraShake?.PlayFeedbacks();
    }
}
