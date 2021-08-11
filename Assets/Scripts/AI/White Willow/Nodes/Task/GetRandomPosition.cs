using UnityEngine;
using UnityEngine.AI;

namespace WhiteWillow.Nodes
{
    [Category("Tasks")]
    public class GetRandomPosition : Task
    {
        public enum RangeValueType { Fixed, Varying }
        
        [Tooltip("Determines the final distance of the random position.\nFixed = Radius never changes\nVarying = Radius slightly flucuates around the original value")]
        public RangeValueType RangeType = RangeValueType.Fixed;
        
        [Min(1.0f)]
        [Tooltip("The distance of the random position from the agent.")]
        public float Radius = 5.0f;

        private TaskDebugView m_DebugView;

        protected override void OnEnter()
        {
            if (m_DebugView == null)
                m_DebugView = new TaskDebugView();

            if (RangeType == RangeValueType.Varying)
                Radius += Random.Range(-Radius, Radius) / 2;
        }

        protected override void OnExit()
        {

        }

        protected override NodeResult OnTick()
        {
            var offset = Random.insideUnitSphere * Radius;

            m_DebugView.Position = new Vector3(offset.x, 0.0f, offset.y);

            if (Owner.Agent.SetDestination(m_DebugView.Position))
                return NodeResult.Success;
            else
            {
                if (Interruptable)
                    return NodeResult.Failure;
                else
                    return NodeResult.Running;
            }
        }
    }

    public class TaskDebugView : UnityEditor.Editor
    {
        public Vector3 Position { get; set; } = Vector3.zero;

        private void OnSceneGUI()
        {
            UnityEditor.Handles.DrawSolidArc(Position, Vector3.up, Vector3.zero, 360.0f, 0.5f);
        }
    }
}