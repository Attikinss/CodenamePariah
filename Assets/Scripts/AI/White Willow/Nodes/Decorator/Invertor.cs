namespace WhiteWillow.Nodes
{
    [Category("Decorator")]
    public sealed class Invertor : Decorator
    {
        protected override void OnEnter() { }

        protected override void OnExit() { }

        protected override NodeResult OnTick()
        {
            // Kinda weird logic but a node without a child returns failure sooo...
            if (Child == null)
                return NodeResult.Success;

            NodeResult result = Child.Tick();
            if (result == NodeResult.Success)
                return NodeResult.Failure;
            else if (result == NodeResult.Failure)
                return NodeResult.Success;

            return NodeResult.Running;
        }
    }
}