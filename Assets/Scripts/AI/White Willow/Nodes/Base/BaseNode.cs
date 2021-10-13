using System;
using System.Collections.Generic;
using UnityEngine;

namespace WhiteWillow.Nodes
{
    ///<summary>The base of all nodes. All nodes inherit from this class.</summary>
    public abstract class BaseNode : ScriptableObject, IComparer<BaseNode>, IComparable<BaseNode>
    {
        [HideInInspector]
        public Action OnNodeActive;

        [HideInInspector]
        public Action OnNodeInactive;

        /// <summary>Defines whether the node is currently active.</summary>
        [HideInInspector]
        public bool Active = false;

        /// <summary>The behaviour tree the node belongs to.</summary>
        [HideInInspector]
        public BehaviourTree Owner;

        /// <summary>The parent of the node. Nodes only ever have one parent.</summary>
        [HideInInspector]
        public BaseNode Parent;

        /// <summary>The current state of the nde.</summary>
        [HideInInspector]
        public NodeResult State;

        /// <summary>The display name of the node.</summary>
        [Header("General")]
        public string Title = "";

        /// <summary>The unique ID of the node.</summary>
        [HideInInspector]
        public string GUID = "";

        /// <summary>Defines whether the node is currently unavailable to update/tick.</summary>
        [HideInInspector]
        public bool Locked = false;

        /// <summary>The order in which the node will be executed in relation to its siblings.</summary>
        [HideInInspector]
        public ushort ExecutionOrder = 1;

        /// <summary>The dimensions of the node when visible in the graph view.</summary>
        [HideInInspector]
        public Rect GraphDimensions;

        private bool m_Aborted = false;

        /// <summary>The file path to the icon the node will display in the graph view.</summary>
        public virtual string IconPath { get; } = "Icons/Task";

        /// <summary>A higher level tick function that's used to trigger entry/exit code AND retrieve the result of active leaf nodes.</summary>
        /// <returns>The result of a leaf nodes current status.</returns>
        public NodeResult Tick()
        {
            // Prevents traversal of this node while it's locked
            if (Locked) return NodeResult.Locked;

            if (!Active)
            {
                // Node now active and will perform pre-op functionality
                OnEnter();
                Active = true;
                OnNodeActive?.Invoke();
            }

            // Cache node state
            State = OnTick();

            if (State != NodeResult.Running)
            {
                // Node completed and will now clean up
                OnExit();
                Active = false;
                OnNodeInactive?.Invoke();
            }

            return State;
        }

        /// <summary>Sets the parent of the node.</summary>
        /// <param name="node">The node that will be set as the parent.</param>
        public void SetParent(BaseNode node) => Parent = node;

        /// <summary>Sets the parent of the node.</summary>
        /// <param name="node">The guid that will be used to find the parent.</param>
        public void SetParent(string guid) => Parent = Owner.Nodes.Find(itr => string.CompareOrdinal(itr.GUID, guid) == 0);

        /// <summary>Sets the order of execution for the node.</summary>
        /// <param name="order">The value defining execution order.</param>
        public void SetExecutionOrder(ushort order) => ExecutionOrder = order;

        /// <summary>Locks the node to prevent traversal.</summary>
        public void Lock() => Locked = true;

        /// <summary>Unlocks the node to allow traversal.</summary>
        public void Unlock() => Locked = false;

        /// <summary>Compares the execution order of two nodes for sorting purposes.</summary>
        /// <param name="a">The first node to compare.</param>
        /// <param name="b">The second node to compare.</param>
        /// <returns>
        /// <br>equals 0     : Both sides are the same</br>
        /// <br>more than 0  : A is greater than B</br>
        /// <br>less than 0  : A is less than B</br>
        /// </returns>
        public int Compare(BaseNode a, BaseNode b) => a.ExecutionOrder - b.ExecutionOrder;

        /// <summary>Compares the execution order of two nodes for sorting purposes.</summary>
        /// <param name="other">The other node to compare to.</param>
        /// <returns>
        /// <br>equals 0     : Both sides are the same</br>
        /// <br>more than 0  : A is greater than B</br>
        /// <br>less than 0  : A is less than B</br>
        /// </returns>
        public int CompareTo(BaseNode other) => ExecutionOrder - other.ExecutionOrder;

        /// <summary>Executes pre-operation code before ticking the node.</summary>
        protected abstract void OnEnter();

        /// <summary>A recursive function that's used to retrieve the result of active leaf nodes.</summary>
        /// <returns>The result of a leaf nodes current status.</returns>
        protected abstract NodeResult OnTick();

        /// <summary>Executes post-operation code once the node returns an exit state.</summary>
        protected abstract void OnExit();

        public virtual void Abort() => m_Aborted = true;
    }
}