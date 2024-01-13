using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Debug = UnityEngine.Debug;

/// <summary>
/// Taken from UnityEngine.InputSystem.Utilities
/// Helper to avoid array allocations if there's only a single value in the array.
/// </summary>
/// <remarks>
/// Also, once more than one entry is necessary, allows treating the extra array as having capacity.
/// This means that, for example, 5 or 10 entries can be allocated in batch rather than growing an
/// array one by one.
/// </remarks>
/// <typeparam name="TValue">Element type for the array.</typeparam>
public struct InlinedArray<TValue> : IEnumerable<TValue>
{
    // We inline the first value so if there's only one, there's
    // no additional allocation. If more are added, we allocate an array.
    public int length;

    public TValue   firstValue;
    public TValue[] additionalValues;

    public int Capacity => additionalValues?.Length + 1 ?? 1;

    public InlinedArray(TValue value)
    {
        length = 1;
        firstValue = value;
        additionalValues = null;
    }

    public InlinedArray(TValue firstValue, params TValue[] additionalValues)
    {
        length = 1 + additionalValues.Length;
        this.firstValue = firstValue;
        this.additionalValues = additionalValues;
    }

    public InlinedArray(IEnumerable<TValue> values)
        : this()
    {
        var tValues = values as IReadOnlyCollection<TValue> ?? values.ToArray();
        length = tValues.Count;
        additionalValues = length > 1 ? new TValue[length - 1] : null;

        var index = 0;
        foreach (var value in tValues) {
            if (index == 0)
                firstValue = value;
            else
                additionalValues![index - 1] = value;
            ++index;
        }
    }

    public TValue this[int index]
    {
        get
        {
            if (index < 0 || index >= length)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (index == 0)
                return firstValue;

            return additionalValues[index - 1];
        }
        set
        {
            if (index < 0 || index >= length)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (index == 0)
                firstValue = value;
            else
                additionalValues[index - 1] = value;
        }
    }

    public void Clear()
    {
        length = 0;
        firstValue = default;
        additionalValues = null;
    }

    public void ClearWithCapacity()
    {
        firstValue = default;
        for (var i = 0; i < length - 1; ++i)
            additionalValues[i] = default;
        length = 0;
    }

    ////REVIEW: This is inconsistent with ArrayHelpers.Clone() which also clones elements
    public InlinedArray<TValue> Clone()
    {
        return new InlinedArray<TValue>
        {
            length = length,
            firstValue = firstValue,
            additionalValues = additionalValues is { Length: > 0 } ? ArrayHelpers.Copy(additionalValues) : null
        };
    }

    public void SetLength(int size)
    {
        // Null out everything we're cutting off.
        if (size < length) {
            for (var i = size; i < length; ++i)
                this[i] = default;
        }

        length = size;

        if (size > 1 && (additionalValues == null || additionalValues.Length < size - 1))
            Array.Resize(ref additionalValues, size - 1);
    }

    public TValue[] ToArray()
    {
        return ArrayHelpers.Join(firstValue, additionalValues);
    }

    public TOther[] ToArray<TOther>(Func<TValue, TOther> mapFunction)
    {
        if (length == 0)
            return null;

        var result = new TOther[length];
        for (var i = 0; i < length; ++i)
            result[i] = mapFunction(this[i]);

        return result;
    }

    public int IndexOf(TValue value, [CanBeNull] IEqualityComparer<TValue> comparer = null)
    {
        comparer ??= EqualityComparer<TValue>.Default;
        if (length > 0) {
            if (comparer.Equals(firstValue, value))
                return 0;
            if (additionalValues is { Length: > 0 }) {
                for (var i = 0; i < length - 1; ++i)
                    if (comparer.Equals(additionalValues[i], value))
                        return i + 1;
            }
        }

        return -1;
    }

    public int Append(TValue value)
    {
        if (length == 0) {
            firstValue = value;
        }
        else if (additionalValues == null) {
            additionalValues = new TValue[1];
            additionalValues[0] = value;
        }
        else {
            Array.Resize(ref additionalValues, length);
            additionalValues[length - 1] = value;
        }

        var index = length;
        ++length;
        return index;
    }

    public int AppendWithCapacity(TValue value, int capacityIncrement = 10)
    {
        if (length == 0) {
            firstValue = value;
        }
        else {
            var numAdditionalValues = length - 1;
            ArrayHelpers.AppendWithCapacity(ref additionalValues, ref numAdditionalValues, value,
                capacityIncrement: capacityIncrement);
        }

        var index = length;
        ++length;
        return index;
    }

    public void AssignWithCapacity(InlinedArray<TValue> values)
    {
        if (Capacity < values.length && values.length > 1)
            additionalValues = new TValue[values.length - 1];

        length = values.length;
        if (length > 0)
            firstValue = values.firstValue;
        if (length > 1)
            Array.Copy(values.additionalValues, additionalValues, length - 1);
    }

    public void Append(IEnumerable<TValue> values)
    {
        foreach (var value in values)
            Append(value);
    }

    public bool Remove(TValue value, [CanBeNull] IEqualityComparer<TValue> comparer = null)
    {
        if (length < 1)
            return false;

        comparer ??= EqualityComparer<TValue>.Default;

        if (comparer.Equals(firstValue, value)) {
            RemoveAt(0);
            return true;
        }
        else if (additionalValues is { Length: > 0 }) {
            for (var i = 0; i < length - 1; ++i) {
                if (comparer.Equals(additionalValues[i], value)) {
                    RemoveAt(i + 1);
                    return true;
                }
            }
        }
        return false;
    }

    public void RemoveAtWithCapacity(int index)
    {
        if (index < 0 || index >= length)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (index == 0) {
            if (length == 1) {
                firstValue = default;
            }
            else if (length == 2) {
                firstValue = additionalValues[0];
                additionalValues[0] = default;
            }
            else {
                Debug.Assert(length > 2);
                firstValue = additionalValues[0];
                var numAdditional = length - 1;
                additionalValues.EraseAtWithCapacity(ref numAdditional, 0);
            }
        }
        else {
            var numAdditional = length - 1;
            additionalValues.EraseAtWithCapacity(ref numAdditional, index - 1);
        }

        --length;
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= length)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (index == 0) {
            if (additionalValues is { Length: > 0 }) {
                firstValue = additionalValues[0];
                if (additionalValues.Length == 1)
                    additionalValues = null;
                else {
                    Array.Copy(additionalValues, 1, additionalValues, 0, additionalValues.Length - 1);
                    Array.Resize(ref additionalValues, additionalValues.Length - 1);
                }
            }
            else {
                firstValue = default;
            }
        }
        else {
            Debug.Assert(additionalValues is { Length: > 0 });

            var numAdditionalValues = length - 1;
            if (numAdditionalValues == 1) {
                // Remove only entry in array.
                additionalValues = null;
            }
            else if (index == length - 1) {
                // Remove entry at end.
                Array.Resize(ref additionalValues, numAdditionalValues - 1);
            }
            else {
                // Remove entry at beginning or in middle by pasting together
                // into a new array.
                var newAdditionalValues = new TValue[numAdditionalValues - 1];
                if (index >= 2) {
                    // Copy elements before entry.
                    Array.Copy(additionalValues, 0, newAdditionalValues, 0, index - 1);
                }

                // Copy elements after entry. We already know that we're not removing
                // the last entry so there have to be entries.
                Array.Copy(additionalValues, index + 1 - 1, newAdditionalValues, index - 1,
                    length - index - 1);

                additionalValues = newAdditionalValues;
            }
        }

        --length;
    }

    public void RemoveAtByMovingTailWithCapacity(int index)
    {
        if (index < 0 || index >= length)
            throw new ArgumentOutOfRangeException(nameof(index));

        var numAdditionalValues = length - 1;
        if (index == 0) {
            if (length > 1) {
                firstValue = additionalValues[numAdditionalValues - 1];
                additionalValues[numAdditionalValues - 1] = default;
            }
            else {
                firstValue = default;
            }
        }
        else {
            Debug.Assert(additionalValues is { Length: > 0 });

            ArrayHelpers.EraseAtByMovingTail(additionalValues, ref numAdditionalValues, index - 1);
        }

        --length;
    }

    public bool RemoveByMovingTailWithCapacity(TValue value, IEqualityComparer<TValue> comparer = null)
    {
        var index = IndexOf(value, comparer);
        if (index == -1)
            return false;

        RemoveAtByMovingTailWithCapacity(index);
        return true;
    }

    public bool Contains(TValue value, [CanBeNull] IEqualityComparer<TValue> comparer = null)
    {
        comparer ??= EqualityComparer<TValue>.Default;
        for (var n = 0; n < length; ++n)
            if (comparer.Equals(this[n], value))
                return true;
        return false;
    }

    public void Merge(InlinedArray<TValue> other, [CanBeNull] IEqualityComparer<TValue> comparer = null)
    {
        comparer ??= EqualityComparer<TValue>.Default;
        for (var i = 0; i < other.length; ++i) {
            var value = other[i];
            if (Contains(value, comparer))
                continue;

            ////FIXME: this is ugly as it repeatedly copies
            Append(value);
        }
    }
    public Enumerator GetEnumerator() => new() { array = this, index = -1 };
    IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<TValue>
    {
        public InlinedArray<TValue> array;
        public int                  index;

        public bool MoveNext()
        {
            if (index >= array.length)
                return false;
            ++index;
            return index < array.length;
        }

        public void Reset()
        {
            index = -1;
        }

        public TValue Current => array[index];
        object IEnumerator.Current => Current;

        public void Dispose() {}
    }
}