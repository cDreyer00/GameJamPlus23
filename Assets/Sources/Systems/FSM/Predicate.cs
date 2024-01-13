using UnityEngine;

namespace Sources.Systems.FSM
{
    public abstract class Predicate : ScriptableObject, IPredicate
    {
        public abstract bool Evaluate();
    }
}