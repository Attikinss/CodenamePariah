using System.Linq;

namespace WhiteWillow.Nodes
{
    ///<summary>
    ///<br>A node that iterates through its child nodes one after another</br>
    ///<br>until one returns a failure or all nodes are successful.</br>
    ///</summary>
    [Category("Composites")]
    public class Sequence : Composite
    {
        public override string IconPath { get; } = "Icons/Sequence";

        protected override void OnEnter()
        {
            
        }

        protected override void OnExit()
        {
            RunningChild = null;
        }

        protected override NodeResult OnTick()
        {
            // Prevents traversal of this node while it's locked
            if (Locked) return NodeResult.Locked;

            // No point doing anything
            if (Children.Count == 0) return NodeResult.Failure;

            // Ignore all other children if there is a running child
            if (RunningChild != null) return TickRunningChild();

            /////////////////////////////////////////////////////////////////////////
            ////                       ---<[ N O T E ]>---                       ////
            //// The following section of code only gets called ONCE if there is ////
            ////  no running child and the initial result is success or running  ////
            /////////////////////////////////////////////////////////////////////////

            // Get first (unlocked) child's result
            NodeResult result = GetFirstUnlockedNode().Tick();

            // TODO: Implement abort

            if (result == NodeResult.Running)
                RunningChild = Children.First();
            else if (result == NodeResult.Success)
            {
                // Get next node if one exists
                var nextNode = GetNextUnlockedNode(RunningChild);

                if (nextNode != null)
                {
                    // If there is another node to tick through then ensure
                    // the tree doesn't re-evaluate and comes back here
                    RunningChild = nextNode;
                    (Parent as Composite)?.SetRunningChild(this);
                    return NodeResult.Running;
                }
            }

            if (result != NodeResult.Failure && GetFirstUnlockedNode() != Children.Last())
            {
                (Parent as Composite)?.SetRunningChild(this);
                return NodeResult.Running;
            }

            // Node more nodes are available so the tree can now re-evaluate
            return result;
        }

        protected override NodeResult TickRunningChild()
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
            else if (result == NodeResult.Success)
            {
                // Get next node if one exists
                var nextNode = GetNextUnlockedNode(RunningChild);

                if (nextNode != null)
                {
                    // If there is another node to tick through then ensure
                    // the tree doesn't re-evaluate and comes back here
                    RunningChild = nextNode;
                    return NodeResult.Running;
                }

                // Node more nodes are available so the tree can now re-evaluate
                RunningChild = null;
                return result;
            }

            // Clear child now that it is no longer running
            RunningChild = null;
            return result;
        }
    }
}