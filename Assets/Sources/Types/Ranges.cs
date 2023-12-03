public static class Ranges
{
    public static float Map(float value,
        float min, float max,
        float newMin, float newMax)
    {
        return (value - min) / (max - min) * (newMax - newMin) + newMin;
    }
    public static int Map(int value,
        int min, int max,
        int newMin, int newMax)
    {
        return (value - min) / (max - min) * (newMax - newMin) + newMin;
    }
    public static float Map01(float value, float min, float max)
    {
        return Map(value, min, max, 0, 1);
    }
    public static int Map01(int value, int min, int max)
    {
        return Map(value, min, max, 0, 1);
    }
}