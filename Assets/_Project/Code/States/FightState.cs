using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Core.FinalStateMachine;
using UnityEngine;

namespace Code.States
{
    public class FightState : BaseState
    {
        private GameFlow gameFlow;
        
        public FightState(GameFlow gameFlow)
        {
            this.gameFlow = gameFlow;
        }

        protected override async UniTask OnEnter()
        {
            Debug.Log("FightState OnEnter");

            Game.Instance.AttackButton.gameObject.SetActive(true);
            Game.Instance.AttackButton.onClick.AddListener(OnAttackButtonClicked);
            
            await StartTurn();
        }

        protected override async UniTask OnExit()
        {
            Game.Instance.AttackButton.gameObject.SetActive(false);
            Game.Instance.AttackButton.onClick.RemoveListener(OnAttackButtonClicked);
        }

        private void OnAttackButtonClicked() => Attack().Forget();

        private async UniTask Attack()
        {
            Game.Instance.AttackButton.interactable = false;
            var game = Game.Instance;
            var attackAmount = 0;
            for (var i = 0; i < game.attackDiceHolder.Dices.Count; i++)
            {
                var dice = game.attackDiceHolder.Dices[i];
                gameFlow.GameState.Hand.Remove(dice.DiceState);
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
                dice.DiceState.DestroyDice().Forget();
            }
            
            EndTurn().Forget();

            Game.Instance.AttackButton.interactable = true;
        }

        private async UniTask StartTurn()
        {
            var gameState = gameFlow.GameState;
            gameState.Hand.Clear();
            gameState.Bag.Clear();
            gameState.Bag.AddRange(gameState.Dices);
            Game.Instance.DiceInBagText.text = gameState.Bag.Count.ToString();
            gameState.ShuffleBag();
            gameState.CurrentDrawnDicesCount = gameState.DrawnDicesCount;

            await gameFlow.DrawHand();
        }
        
        private async UniTask EndTurn()
        {
            foreach (var diceState in gameFlow.GameState.Hand)
            {
                diceState.DestroyDice().Forget();
            }

            gameFlow.GameState.Hand.Clear();
            await gameFlow.DrawHand();
        }
    }
}