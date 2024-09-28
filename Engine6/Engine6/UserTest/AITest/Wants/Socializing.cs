namespace UserTest.AITest.Wants;

public sealed class Socializing : WantBase
{
    public Socializing() : base("Socializing") { }

    public override double DeterminePriority(AgentBase agent)
    {
        return 1;
    }
}
