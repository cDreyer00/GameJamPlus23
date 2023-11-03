using UnityEngine;
using UnityEngine.AI;

public static class NavMeshRandom
{
    public static Vector3 InsideBounds(Bounds bounds)
    {
        Vector3 randomPosition;
        randomPosition.x = Random.Range(bounds.max.x, bounds.min.x);
        randomPosition.z = Random.Range(bounds.max.z, bounds.min.z);
        randomPosition.y = bounds.max.y;
    Begin:
        if (!NavMesh.SamplePosition(randomPosition, out var hit, Mathf.Infinity, NavMesh.AllAreas))
        {
            randomPosition.x = Random.Range(bounds.max.x, bounds.min.x);
            randomPosition.y = Random.Range(bounds.max.z, bounds.min.z);
            goto Begin;
        }
        return hit.position;
    }
    public static Vector3 InsideSphere(Vector3 point, float maxRadius)
    {
        var radius = Random.Range(1f, maxRadius);
        var unitSphere = Random.insideUnitSphere * radius;
        var randomPosition = point + unitSphere;
    Begin:
        if (!NavMesh.SamplePosition(randomPosition, out var hit, Mathf.Infinity, NavMesh.AllAreas))
        {
            randomPosition = point + unitSphere;
            goto Begin;
        }
        return hit.position;
    }
    public static Vector2 InsideCircle(Vector2 point, float maxRadius)
    {
        var radius = Random.Range(1f, maxRadius);
        var unitCircle = Random.insideUnitCircle * radius;
        var randomPosition = point + unitCircle;
    Begin:
        if (!NavMesh.SamplePosition(randomPosition, out var hit, Mathf.Infinity, NavMesh.AllAreas))
        {
            randomPosition = point + unitCircle;
            goto Begin;
        }
        return hit.position;
    }
}