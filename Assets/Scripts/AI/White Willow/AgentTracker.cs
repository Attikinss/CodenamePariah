using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteWillow
{
    public static class AgentTracker
    {
        public static List<Agent> Agents { get; private set; } = new List<Agent>();

        public static void AddAgent(Agent agent)
        {
            if (!Agents.Contains(agent))
                Agents.Add(agent);
        }

        public static void RemoveAgent(Agent agent)
        {
            if (Agents.Contains(agent))
                Agents.Remove(agent);
        }

        public static List<Vector3> GetPositions(params Agent[] exclude)
        {
            return Agents.Where(agent => !exclude.Any(excl => excl == agent)).Select(x => x.transform.position).ToList();
        }
    }
}