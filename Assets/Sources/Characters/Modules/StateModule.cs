using Sources.Systems.FSM;

namespace Sources.Characters.Modules
{
    public class StateModule : CharacterModule
    {
        StateMachine<Character.State> _stateMachine = new(Character.State.Idle);
        protected override void Init() {}
    }
}