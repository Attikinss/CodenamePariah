using UnityEngine;

namespace WhiteWillow.Nodes
{
    ///<summary>The base node that all action/leaf nodes inherit from.</summary>
    public abstract class Task : BaseNode
    {
        [Tooltip("Defines whether or not this node can be interrupted during the running state.")]
        public bool Interruptable = false;
    }
}