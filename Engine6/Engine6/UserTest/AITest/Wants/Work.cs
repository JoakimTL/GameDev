namespace UserTest.AITest.Wants;

public sealed class Work : WantBase
{
    public Work() : base("Work") { }

    public override double DeterminePriority(AgentBase agent)
    {
        return 1;
    }
}
