using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace Game.Core.FinalStateMachine
{
    public interface IFsm
    
    {
        Type CurrentState { get; }
        public void RegistryState(IState state);
        UniTask ToState(Type state);
        UniTask ToState<TState>() where TState : IState;
        UniTask ToStateWithParams<TState>(object parameters) where TState : IState;
        UniTask Exit();
    }

    [UsedImplicitly]
    public class Fsm : IFsm
    {
        private readonly Dictionary<Type, IState> states = new();

        private IState currentState;

        public Type CurrentState => currentState.GetType();
        
        private bool changingState;

        public void RegistryState(IState state)
        {
            (state as IHasFsm)?.SetFsm(this);
            states.Add(state.GetType(), state);
        }

        public async UniTask ToState(Type state)
        {
            if (changingState) throw new InvalidOperationException("Already changing state");

            changingState = true;
            if (currentState != null) await currentState.Exit();

            var newState = states[state];
            if (newState is IHasParameters)
            {
                throw new InvalidOperationException($"State {state} requires parameters");
            }

            currentState = newState;
            await currentState.Enter();
            changingState = false;
        }

        public async UniTask ToStateWithParams<TState>(object parameters)
            where TState : IState
        {
            if (changingState) throw new InvalidOperationException("Already changing state");

            changingState = true;
            if (currentState != null) await currentState.Exit();

            var newState = states[typeof(TState)];
            if (newState is not IHasParameters hasParameters)
            {
                throw new InvalidOperationException($"State {typeof(TState)} does not have parameters");
            }

            hasParameters.SetArguments(parameters);
            currentState = newState;
            await currentState.Enter();
            changingState = false;
        }

        public UniTask ToState<TState>() where TState : IState => ToState(typeof(TState));

        public async UniTask Exit()
        {
            if (currentState != null) await currentState.Exit();

            currentState = null;
        }
    }
}