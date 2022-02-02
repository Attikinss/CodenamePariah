using UnityEngine;
using System.Collections;

namespace WhiteWillow.Nodes
{
    ///<summary>The root node of a behaviour tree. All computation begins from this node.</summary>
    public sealed class Root : BaseNode
    {
        public override string IconPath { get; } = "Icons/Root";

        /// <summary>The child of the node. Roots only ever have one child.</summary>
        [HideInInspector]
        public BaseNode Child;

        protected override void OnEnter()
        {

        }

        protected override void OnExit()
        {

        }

        protected override NodeResult OnTick()
        {
            // If child hasn't been assigned return failure
            return Child is null ? NodeResult.Failure : Child.Tick();
        }

        /// <summary>Sets the child of the root node. Root nodes only ever have one child.</summary>
        /// <param name="node">The node that will be set as the child.</param>
        public void SetChild(BaseNode node)
        {
            // Ensure the old child no longer references this node at its parent
            if (Child != null)
            {
                BaseNode temp = null;
                Child.SetParent(temp);
            }

            // Set the new child's parent to this node
            if (node != null)
                node.SetParent(this);

            Child = node;
        }

        /// <summary>Clears the node's child.</summary>
        public void ClearChild() => Child = null;

        /// <summary>Sets the child of the root node. Root nodes only ever have one child.</summary>
        /// <param name="nodeGuid">The guid that will be used to find the correct child.</param>
        public void SetChild(string nodeGuid)
        {
            // Ensure the old child no longer references this node at its parent
            if (Child != null)
            {
                BaseNode temp = null;
                Child.SetParent(temp);
            }

            // Set the new child's parent to this node
            var node = Owner.Nodes.Find(itr => itr.GUID == nodeGuid);
            node.SetParent(this);

            Child = node;
        }
    }
}