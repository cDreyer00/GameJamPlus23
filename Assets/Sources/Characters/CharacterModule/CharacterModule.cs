using UnityEngine;

public abstract class CharacterModule : MonoBehaviour
{
    Character _character;
    public Character Character => _character;

    void Awake()
    {
        if (!transform.root.TryGetComponent(out _character))
        {
            Debug.LogError($"Character not found in root, removing module {GetType().Name} from {transform.root.name}");
            Destroy(this);
            return;
        }

        Init();
    }
    
    protected abstract void Init();
}
