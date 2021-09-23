using UnityEngine;

namespace WhiteWillow.Nodes
{
    ///<summary>The base of all decorator nodes.</summary>
    public abstract class Decorator : BaseNode
    {
        /// <summary>The child of the node. Decorators only ever have one child.</summary>
        [HideInInspector]
        public BaseNode Child;

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

        /// <summary>Sets the child of the node.</summary>
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