using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace Sources.Enemy
{
    public static class NavHelper
    {
        public static Vector3 GetRandomPoint(this NavMeshSurface surface)
        {
            Bounds  bounds = surface.navMeshData.sourceBounds;
            Vector3 randomPosition;
            randomPosition.x = Random.Range(bounds.max.x, bounds.min.x);
            randomPosition.z = Random.Range(bounds.max.z, bounds.min.z);
            randomPosition.y = bounds.max.y;
            Begin:
            if (!NavMesh.SamplePosition(randomPosition, out var hit, Mathf.Infinity, NavMesh.AllAreas)) {
                randomPosition.x = Random.Range(bounds.max.x, bounds.min.x);
                randomPosition.y = Random.Range(bounds.max.z, bounds.min.z);
                goto Begin;
            }
            return hit.position;
        }
    }
}