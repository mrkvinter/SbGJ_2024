using System.Collections.Generic;
using System.Linq;
using Code.Dices;
using Code.DiceSets;
using Code.Utilities;
using Cysharp.Threading.Tasks;
using Game.Core.FinalStateMachine;
using GameAnalyticsSDK;
using KvinterGames;
using RG.ContentSystem.Core;
using UnityEngine;

namespace Code.States
{
    public class SelectDiceFromDiceSet : BaseState<DiceSetShopEntry>
    {
        private GameFlow gameFlow;
        private ShopState shopState;
        private GameRunState gameRunState;
        private List<DiceState> currentDices;

        private int countToSelect;

        public SelectDiceFromDiceSet(GameFlow gameFlow, GameRunState gameRunState, ShopState shopState)
        {
            this.gameFlow = gameFlow;
            this.gameRunState = gameRunState;
            this.shopState = shopState;
        }

        protected override async UniTask OnEnter()
        {
            var dicesByRarity = ContentManager.GetContentMap<DiceEntry>().ContentEntries
                .GroupBy(d => d.DiceRank).ToDictionary(g => g.Key, g => g.ToList());
            var dices = new List<DiceState>();
            countToSelect = Arguments.DiceToSelect;
            var fullProbability = Arguments.Probabilities.Sum(e => e.Probability);
            
            currentDices = dices;

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
                diceState.DiceView.IsDraggable = false;
                diceState.OnClick += OnDiceSelected;
                currentDices.Add(diceState);

                diceState.DiceView.SetDiceHolderParent(Game.Instance.handDiceHolder);
                Game.Instance.handDiceHolder.Occupy(diceState.DiceView);
            }
        }

        protected override async UniTask OnExit()
        {
            for (var i = 0; i < currentDices.Count; i++)
            {
                currentDices[i].OnClick -= OnDiceSelected;
            }

            for (var i = Game.Instance.handDiceHolder.Dices.Count - 1; i >= 0; i--)
            {
                var dice = Game.Instance.handDiceHolder.Dices[i];
                dice.DiceState.DestroyDice().Forget();
            }
        }

        protected void OnDiceSelected(DiceState diceState)
        {
            SoundController.Instance.PlaySound("dice_deal", 0.1f, 0.7f);
            GameAnalytics.NewDesignEvent($"shop:dice_selected:{diceState.DiceEntry.Id}");

            countToSelect--;
            gameFlow.GameState.Dices.Add(diceState);
            diceState.DestroyDice().Forget();
            if (!gameRunState.IsFirstShop)
            {
                shopState.CurrentDices.Add(diceState);
            }

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