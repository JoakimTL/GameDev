using Engine.Datatypes.Vectors;
using System.Numerics;

namespace Engine.Utilities.Noise;

public class Simplex2Noise : INoiseProvider<Vector2, float>
{
    private readonly uint _seed;
    private readonly Vector2 _spread;

    public Simplex2Noise(uint seed, Vector2 spread)
    {
        this._seed = seed;
        _spread = spread;
    }

    public float Sample(Vector2 input)
    {
        Vector2 pointInGenSpace = input / this._spread;
        Vector2i low = Vector2i.Floor(pointInGenSpace);
        Vector2 frac = pointInGenSpace - low.AsFloat;

        Span<float> dataValues = stackalloc float[4];
        GetDataValues(low, ref dataValues);
        return GetValueInterpolated(dataValues, frac);
    }

    private float GetValueInterpolated(Span<float> dataValues, Vector2 frac)
    {
        float x0 = Interpolate(dataValues[0], dataValues[1], frac.X);
        float x1 = Interpolate(dataValues[2], dataValues[3], frac.X);
        return Interpolate(x0, x1, frac.Y);
    }

    private float Interpolate(float a, float b, float p) => (b * p) + (a * (1 - p));

    private void GetDataValues(Vector2i low, ref Span<float> dataValues)
    {
        Span<Vector2i> points = stackalloc[] {
            low,
            low + new Vector2i(1, 0),
            low + new Vector2i(0, 1),
            low + new Vector2i(1, 1)
        };
        for (int i = 0; i < dataValues.Length; i++)
            dataValues[i] = GetDataValue(points[i]);
    }

    private float GetDataValue(Vector2i p)
    {
        unchecked
        {
            uint v = 42595009 + this._seed;
            v *= 40631027;
            v ^= (uint)((p.X * 4919) + (p.Y * 5879));
            v *= 40014307;
            v ^= (uint)((p.X * 5237) + (p.Y * 6823));
            return v * Constants.OneOverUintMax;
        }
    }
}
