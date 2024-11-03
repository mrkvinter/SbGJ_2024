using Code.Buddies;
using Code.Utilities;
using Cysharp.Threading.Tasks;
using Game.Core.FinalStateMachine;
using RG.ContentSystem.Core;
using UnityEngine;

namespace Code.States
{
    public class GameRunState : BaseState<ContentRef<BuddyEntry>>
    {
        private GameFlow gameFlow;
        private IFsm fsm;

        private GameSettings gameSettings;

        public GameRunState(GameFlow gameFlow)
        {
            this.gameFlow = gameFlow;
            fsm = new Fsm();
            
            gameSettings = ContentManager.GetSettings<GameSettings>();
            fsm.RegistryState(new AttributesSelectionState(gameFlow, this));
            fsm.RegistryState(new FightState(gameFlow, this));
        }

        protected override async UniTask OnEnter()
        {
            Game.Instance.GameUIRoot.gameObject.SetActive(true);
            
            fsm.ToStateWithParams<AttributesSelectionState>(Arguments);
        }
        
        protected override async UniTask OnExit()
        {
            Game.Instance.GameUIRoot.gameObject.SetActive(false);
            fsm.Exit();
        }

        public void EnterFightState() => UniTask.Create(async () =>
        {
            await gameFlow.ShowBlackScreen();
            fsm.ToState<FightState>();
            await gameFlow.HideBlackScreen();
        });

        public async UniTask WinFightState()
        {
            await gameFlow.ShowBlackScreen();
            gameFlow.GameState.ChallengeIndex++;
            if (gameFlow.GameState.ChallengeIndex == gameSettings.Challenges.Length)
            {
                Debug.Log("You win!");
                fsm.ToState<SelectBuddyState>().Forget();
                return;
            }
            
            fsm.ToState<FightState>().Forget();
            await gameFlow.HideBlackScreen();
        }
    }
}