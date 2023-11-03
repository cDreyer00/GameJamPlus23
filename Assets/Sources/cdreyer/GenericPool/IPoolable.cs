using System;
using UnityEngine;

public interface IPoolable<T> where T : MonoBehaviour
{
    public GenericPool<T> Pool { get; set; }    
    public void OnGet(GenericPool<T> pool);
    public void OnRelease();
    public void OnCreated();
}