namespace UserTest.AITest.Wants;

public sealed class Sleep : WantBase
{
    public Sleep() : base("Sleep") { }

    public override double DeterminePriority(AgentBase agent)
    {
        return 1;
    }
}
