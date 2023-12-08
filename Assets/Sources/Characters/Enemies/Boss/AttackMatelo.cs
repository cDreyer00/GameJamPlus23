using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using Sources;

public class AttackMatelo : MonoBehaviour
{
    public MMFeedbacks marteloFeel, dash, dashOff;
    public void Martelo()
    {
        marteloFeel.OrNull()?.PlayFeedbacks();
    }

    public void Dash()
    {
        dash.OrNull()?.PlayFeedbacks();
    }

    public void DashOff()
    {
        dashOff.OrNull()?.PlayFeedbacks();
    }
}
