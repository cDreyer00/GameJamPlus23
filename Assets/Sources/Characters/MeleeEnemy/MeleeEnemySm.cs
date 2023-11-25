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
        [SerializeField] HealthModule    healthModule;
        [SerializeField] NavMeshMovement movementModule;
        protected override void Init()
        {
            base.Init();
            movementModule.Target = GameManager.Instance.Player.transform;
            StateMachine.Transition(Idle, Chasing, () => movementModule.Target);
            StateMachine.Transition(Chasing, Idle, () => !movementModule.Target);
        }
    }
}