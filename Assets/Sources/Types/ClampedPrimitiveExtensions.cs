public static class ClampedPrimitiveExtensions
{
    public static ClampedPrimitive<float> MapTo(this ClampedPrimitive<float> clampedPrimitive, float min, float max)
    {
        float newValue = Ranges.Map(clampedPrimitive.Value, clampedPrimitive.min, clampedPrimitive.max, min, max);
        return new ClampedPrimitive<float>(newValue, min, max);
    }
    public static ClampedPrimitive<int> MapTo(this ClampedPrimitive<int> clampedPrimitive, int min, int max)
    {
        int newValue = Ranges.Map(clampedPrimitive.Value, clampedPrimitive.min, clampedPrimitive.max, min, max);
        return new ClampedPrimitive<int>(newValue, min, max);
    }
}