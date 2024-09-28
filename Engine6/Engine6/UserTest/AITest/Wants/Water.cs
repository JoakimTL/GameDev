namespace UserTest.AITest.Wants;

public sealed class Water : WantBase
{
    public Water() : base("Water") { }

    public override double DeterminePriority(AgentBase agent)
    {
        return 1;
    }
}
