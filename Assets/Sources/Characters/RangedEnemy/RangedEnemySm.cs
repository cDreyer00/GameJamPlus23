using Sources.Characters.Modules;
using Sources.Systems.FSM;
using UnityEngine;
using UnityEngine.Serialization;
using static Character.State;

namespace Sources.Characters.RangedEnemy
{
    public class RangedEnemySm : StateMachineModule<RangedEnemySm, Character.State>
    {
        ProjectileLauncher _attackModule;
        protected override Character.State InitialState => Idle;
        protected override RangedEnemySm Context => this;
        protected override void Init()
        {
            base.Init();
            _attackModule = Character.GetModule<ProjectileLauncher>();
            _attackModule.Target = GameManager.Instance.Player.transform;
            IdleState();
            AttackingState();
        }

        void IdleState()
        {
            stateMachine.Transition(Idle, Attacking, static sm => sm._attackModule.Target);
        }
        void AttackingState()
        {
            stateMachine.Transition(Attacking, Idle, static sm => !sm._attackModule.Target);
            stateMachine[LifeCycle.Enter, Attacking] = static sm => sm._attackModule.StartModule();
            stateMachine[LifeCycle.Exit, Attacking] = static sm => sm._attackModule.StopModule();
        }
    }
}