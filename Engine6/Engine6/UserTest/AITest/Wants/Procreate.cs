namespace UserTest.AITest.Wants;

public sealed class Procreate : WantBase
{
    public Procreate() : base("Procreate") { }

    public override double DeterminePriority(AgentBase agent)
    {
        return 1;
    }
}
