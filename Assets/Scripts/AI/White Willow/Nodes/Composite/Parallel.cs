using System.Collections.Generic;
using UnityEngine;

namespace WhiteWillow.Nodes
{
    [Category("Composite")]
    public sealed class Parallel : Composite
    {
        private List<NodeResult> m_ChildrenToExecute = new List<NodeResult>();

        protected override void OnEnter()
        {
            m_ChildrenToExecute.Clear();

            Children.ForEach(a => m_ChildrenToExecute.Add(NodeResult.Running));
        }

        protected override void OnExit()
        {
            if (State != NodeResult.Running)
                ClearRunningNode();
        }

        protected override NodeResult OnTick()
        {
            bool running = false;

            for (int i = 0; i < m_ChildrenToExecute.Count; ++i)
            {
                if (m_ChildrenToExecute[i] == NodeResult.Running)
                {
                    NodeResult result = Children[i].Tick();
                    if (result == NodeResult.Failure)
                    {
                        AbortRunningChildren();
                        return result;
                    }

                    if (result == NodeResult.Running)
                        running = true;

                    m_ChildrenToExecute[i] = result;
                }
            }

            return running ? NodeResult.Running : NodeResult.Success;
        }

        void AbortRunningChildren()
        {
            for (int i = 0; i < m_ChildrenToExecute.Count; ++i)
            {
                if (m_ChildrenToExecute[i] == NodeResult.Running)
                    Children[i].Abort();
            }
        }
    }
}