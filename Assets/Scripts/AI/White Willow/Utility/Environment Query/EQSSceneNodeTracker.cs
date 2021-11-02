using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EQSSceneNodeTracker
{
    public static List<EQSSceneNode> Nodes { get; private set; } = new List<EQSSceneNode>();

    public static void AddNode(EQSSceneNode node)
    {
        if (!Nodes.Contains(node))
            Nodes.Add(node);
    }

    public static void RemoveNode(EQSSceneNode node)
    {
        if (Nodes.Contains(node))
            Nodes.Remove(node);
    }

    public static IEnumerable<EnvironmentQuerySystem.EQSNode> GetNodes(params EnvironmentQuerySystem.EQSNode[] exclude)
    {
        return Nodes.Where(node => !exclude.Any(excl => excl == node.EQSNode)).Select(x => x.EQSNode);
    }

    public static IEnumerable<EnvironmentQuerySystem.EQSNode> GetNodesInRange(Vector3 position, float range, params EnvironmentQuerySystem.EQSNode[] exclude)
    {
        return new List<EnvironmentQuerySystem.EQSNode>(Nodes.Where(node =>
        {
            if (!exclude.Any(excl => excl == node.EQSNode))
                if (!node.EQSNode.Taken)
                    if ((node.EQSNode.Position - position).sqrMagnitude < range * range)
                        return true;

            return false;
        }).Select(x => x.EQSNode));
    }
}
