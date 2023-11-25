using System;
using JetBrains.Annotations;
using Sources.Systems.FSM;
using UnityEngine;
using UnityEngine.Serialization;
using static Character.State;

namespace Sources.Characters.Modules
{
    [Serializable]
    public abstract class CharacterStateModule : CharacterModule, IState<Character.State>
    {
        [SerializeField] StateMachineModule stateMachineModule;
        public StateMachine<Character.State> StateMachine => stateMachineModule.StateMachine;
        public abstract Character.State StateEnum { get; }
        public bool ActiveState => StateMachine != null && StateMachine.CurrentState == StateEnum;
        public virtual void FixedUpdate() {}
        public virtual void Update() {}
        public abstract void Enter();
        public abstract void Exit();
        protected override void Awake()
        {
            base.Awake();
            this.AddTransitionListeners(StateMachine);
        }
    }
}