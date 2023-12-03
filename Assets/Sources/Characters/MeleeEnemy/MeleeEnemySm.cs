using Sources.Characters.Modules;
using Sources.Systems.FSM;
using static Character.State;

namespace Sources.Characters.MeleeEnemy
{
    public class MeleeEnemySm : StateMachineModule<MeleeEnemySm, Character.State>
    {
        NavMeshMovement _movementModule;
        protected override Character.State InitialState => Idle;
        protected override MeleeEnemySm Context => this;
        protected override void Init()
        {
            base.Init();
            _movementModule = Character.GetModule<NavMeshMovement>();
            _movementModule.Target = GameManager.Instance.Player.transform;
            IdleState();
            ChasingState();
        }
        void IdleState()
        {
            stateMachine.Transition(Idle, Chasing, static sm => sm._movementModule.Target);
        }
        void ChasingState()
        {
            stateMachine.Transition(Chasing, Idle, static sm => !sm._movementModule.Target);
            stateMachine[LifeCycle.Enter, Chasing] = static sm => sm._movementModule.StartChase();
            stateMachine[LifeCycle.Exit, Chasing] = static sm => sm._movementModule.StopMovement();
        }
    }
}