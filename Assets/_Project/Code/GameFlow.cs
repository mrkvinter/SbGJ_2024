using System.Collections.Generic;
using Code.Dices;
using Code.Utilities;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Code
{
    public class GameState
    {
        public List<DiceState> Dices = new();
        public List<DiceState> Bag = new();
        public List<DiceState> Hand = new();
        public int DrawnDicesCount = 3;
        public int CurrentDrawnDicesCount = 0;
    }

    public class GameFlow
    {
        private Game game;
        private GameState gameState;
        
        public GameFlow(Game game)
        {
            this.game = game;
        }

        public void StartGame()
        {
            gameState = new GameState();
            gameState.Dices.Add(new DiceState(DiceType.D4.As<DiceEntry>()));
            gameState.Dices.Add(new DiceState(DiceType.D4.As<DiceEntry>()));
            gameState.Dices.Add(new DiceState(DiceType.D4.As<DiceEntry>()));
            
            gameState.Dices.Add(new DiceState(DiceType.D4.As<DiceEntry>()));
            gameState.Dices.Add(new DiceState(DiceType.D6.As<DiceEntry>()));
            gameState.Dices.Add(new DiceState(DiceType.D6.As<DiceEntry>()));
            gameState.Dices.Add(new DiceState(DiceType.D6.As<DiceEntry>()));
            gameState.Dices.Add(new DiceState(DiceType.D8.As<DiceEntry>()));
            gameState.Dices.Add(new DiceState(DiceType.D12.As<DiceEntry>()));
            gameState.Dices.Add(new DiceState(DiceType.D20.As<DiceEntry>()));
            // gameState.Dices.Add(DiceType.D6); //D8

            StartTurn().Forget();
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

        private async UniTask EndTurn()
        {
            foreach (var diceState in gameState.Hand)
            {
                DestroyDice(diceState).Forget();
            }

            gameState.Hand.Clear();
            await DrawHand();
        }

        private async UniTask DrawHand()
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

        public async UniTask RollDice()
        {
            if (gameState.Bag.Count == 0)
            {
                await ReturnDicesToBag(gameState.Hand);
            }
            var diceState = gameState.Bag[^1];
            gameState.Bag.RemoveAt(gameState.Bag.Count - 1);
            gameState.Hand.Add(diceState);
                
            diceState.SetView(Object.Instantiate(diceState.DiceEntry.DicePrefab));
            
            var diceValue = Random.Range(1, diceState.DiceEntry.MaxDiceValue + 1);
            diceState.SetValue(diceValue);
                
            diceState.DiceView.SetDiceHolderParent(game.handDiceHolder);
            game.handDiceHolder.Occupy(diceState.DiceView);
            game.DiceInBagText.text = gameState.Bag.Count.ToString();

            await UniTask.Delay(100);
        }
        
        public async UniTask Attack()
        {
            var attackAmount = 0;
            for (var i = 0; i < game.attackDiceHolder.Dices.Count; i++)
            {
                var dice = game.attackDiceHolder.Dices[i];
                gameState.Hand.Remove(dice.DiceState);
                attackAmount += dice.DiceState.Value;
                game.damageText.text = attackAmount.ToString();
                await dice.transform.DOLocalMoveY(.25f, 0.1f).ToUniTask();
                await dice.transform.DOLocalMoveY(0, 0.05f).ToUniTask();
                await UniTask.Delay(500);
            }
            var tasks = new List<UniTask>();
            var countFX = attackAmount/4f;
            for (var i = 0; i < countFX; i++)
            {
                var fx = Object.Instantiate(game.attackFx, game.enemy.transform);
                fx.transform.localPosition = game.attackPoint.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
                var task = fx.transform.DOMove(game.enemy.transform.position, 0.25f).SetEase(Ease.InSine).SetDelay(Random.Range(0, 0.2f)).ToUniTask()
                    .ContinueWith(() =>
                    {
                        Object.Destroy(fx);
                    });
                tasks.Add(task);
            }
            await UniTask.WhenAll(tasks);
            game.enemy.TakeDamage(attackAmount);
            game.damageText.text = string.Empty;
            
            for (var index = game.attackDiceHolder.Dices.Count - 1; index >= 0; index--)
            {
                var dice = game.attackDiceHolder.Dices[index];
                DestroyDice(dice.DiceState).Forget();
            }
            
            EndTurn().Forget();
        }
        
        private async UniTask DestroyDice(DiceState diceState)
        {
            var diceView = diceState.DiceView;
            diceState.ClearView();

            var diceHolder = diceView.DiceHolderParent;
            diceHolder.DeOccupy(diceView);
            diceView.transform.DOKill();
            await diceView.transform.DOScale(Vector3.zero, 0.2f).ToUniTask(); 
            Object.Destroy(diceView.gameObject);
        }

        private void ShuffleBag()
        {
            gameState.Bag.Sort((_, _) => Random.Range(-1, 1));
        }
    }
}