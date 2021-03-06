using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Query
{
    public string ID { get; }

    public struct QueryValue
    {
        public Vector3 Position;
        public float Score;
        public bool Active;
    }


    public List<EnvironmentQuerySystem.EQSNode> Values { get; private set; }
    //public List<QueryValue> Values { get; private set; }
    private bool m_DataInvalid = false;

    public Query(string id, List<EnvironmentQuerySystem.EQSNode> values = null)
    {
        ID = id;
        Values = values;
    }

    public bool Invalid()
    {
        return m_DataInvalid;
    }

    public bool Empty()
    {
        return Values.Count == 0;
    }

    public void AddPosition(Vector3 position)
    {
        //if (!Values.Any(node => node.Position == position))
        //    Values.Add(new QueryValue() { Position = position, Active = true });
    }

    public void AddNode(EnvironmentQuerySystem.EQSNode node)
    {
        if (!Values.Any(n => n == node))
            Values.Add(node);
    }

    public void RemovePosition(Vector3 position)
    {
        //QueryValue node = Values.First(node => node.Position == position);
        //
        //Values.Remove(node);
    }

    public void RemoveNode(EnvironmentQuerySystem.EQSNode node)
    {
        var nodeToRemove = Values.First(n => n == node);

        Values.Remove(node);
    }

    public void FilterByAngle(Vector3 target, Vector3 facing, float angle)
    {
        Values.RemoveAll(node =>
        {
            float toTarget = Vector3.Angle(node.Position - target, facing);

            return toTarget <= -angle || toTarget >= angle;
        });
    }

    public void FilterByDistance(Vector3 target, float distance, float threshold)
    {
        float sqrThreshold = threshold * threshold;
        float sqrDistance = distance * distance;

        Values.RemoveAll(node =>
        {
            float sqrMag = (target - node.Position).sqrMagnitude;

            return sqrMag <= (sqrDistance - sqrThreshold) &&
                    sqrMag >= (sqrDistance + sqrThreshold);
        });
    }

    public void FilterByClosest(Vector3 target, float distanceToTarget)
    {
        float sqrDistance = distanceToTarget * distanceToTarget;

        Values.RemoveAll(node =>
        {
            float sqrMag = (target - node.Position).sqrMagnitude;
            return sqrMag >= sqrDistance;
        });
    }

    public void FilterByFurthest(Vector3 target, float distanceToTarget)
    {
        float sqrDistance = distanceToTarget * distanceToTarget;

        Values.RemoveAll(node =>
        {
            float sqrMag = (target - node.Position).sqrMagnitude;
            return sqrMag <= sqrDistance;
        });
    }

    public void ShuffleValues()
    {
        var count = Values.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = Random.Range(i, count);
            var tmp = Values[i];
            Values[i] = Values[r];
            Values[r] = tmp;
        }
    }
}
