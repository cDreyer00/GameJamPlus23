using UnityEngine;

public class CharacterModule : MonoBehaviour
{
    Character _character;
    public Character Character
    {
        get
        {
            if (_character != null)
                return _character;

            if (!transform.root.TryGetComponent(out _character))
                Debug.LogWarning($"character for {GetType().Name} not found");

            return _character;
        }
    }
}
