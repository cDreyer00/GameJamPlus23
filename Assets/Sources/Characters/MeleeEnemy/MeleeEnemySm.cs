using System;
using Sources.Characters.Modules;
using Sources.Systems.FSM;
using UnityEngine;
using UnityEngine.AI;
using static Character.State;

namespace Sources.Characters.MeleeEnemy
{
    public class MeleeEnemySm : StateMachineModule
    {
        NavMeshMovement _movementModule;
        protected override void Init()
        {
            base.Init();
            _movementModule = Character.GetModule<NavMeshMovement>();
            _movementModule.Target = GameManager.Instance.Player.transform;
            StateMachine.Transition(Idle, Chasing, () => _movementModule.Target);
            StateMachine.Transition(Chasing, Idle, () => !_movementModule.Target);
            
            StateMachine[LifeCycle.Enter, Chasing] += () => _movementModule.StartModule();
            StateMachine[LifeCycle.Exit, Chasing] += () => _movementModule.StopModule();
        }
    }
}