using Sources.Characters.Modules;
using Sources.Systems.FSM;
using UnityEngine;
using UnityEngine.Serialization;
using static Character.State;

namespace Sources.Characters.RangedEnemy
{
    public class RangedEnemySm : StateMachineModule
    {
        [SerializeField] HealthModule       healthModule;
        [SerializeField] ProjectileLauncher attackModule;
        protected override void Init()
        {
            base.Init();
            attackModule.Target = GameManager.Instance.Player.transform;
            StateMachine.Transition(Idle, Attacking, () => attackModule.Target);
            StateMachine.Transition(Attacking, Idle, () => !attackModule.Target);
        }
    }
}