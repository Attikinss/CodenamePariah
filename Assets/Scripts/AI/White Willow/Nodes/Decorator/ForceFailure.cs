namespace WhiteWillow.Nodes
{
    [Category("Decorator")]
    public sealed class ForceFailure : Decorator
    {
        protected override void OnEnter() { }

        protected override void OnExit() { }

        protected override NodeResult OnTick()
        {
            if (Child == null || Child.Tick() != NodeResult.Running)
                return NodeResult.Failure;

            return NodeResult.Running;
        }
    }
}