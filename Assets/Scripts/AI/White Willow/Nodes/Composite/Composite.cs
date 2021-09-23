using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteWillow.Nodes
{
    ///<summary>The base of all composite nodes.</summary>
    public abstract class Composite : BaseNode
    {
        /// <summary>The child that returned a running result. All other nodes will be ignored until this node returns a success or fail state result.</summary>
        [HideInInspector]
        public BaseNode RunningChild;

        /// <summary>A set of child nodes the node can iterate through when calculating an optimal behaviour.</summary>
        [HideInInspector]
        public List<BaseNode> Children = new List<BaseNode>();

        /// <summary>Sets the running child node, forcing the composite node to ignore all other child nodes.</summary>
        /// <param name="node">The running node.</param>
        public void SetRunningChild(BaseNode node) => RunningChild = node;

        /// <summary>Clears the running node, forcing the composite node to start from the first next tick.</summary>
        public void ClearRunningNode() => RunningChild = null;

        /// <summary>Adds node(s) to the children set.</summary>
        /// <param name="nodes">The node(s) to add.</param>
        public void AddChildren(params BaseNode[] nodes)
        {
            if (Children == null) Children = new List<BaseNode>();

            foreach (BaseNode node in nodes)
            {
                if (node != null)
                {
                    // Connect the two nodes
                    node.SetParent(this);
                    Children.Add(node);
                }
            }

            Children = Children.OrderBy(n => n.ExecutionOrder).ToList();
        }

        /// <summary>Adds node(s) to the children set.</summary>
        /// <param name="guids">The guid(s) of the node(s) to add.</param>
        public void AddChildren(params string[] guids)
        {
            if (Children == null) Children = new List<BaseNode>();

            foreach (var guid in guids)
            {
                // Connect the two nodes
                var node = Owner.Nodes.Find(itr => itr.GUID == guid);

                if (Children.Contains(node))
                    return;

                node.SetParent(this);
                Children.Add(node);
            }

            Children = Children.OrderBy(n => n.ExecutionOrder).ToList();
        }

        /// <summary>Removes node(s) from the children set.</summary>
        /// <param name="nodes">The node(s) to remove.</param>
        public void RemoveChildren(params BaseNode[] nodes)
        {
            if (Children == null)
            {
                Children = new List<BaseNode>();
                return;
            }

            foreach (BaseNode node in nodes)
            {
                if (node != null)
                {
                    // Disconnect the two nodes
                    BaseNode temp = null;
                    node.SetParent(temp);
                    Children.Remove(node);
                }
            }
        }

        /// <summary>Removes node(s) from the children set.</summary>
        /// <param name="guids">The guid(s) of the node(s) to remove.</param>
        public void RemoveChildren(params string[] guids)
        {
            if (Children == null)
            {
                Children = new List<BaseNode>();
                return;
            }

            foreach (var guid in guids)
            {
                // Disconnect the two nodes
                var node = Children.Find(itr => itr.GUID == guid);
                BaseNode temp = null;
                node.SetParent(temp);
                Children.Remove(node);
            }
        }

        /// <summary>Gets the next unlocked node relevant to the order of execution.</summary>
        /// <param name="node">The current node.</param>
        /// <returns>The first unlocked node that follows the execution order.</returns>
        public BaseNode GetNextUnlockedNode(BaseNode node)
        {
            var minList = Children.Where(min => min != null && min.ExecutionOrder > node.ExecutionOrder && min != node).ToList();

            if (minList == null || minList.Count == 0)
                return null;
                
            return minList.Min();
        }
            
        /// <summary>Gets the first unlocked node available in child nodes.</summary>
        /// <returns>The first unlocked node in child nodes.</returns>
        public BaseNode GetFirstUnlockedNode()
        {
            BaseNode firstNode = null;

            foreach (var node in Children)
            {
                if (firstNode == null && !node.Locked)
                {
                    firstNode = node;
                    continue;
                }

                if (node.ExecutionOrder < firstNode.ExecutionOrder)
                    firstNode = node;
            }

            RunningChild = firstNode;
            return firstNode;
        }

        protected virtual NodeResult TickRunningChild()
        {
            // Get running child's result
            NodeResult result = RunningChild.Tick();

            // TODO: Implement abort

            if (result == NodeResult.Running)
            {
                // If child is still running, set self as parent's
                // running node if parent is a composite node
                (Parent as Composite)?.SetRunningChild(this);
                return result;
            }

            // Clear child now that it is no longer running
            ClearRunningNode();
            return result;
        }
    }
}