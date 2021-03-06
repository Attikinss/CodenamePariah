using UnityEngine;

namespace WhiteWillow.Nodes
{
    [Category("Task")]
    public sealed class Log : Task
    {
        public override string IconPath { get; } = "Icons/Log";

        [Tooltip("The message that will be logged to the console.")]
        public NodeMember<string> Message;

        protected override void OnEnter() { }

        protected override void OnExit() { }

        protected override NodeResult OnTick()
        {
            Debug.Log($"[{Owner.Agent}]: {Message.Value}");
            return NodeResult.Success;
        }
    }
}