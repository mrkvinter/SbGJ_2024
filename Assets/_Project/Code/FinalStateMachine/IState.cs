using Cysharp.Threading.Tasks;

namespace Game.Core.FinalStateMachine
{
    
    public interface IHasFsm {
        void SetFsm(IFsm newFsm);
    }

    public interface IHasParameters
    {
        void SetArguments(object parameters);
    }

    public interface IState
    {
        UniTask Enter();
        UniTask Exit();
    }
    
    public interface IStateWithArguments<in T> : IState, IHasParameters
    {
    }

    public abstract class BaseState : IState, IHasFsm
    {
        private IFsm fsm;

        public UniTask Enter()
        {
            return OnEnter();
        }

        public UniTask Exit()
        {
            return OnExit();
        }
        
        void IHasFsm.SetFsm(IFsm newFsm)
        {
            fsm = newFsm;
        }

        protected virtual UniTask OnEnter()
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnExit()
        {
            return UniTask.CompletedTask;
        }

        protected void ToState<TState>() where TState : IState
        {
            fsm.ToState<TState>();
        }
    }
    
    public abstract class BaseState<TArgs> : BaseState, IStateWithArguments<TArgs>
    {
        protected TArgs Arguments { get; private set; }
        
        private void SetArguments(TArgs parameters)
        {
            Arguments = parameters;
        }

        public void SetArguments(object parameters)
        {
            if (parameters is not TArgs args)
            {
                throw new System.InvalidCastException($"Expected {typeof(TArgs)} but got {parameters.GetType()}");
            }
            
            SetArguments(args);
        }
    }
}