namespace UserTest.AITest.Wants;

public sealed class Food : WantBase
{
    public Food() : base("Food") { }

    public override double DeterminePriority(AgentBase agent)
    {
        return 1;
    }
}
