namespace Engine.Structure.Attributes;

public class SystemAttribute : Attribute
{
    public bool Essential { get; }
    public int TickInterval { get; }

    public SystemAttribute(bool essential, int tickInterval)
    {
        Essential = essential;
        TickInterval = tickInterval;
    }
}
