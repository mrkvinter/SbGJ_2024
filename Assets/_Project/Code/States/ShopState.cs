using System.Collections.Generic;
using System.Linq;
using Code.Buddies;
using Code.Dices;
using Code.DiceSets;
using Code.Utilities;
using Cysharp.Threading.Tasks;
using Game.Core.FinalStateMachine;
using RG.ContentSystem.Core;
using UnityEngine;

namespace Code.States
{
    public class ShopState : BaseState<DiceSetShopEntry[]>
    {
        private GameFlow gameFlow;
        private GameRunState gameRunState;
        private IFsm fsm;
        private List<ShopItem> shopItems;

        public ShopState(GameFlow gameFlow, GameRunState gameRunState)
        {
            this.gameFlow = gameFlow;

            fsm = new Fsm();
            fsm.RegistryState(new SelectDiceFromDiceSet(gameFlow, this));
        }

        protected override async UniTask OnEnter()
        {
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

        protected override async UniTask OnExit()
        {
            for (var i = 0; i < Game.Instance.ShopSlots.Length; i++)
            {
                Game.Instance.ShopSlots[i].gameObject.SetActive(false);
            }
        }

        public void OnDiceSetSelected(ShopItem shopItem)
        {
            for (var i = 0; i < Game.Instance.ShopSlots.Length; i++)
            {
                Game.Instance.ShopSlots[i].gameObject.SetActive(false);
            }

            shopItems.Remove(shopItem);
            gameFlow.GameState.Coins -= shopItem.DiceSetShopEntry.Price;
            var diceSetEntry = shopItem.DiceSetShopEntry;
            fsm.ToStateWithParams<SelectDiceFromDiceSet>(diceSetEntry);
        }
        
        public void AllDiceSelected()
        {
            fsm.Exit();

            for (var i = 0; i < shopItems.Count; i++)
            {
                shopItems[i].gameObject.SetActive(true);
            }
        }
    }

    public class SelectDiceFromDiceSet : BaseState<DiceSetShopEntry>
    {
        private GameFlow gameFlow;
        private ShopState shopState;
        private GameRunState gameRunState;
        
        private int countToSelect;
        public SelectDiceFromDiceSet(GameFlow gameFlow, ShopState shopState)
        {
            this.gameFlow = gameFlow;
            this.shopState = shopState;
        }

        protected override async UniTask OnEnter()
        {
            var dicesByRarity = ContentManager.GetContentMap<DiceEntry>().ContentEntries
                .GroupBy(d => d.DiceRank).ToDictionary(g => g.Key, g => g.ToList());
            var dices = new List<DiceState>();
            countToSelect = Arguments.DiceToSelect;
            var fullProbability = Arguments.Probabilities.Sum(e => e.Probability);
            
            for (var i = 0; i < Arguments.DiceCount; i++)
            {
                var random = Random.Range(0, fullProbability);
                var currentProbability = 0f;
                var selectedDice = default(DiceEntry);
                foreach (var probability in Arguments.Probabilities)
                {
                    currentProbability += probability.Probability;
                    if (random < currentProbability)
                    {
                        var diceEntries = dicesByRarity[probability.Rank];
                        selectedDice = diceEntries[Random.Range(0, diceEntries.Count)];
                        break;
                    }
                }

                var diceState = new DiceState(selectedDice);
                diceState.SetView(Object.Instantiate(diceState.DiceEntry.DicePrefab));
                dices.Add(diceState);
                diceState.OnClick += OnDiceSelected;
                
                diceState.DiceView.SetDiceHolderParent(Game.Instance.handDiceHolder);
                Game.Instance.handDiceHolder.Occupy(diceState.DiceView);
            }
        }

        protected override async UniTask OnExit()
        {
            for (var i = Game.Instance.handDiceHolder.Dices.Count - 1; i >= 0; i--)
            {
                var dice = Game.Instance.handDiceHolder.Dices[i];
                dice.DiceState.DestroyDice().Forget();
            }
        }

        protected void OnDiceSelected(DiceState diceState)
        {
            countToSelect--;
            gameFlow.GameState.Dices.Add(diceState);
            diceState.DestroyDice().Forget();
            if (countToSelect == 0)
            {
                shopState.AllDiceSelected();
            }
        }
        public void EnterGameRunState(ContentRef<DiceSetEntry> diceSetEntry) => UniTask.Create(async () =>
        {
            // gameFlow.GameState.DiceSet = new DiceSet(diceSetEntry);
            // await gameFlow.ShowBlackScreen();
            // fsm.ToStateWithParams<GameRunState>(diceSetEntry);
            // await gameFlow.HideBlackScreen();
        });
    }
}