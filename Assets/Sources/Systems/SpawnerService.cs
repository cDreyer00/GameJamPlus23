public static class SpawnerSrevice
{
    static EffectSignSpawner _effecSignSpawner;
    public static EffectSignSpawner EffecSignSpawner
    {
        get => _effecSignSpawner;
        set => _effecSignSpawner = value;
    }

    static MeleeEnemySpawner _meleeEnemySpawner;
    public static MeleeEnemySpawner MeleeEnemySpawner
    {
        get => _meleeEnemySpawner;
        set => _meleeEnemySpawner = value;
    }
}