namespace UserTest.AITest.Wants;

public sealed class Temperature : WantBase
{
    public Temperature() : base("Temperature") { }

    public override double DeterminePriority(AgentBase agent)
    {
        return 1;
    }
}
