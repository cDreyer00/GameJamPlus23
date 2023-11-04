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
        Character.AddModule(this);
        Init();
    }

    protected abstract void Init();

    void OnDestroy()
    {
        if (Character == null) return;

        Character.RemoveModule(this);
    }

    public void Enable() => enabled = true;
    public void Disable() => enabled = true;
}

public interface IModule { }
public interface IMovementModule : IModule { }
public interface IAttackModule : IModule { }