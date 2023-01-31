namespace Engine.Utilities.Noise;
public interface INoiseProvider<TInput, TOutput>
{
    TOutput Sample(TInput input);
}
