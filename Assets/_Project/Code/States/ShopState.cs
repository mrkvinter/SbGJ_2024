using System.Collections.Generic;
using System.Linq;
using Code.Dices;
using Code.DiceSets;
using Cysharp.Threading.Tasks;
using Game.Core.FinalStateMachine;
using GameAnalyticsSDK;
using KvinterGames;
using UnityEngine;

namespace Code.States
{
    public class ShopState : BaseState<DiceSetShopEntry[]>
    {
        private GameFlow gameFlow;
        private GameRunState gameRunState;
        private IFsm fsm;
        private List<ShopItem> shopItems;
        
        public readonly List<DiceState> CurrentDices = new();

        public ShopState(GameFlow gameFlow, GameRunState gameRunState)
        {
            this.gameFlow = gameFlow;
            this.gameRunState = gameRunState;

            fsm = new Fsm();
            fsm.RegistryState(new SelectDiceFromDiceSet(gameFlow, gameRunState, this));
        }

        protected override async UniTask OnEnter()
        {
            if (gameFlow.GameState.Buddy.BuddyEntry.IsTutorialBuddy && gameRunState.IsFirstShop)
            {
                await Game.Instance.DialoguePanel.ShowDialogueAsync(GameTexts.tutor_first);
            }

            CurrentDices.Clear();
            if (!gameRunState.IsFirstShop)
            {
                await gameFlow.DrawHand(5, true);
                CurrentDices.AddRange(gameFlow.GameState.Dices);
            }

            Game.Instance.AttackButton.gameObject.SetActive(true);
            Game.Instance.AttackButton.onClick.AddListener(HandleAttackButton);
            Game.Instance.SetTextToButton(Texts.BattleButton);
            
            gameFlow.GameState.Buddy.BuddyView.HpDiceHandHolder.Unlock();
            gameFlow.GameState.Buddy.BuddyView.ShieldDiceHandHolder.Unlock();

            shopItems = new List<ShopItem>();
            for (var i = 0; i < Arguments.Length; i++)
            {
                var shopEntry = Arguments[i];
                var shopEntryView = Game.Instance.ShopSlots[i];
                shopEntryView.SetContent(shopEntry);
                shopEntryView.gameObject.SetActive(true);
                shopEntryView.OnSelected += OnDiceSetSelected;

                shopItems.Add(shopEntryView);
            }
        }

        private void HandleAttackButton()
        {
            if (!gameRunState.IsFirstShop && gameFlow.GameState.Buddy.BuddyView.HpDiceHandHolder.Dices.Count == 0)
            {
                Game.Instance.DialoguePanel.ShowDialogue(GameTexts.no_hp_dice);
                return;
            }
            gameRunState.OnShopClosed();
        }

        protected override async UniTask OnExit()
        {
            Game.Instance.AttackButton.gameObject.SetActive(false);
            Game.Instance.AttackButton.onClick.RemoveListener(HandleAttackButton);
            Game.Instance.DialoguePanel.Clear();

            for (var i = 0; i < Game.Instance.ShopSlots.Length; i++)
            {
                Game.Instance.ShopSlots[i].gameObject.SetActive(false);
            }
            
            for (var i = Game.Instance.handDiceHolder.Dices.Count - 1; i >= 0; i--)
            {
                var currentDice = Game.Instance.handDiceHolder.Dices[i];
                if (!gameFlow.GameState.Dices.Contains(currentDice.DiceState))
                {
                    gameFlow.GameState.Dices.Add(currentDice.DiceState);
                }
                
                await currentDice.DiceState.DestroyDice();
            }

            var buddy = gameFlow.GameState.Buddy;
            foreach (var dice in buddy.BuddyView.HpDiceHandHolder.Dices)
            {
                gameFlow.GameState.Dices.Remove(dice.DiceState);
            }
            
            foreach (var dice in buddy.BuddyView.ShieldDiceHandHolder.Dices)
            {
                gameFlow.GameState.Dices.Remove(dice.DiceState);
            }
        }

        public void OnDiceSetSelected(ShopItem shopItem) => UniTask.Create(async () =>
        {
            if (Game.Instance.GameFlow.GameState.Coins < shopItem.DiceSetShopEntry.Price)
            {
                return;
            }

            Game.Instance.AttackButton.gameObject.SetActive(false);
            GameAnalytics.NewDesignEvent($"shop:dice_set_selected:{shopItem.DiceSetShopEntry.Id}");

            for (var i = 0; i < Game.Instance.ShopSlots.Length; i++)
            {
                Game.Instance.ShopSlots[i].gameObject.SetActive(false);
            }
            
            gameFlow.GameState.Buddy.BuddyView.HpDiceHandHolder.Lock();
            gameFlow.GameState.Buddy.BuddyView.ShieldDiceHandHolder.Lock();

            shopItems.Remove(shopItem);
            gameFlow.GameState.Coins -= shopItem.DiceSetShopEntry.Price;
            Game.Instance.GoldCountText.text = gameFlow.GameState.Coins.ToString();
            SoundController.Instance.PlaySound("coin", 0.1f, pitch: 0.7f);

            CurrentDices.Clear();
            CurrentDices.AddRange(Game.Instance.handDiceHolder.Dices.Select(e => e.DiceState));
            for (var i = Game.Instance.handDiceHolder.Dices.Count - 1; i >= 0; i--)
            {
                var currentDice = Game.Instance.handDiceHolder.Dices[i];
                await currentDice.DiceState.DestroyDice();
            }

            var diceSetEntry = shopItem.DiceSetShopEntry;
            fsm.ToStateWithParams<SelectDiceFromDiceSet>(diceSetEntry);
        });

        public void AllDiceSelected()
        {
            fsm.Exit();
            Game.Instance.AttackButton.gameObject.SetActive(true);

            for (var i = 0; i < shopItems.Count; i++)
            {
                shopItems[i].gameObject.SetActive(true);
                shopItems[i].UpdateColorPrice();
            }

            foreach (var currentDice in CurrentDices)
            {
                currentDice.SetView(Object.Instantiate(currentDice.DiceEntry.DicePrefab));
                currentDice.DiceView.SetDiceHolderParent(Game.Instance.handDiceHolder);
                Game.Instance.handDiceHolder.Occupy(currentDice.DiceView);
            }
            
            gameFlow.GameState.Buddy.BuddyView.HpDiceHandHolder.Unlock();
            gameFlow.GameState.Buddy.BuddyView.ShieldDiceHandHolder.Unlock();
        }
    }
}