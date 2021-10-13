using UnityEngine;

namespace WhiteWillow.Nodes
{
    [Category("Tasks")]
    public class BreakPoint : Task
    {
        protected override void OnEnter()
        {
            Debug.Log($"{Owner.Agent.name}: Trigging Breakpoint");
            Debug.Break();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPaused = true;

            // TODO: Zoom in editor camera on agent which hit this breakpoint
            // CameraUtilities.ZoomToAgent();
#endif
        }

        protected override void OnExit() { }

        protected override NodeResult OnTick()
        {
            return NodeResult.Success;
        }
    }
}