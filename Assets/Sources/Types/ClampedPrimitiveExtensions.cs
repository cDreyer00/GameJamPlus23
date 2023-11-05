public static class ClampedPrimitiveExtensions
{
    public static ClampedPrimitive<float> MapTo(this ClampedPrimitive<float> clampedPrimitive, float min, float max)
    {
        float newValue = MapRange(clampedPrimitive.Value, clampedPrimitive.min, clampedPrimitive.max, min, max);
        return new ClampedPrimitive<float>(newValue, min, max);
    }
    public static ClampedPrimitive<int> MapTo(this ClampedPrimitive<int> clampedPrimitive, int min, int max)
    {
        int newValue = MapRange(clampedPrimitive.Value, clampedPrimitive.min, clampedPrimitive.max, min, max);
        return new ClampedPrimitive<int>(newValue, min, max);
    }
    public static float MapRange(float value,
        float min, float max,
        float newMin, float newMax)
    {
        return (value - min) / (max - min) * (newMax - newMin) + newMin;
    }
    public static int MapRange(int value,
        int min, int max,
        int newMin, int newMax)
    {
        return (value - min) / (max - min) * (newMax - newMin) + newMin;
    }
    public static float MapRangeTo01(float value, float min, float max)
    {
        return MapRange(value, min, max, 0, 1);
    }
    public static int MapRangeTo01(int value, int min, int max)
    {
        return MapRange(value, min, max, 0, 1);
    }
}