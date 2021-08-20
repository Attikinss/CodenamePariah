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

        protected override void OnEnter()
        {
            if (RangeType == RangeValueType.Varying)
                Radius += Random.Range(-Radius, Radius) / 2;
        }

        protected override void OnExit()
        {

        }

        protected override NodeResult OnTick()
        {
            var offset = Random.insideUnitSphere * Radius;

            if (Owner.Agent.SetDestination(offset))
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
}