using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class CharacterModule : MonoBehaviour
{
    Character _character;
    public Character Character => _character;

    void Awake()
    {
        // if (!transform.root.TryGetComponent(out _character))
        // {
        //     Debug.LogError($"Character not found in root, removing module {GetType().Name} from {transform.root.name}");
        //     Destroy(this);
        //     return;
        // }

        var character = transform.GetComponentInParent<Character>();
        if (character == null) {
            Debug.LogError($"Character not found upstream, removing module {GetType().Name} from {transform.root.name}");
            Destroy(this);
            return;
        }
        _character = character;
        Character.AddModule(this);
        Init();
    }
    abstract protected void Init();
    void OnDestroy()
    {
        if (Character == null) return;

        Character.RemoveModule(this);
    }
    public void Enable() => enabled = true;
    public void Disable() => enabled = true;
}

public interface IModule {}
public interface IMovementModule : IModule {}
public interface IAttackModule : IModule {}