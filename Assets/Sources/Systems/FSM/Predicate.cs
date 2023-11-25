using UnityEngine;

namespace Sources.Systems.FSM
{
    [CreateAssetMenu(menuName = "Characters/Predicate")]
    public abstract class Predicate : ScriptableObject, IPredicate
    {
        public abstract bool Evaluate();
    }
}