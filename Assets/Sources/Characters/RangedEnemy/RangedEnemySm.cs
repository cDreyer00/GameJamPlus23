using Sources.Characters.Modules;
using Sources.Systems.FSM;
using UnityEngine;
using UnityEngine.Serialization;
using static Character.State;

namespace Sources.Characters.RangedEnemy
{
    public class RangedEnemySm : StateMachineModule
    {
        ProjectileLauncher _attackModule;
        protected override void Init()
        {
            base.Init();
            _attackModule = Character.GetModule<ProjectileLauncher>();
            _attackModule.Target = GameManager.Instance.Player.transform;
            StateMachine.Transition(Idle, Attacking, () => _attackModule.Target);
            StateMachine.Transition(Attacking, Idle, () => !_attackModule.Target);

            StateMachine[LifeCycle.Enter, Attacking] += () => _attackModule.StartModule();
            StateMachine[LifeCycle.Exit, Attacking] += () => _attackModule.StopModule();
        }
    }
}