using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngularProjectile : Projectile
{
    [SerializeField] AnimationCurve yAxisTrajectory;
    protected override void Move(float step)
    {
        var position = _transform.position;
        var forward = _transform.forward;

        var maxSqrDistance = Vector3.SqrMagnitude(_anchor - Target);
        var sqrDistance = Vector3.SqrMagnitude(Target - position);
        var magnitude01 = Ranges.Map01(sqrDistance, 0, maxSqrDistance);

        //print(magnitude01);

        float eval = yAxisTrajectory.Evaluate(magnitude01);
        position.x += step * forward.x;
        position.z += step * forward.z;
        position.y = _anchor.y * (1 + eval);
        _transform.position = position;
    }
}
