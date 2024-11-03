using System.Linq;
using Code.Buddies;
using Code.DiceSets;
using Code.Utilities;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Core.FinalStateMachine;
using KvinterGames;
using RG.ContentSystem.Core;
using UnityEngine;

namespace Code.States
{
    public class GameRunState : BaseState<ContentRef<BuddyEntry>>
    {
        private GameFlow gameFlow;
        private IFsm fsm;

        private GameSettings gameSettings;
        
        private bool isFirstShop;

        public Buddy Buddy { get; private set; }

        public GameRunState(GameFlow gameFlow)
        {
            this.gameFlow = gameFlow;
            fsm = new Fsm();
            
            gameSettings = ContentManager.GetSettings<GameSettings>();
            fsm.RegistryState(new AttributesSelectionState(gameFlow, this));
            fsm.RegistryState(new FightState(gameFlow, this));
            fsm.RegistryState(new ShopState(gameFlow, this));
        }

        protected override async UniTask OnEnter()
        {
            Game.Instance.GameUIRoot.gameObject.SetActive(true);
            gameFlow.GameState.Coins = gameSettings.StartCoins;
            isFirstShop = true;
            Game.Instance.GoldCountText.text = gameFlow.GameState.Coins.ToString();
            
            var buddyEntry = ContentManager.GetContent(Arguments);
            var view = Object.Instantiate(buddyEntry.Prefab, Game.Instance.BuddyPoint);
            view.transform.localPosition = Vector3.zero;
            Buddy = new Buddy(buddyEntry, view);
            gameFlow.GameState.Buddy = Buddy;

            var shop = ContentManager.GetContentMap<DiceSetShopEntry>().ContentEntries.ToArray();
            fsm.ToStateWithParams<ShopState>(shop);
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
            var challenge = gameSettings.Challenges[gameFlow.GameState.ChallengeIndex].Unwrap();
            for (int i = 0; i < challenge.CoinsReward; i++)
            {
                gameFlow.GameState.Coins++;
                Game.Instance.GoldCountText.text = gameFlow.GameState.Coins.ToString();
                SoundController.Instance.PlaySound("coin", 0.1f);
                await Game.Instance.GoldCountText.transform.DOShakeScale(0.5f, 0.1f).ToUniTask();
            }
            
            await UniTask.Delay(500);
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
        
        public void OnShopClosed()
        {
            if (isFirstShop)
            {
                isFirstShop = false;
                fsm.ToState<AttributesSelectionState>();
            }
            else
            {
                fsm.ToState<FightState>();
            }
        }
    }
}