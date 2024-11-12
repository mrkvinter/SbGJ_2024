using System.Linq;
using System.Threading.Tasks;
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
        public IFsm fsm;

        private GameSettings gameSettings;

        public Buddy Buddy { get; private set; }
        public bool IsFirstShop { get; private set; }

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
            gameFlow.ClearState();
            gameFlow.GameState.Coins = gameSettings.StartCoins;
            IsFirstShop = true;
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
            await fsm.Exit();

            Object.Destroy(Buddy.BuddyView.gameObject);
            Buddy = null;
            gameFlow.GameState.Buddy = null;
            Game.Instance.GameUIRoot.gameObject.SetActive(false);
        }

        public void EnterFightState() => UniTask.Create(async () =>
        {
            await gameFlow.ShowBlackScreen();
            fsm.ToState<FightState>();
            await gameFlow.HideBlackScreen();
        });

        public async UniTask WinFightState()
        {
            var buddyEntry = gameFlow.GameState.Buddy.BuddyEntry;
            var challenge = buddyEntry.Challenges[gameFlow.GameState.ChallengeIndex].Unwrap();
            for (int i = 0; i < challenge.CoinsReward; i++)
            {
                gameFlow.GameState.Coins++;
                Game.Instance.GoldCountText.text = gameFlow.GameState.Coins.ToString();
                SoundController.Instance.PlaySound("coin", 0.1f);
                await Game.Instance.GoldCountText.transform.DOShakeScale(0.5f, 0.1f).ToUniTask();
            }
            
            await UniTask.Delay(500);
            gameFlow.GameState.ChallengeIndex++;

            if (gameFlow.GameState.ChallengeIndex == buddyEntry.Challenges.Length) //win
            {
                var buddyCompleted = PlayerPrefs.GetInt("BuddyCompleted", 0);
                var currentBuddy = buddyEntry.Index;
                if (buddyCompleted < currentBuddy)
                {
                    PlayerPrefs.SetInt("BuddyCompleted", currentBuddy);
                }

                var hasTwist = buddyEntry.HasTwist;
                if (hasTwist)
                {
                    await Game.Instance.RealWorldService.ShowRealWorld(buddyEntry.TwistIndex);
                    
                    gameFlow.fsm.ToState<SelectBuddyState>().Forget();

                }
                else
                {
                    await Game.Instance.DialoguePanel.ShowDialogueAsync(GameTexts.BuddyWinDialogue);
                    
                    await gameFlow.ShowBlackScreen();
                    gameFlow.fsm.ToState<SelectBuddyState>().Forget();
                    await gameFlow.HideBlackScreen();
                }

                // await gameFlow.ShowBlackScreen();
                // gameFlow.fsm.ToState<SelectBuddyState>().Forget();
                // await gameFlow.HideBlackScreen();

                return;
            }

            await gameFlow.ShowBlackScreen();
            var shop = ContentManager.GetContentMap<DiceSetShopEntry>().ContentEntries.ToArray();
            fsm.ToStateWithParams<ShopState>(shop).Forget();
            await gameFlow.HideBlackScreen();
        }
        
        public void OnShopClosed()
        {
            if (IsFirstShop)
            {
                IsFirstShop = false;
                fsm.ToState<AttributesSelectionState>();
            }
            else
            {
                gameFlow.GameState.Buddy.BuddyView.HpDiceHandHolder.Lock();
                gameFlow.GameState.Buddy.BuddyView.ShieldDiceHandHolder.Lock();
                fsm.ToState<FightState>();
            }
        }

        public async Task LoseFightState()
        {
            await gameFlow.ShowBlackScreen();
            await gameFlow.fsm.ToState<SelectBuddyState>();
            await gameFlow.HideBlackScreen();
        }
    }
}