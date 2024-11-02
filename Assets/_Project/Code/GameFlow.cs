using System.Collections.Generic;
using Code.Buddies;
using Code.Dices;
using Code.States;
using Code.Utilities;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Core.FinalStateMachine;
using RG.ContentSystem.Core;
using UnityEngine;

namespace Code
{
    public class GameState
    {
        public Buddy Buddy;
        public List<DiceState> Dices = new();
        public List<DiceState> Bag = new();
        public List<DiceState> Hand = new();
        public int DrawnDicesCount = 3;
        public int CurrentDrawnDicesCount = 0;
        
        public void ShuffleBag()
        {
            Bag.Sort((_, _) => Random.Range(-1, 1));
        }
    }

    public class GameFlow
    {
        private Game game;
        private GameState gameState;
        private IFsm fsm;
        
        
        public GameState GameState => gameState;

        public GameFlow(Game game)
        {
            this.game = game;
        }

        public void StartGame()
        {
            gameState = new GameState();
            fsm = new Fsm();
            fsm.RegistryState(new FightState(this));
            fsm.RegistryState(new AttributesSelectionState(this));

            gameState.Dices.Add(new DiceState(DiceType.D4.As<DiceEntry>()));
            gameState.Dices.Add(new DiceState(DiceType.D4.As<DiceEntry>()));
            gameState.Dices.Add(new DiceState(DiceType.D4.As<DiceEntry>()));
            gameState.Dices.Add(new DiceState(DiceType.D4.As<DiceEntry>()));
            gameState.Dices.Add(new DiceState(DiceType.D4.As<DiceEntry>()));
            gameState.Dices.Add(new DiceState(DiceType.D4.As<DiceEntry>()));
            
            gameState.Dices.Add(new DiceState(DiceType.D6.As<DiceEntry>()));
            gameState.Dices.Add(new DiceState(DiceType.D6.As<DiceEntry>()));
            gameState.Dices.Add(new DiceState(DiceType.D6.As<DiceEntry>()));

            // gameState.Dices.Add(new DiceState(DiceType.D8.As<DiceEntry>()));
            //
            // gameState.Dices.Add(new DiceState(DiceType.D12.As<DiceEntry>()));
            // gameState.Dices.Add(new DiceState(DiceType.D20.As<DiceEntry>()));

            fsm.ToStateWithParams<AttributesSelectionState>(new ContentRef<BuddyEntry>("BuddyHeart")).Forget();
        }

        private async UniTask StartTurn()
        {
            gameState.Hand.Clear();
            gameState.Bag.Clear();
            gameState.Bag.AddRange(gameState.Dices);
            game.DiceInBagText.text = gameState.Bag.Count.ToString();
            ShuffleBag();
            gameState.CurrentDrawnDicesCount = gameState.DrawnDicesCount;

            await DrawHand();
        }

        public async UniTask DrawHand()
        {
            for (var i = 0; i < gameState.CurrentDrawnDicesCount; i++)
            {
                await RollDice();
            }
        }
        
        private async UniTask ReturnDicesToBag(List<DiceState> playedDices)
        {
            //показать впервый раз текст: ты опустошил мешок, я перемешаю его, но теперь ты будешь тянуть на 1 кубик меньше
            foreach (var diceState in gameState.Dices)
            {
                if (diceState.DiceView != null)
                {
                    Object.Destroy(diceState.DiceView.gameObject);
                    diceState.ClearView();
                }
                
                if (!playedDices.Contains(diceState))
                {
                    gameState.Bag.Add(diceState);
                }
            }
            
            ShuffleBag();
            gameState.CurrentDrawnDicesCount--;
        }

        public async UniTask RollDice(bool rollMaxValue = false)
        {
            if (gameState.Bag.Count == 0)
            {
                await ReturnDicesToBag(gameState.Hand);
            }
            var diceState = gameState.Bag[^1];
            gameState.Bag.RemoveAt(gameState.Bag.Count - 1);
            gameState.Hand.Add(diceState);
                
            diceState.SetView(Object.Instantiate(diceState.DiceEntry.DicePrefab));
            
            var diceValue = rollMaxValue ? diceState.DiceEntry.MaxDiceValue : Random.Range(1, diceState.DiceEntry.MaxDiceValue + 1);
            diceState.SetValue(diceValue);
                
            diceState.DiceView.SetDiceHolderParent(game.handDiceHolder);
            game.handDiceHolder.Occupy(diceState.DiceView);
            game.DiceInBagText.text = gameState.Bag.Count.ToString();

            await UniTask.Delay(100);
        }
        

        private void ShuffleBag()
        {
            gameState.Bag.Sort((_, _) => Random.Range(-1, 1));
        }

        public void EnterFightState() => fsm.ToState<FightState>().Forget();
    }
}