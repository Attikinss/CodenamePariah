using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class EnvironmentQuerySystem
{
    public static Query NewQuery(Vector3 origin, float range, int radialDensity = 30, float spacing = 1.5f)
    {
        Query query = new Query();
        int positionRingCount = Mathf.RoundToInt(range / spacing);

        // Generate positions within a range with a set density
        for (int i = 1; i <= positionRingCount; i++)
        {
            for (int j = 0; j < radialDensity; j++)
            {
                // Get position on same y level
                Vector3 offset = Vector3.forward;
                offset = Quaternion.AngleAxis(360.0f / radialDensity * j, Vector3.up) * offset;
                offset *= (spacing * i);
                offset += origin;

                // Check if point is on navmesh, if not place it on closest point
                if (NavMesh.SamplePosition(offset, out NavMeshHit hitInfo, 3.0f, NavMesh.AllAreas))
                {
                    if (hitInfo.position.y <= origin.y + 0.1f)
                        offset = hitInfo.position;
                    else continue;
                }
                else continue;

                // Check if point is inside geometry
                var colliders = Physics.OverlapSphere(offset, 0.5f);
                bool insideGeometry = false;
                foreach (var collider in colliders)
                {
                    if (!collider.gameObject.CompareTag("Ground") || collider.bounds.Contains(offset))
                    {
                        insideGeometry = true;
                        break;
                    }
                }

                if (!insideGeometry)
                    query.AddPosition(offset);
            }
        }

        return query;
    }
}
