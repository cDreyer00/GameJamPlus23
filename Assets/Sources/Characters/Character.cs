using System;
using System.Collections.Generic;
using System.Linq;
using Sources;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class Character : MonoBehaviour, ICharacter, IPoolable<MonoBehaviour>
{
    readonly HashSet<CharacterModule> _modules = new();
    readonly CharacterEvents          _events  = new();
    public   string                   team = "";
    public Vector3 Position => transform.position;
    public CharacterEvents Events => _events;
    public IReadOnlyCollection<CharacterModule> Modules => _modules;
    public bool AddModule(CharacterModule module) => _modules.Add(module);
    public bool RemoveModule(CharacterModule module) => _modules.Remove(module);
    public T GetModule<T>() where T : CharacterModule => _modules.OfType<T>().FirstOrDefault();
    public bool TryGetModule<T>(out T module) where T : CharacterModule
    {
        module = GetModule<T>();
        return module != null;
    }
    void Awake()
    {
        _modules.UnionWith(GetComponentsInChildren<CharacterModule>());
    }
    virtual protected void OnEnable() {}
    virtual protected void OnDisable() {}
    void ToggleModules(Object obj, bool arg)
    {
        if (arg) {
            foreach (var characterModule in _modules) {
                characterModule.enabled = false;
            }
        }
    }
    public GenericPool<MonoBehaviour> Pool { get; set; }
    public void OnGet() => _events.Initialized();

    public void OnRelease()
    {
        foreach (var characterModule in _modules) {
            characterModule.CancelInvoke();
        }
    }

    public void OnCreated() {}
    public enum State
    {
        // InControl:
        InControl, Idle, Chasing,
        Attacking,
        // Yielded:
        Yielded, Controlled, Dying,
    }
}