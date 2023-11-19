using System;

[Serializable]
public struct SerializableKVP<TKey, TValue>
{
    public TKey key;
    public TValue value;

    public SerializableKVP(TKey key, TValue value)
    {
        this.key = key;
        this.value = value;
    }
}