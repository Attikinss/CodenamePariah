using UnityEngine;

namespace WhiteWillow.Nodes
{
    [Category("Tasks")]
    public class Log : Task
    {
        public override string IconPath { get; } = "Icons/Log";

        [Tooltip("The message that will be logged to the console.")]
        public string Message = "";

        protected override void OnEnter() { }

        protected override void OnExit() { }

        protected override NodeResult OnTick()
        {
            Debug.Log($"{Owner.Agent} {Message}");
            return NodeResult.Success;
        }
    }
}