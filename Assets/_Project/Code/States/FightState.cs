using System.Collections.Generic;
using System.Linq;
using Code.Dices;
using Code.Enemies;
using Code.Utilities;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Core.FinalStateMachine;
using RG.ContentSystem.Core;
using UnityEngine;

namespace Code.States
{
    public class FightState : BaseState
    {
        private GameFlow gameFlow;
        private Enemy selectedEnemy;
        
        private List<Enemy> enemies = new();

        public FightState(GameFlow gameFlow)
        {
            this.gameFlow = gameFlow;
        }

        protected override async UniTask OnEnter()
        {
            Debug.Log("FightState OnEnter");

            Game.Instance.AttackButton.gameObject.SetActive(true);
            Game.Instance.AttackButton.onClick.AddListener(OnAttackButtonClicked);

            var gameSettings = ContentManager.GetSettings<GameSettings>();
            var challenge = gameFlow.GameState.ChallengeIndex;
            var challengeEntry = ContentManager.GetContent(gameSettings.Challenges[challenge]);

            if (!challengeEntry.FrontEnemy.IsEmpty)
                SpawnEnemy(challengeEntry.FrontEnemy, Game.Instance.FrontEnemyPoint);

            if (!challengeEntry.LeftEnemy.IsEmpty)
                SpawnEnemy(challengeEntry.LeftEnemy, Game.Instance.LeftEnemyPoint);

            if (!challengeEntry.RightEnemy.IsEmpty)
                SpawnEnemy(challengeEntry.RightEnemy, Game.Instance.RightEnemyPoint);
            
            await StartTurn();
        }
        
        private Enemy SpawnEnemy(ContentRef<EnemyEntry> enemyEntry, Transform point)
        {
            var enemy = ContentManager.GetContent(enemyEntry);
            var enemyView = Object.Instantiate(enemy.Prefab, point);
            enemyView.transform.localPosition = Vector3.zero;
            var spawnEnemy = new Enemy(enemy, enemyView);
            if (selectedEnemy == null)
                OnEnemyClicked(spawnEnemy);

            enemies.Add(spawnEnemy);
            enemyView.OnEnemyClicked += OnEnemyClicked;

            return spawnEnemy;
        }

        private void OnEnemyClicked(Enemy enemy)
        {
            selectedEnemy?.View.Deselect();
            selectedEnemy = enemy;
            selectedEnemy?.View.Select();
        }

        protected override async UniTask OnExit()
        {
            Game.Instance.AttackButton.gameObject.SetActive(false);
            Game.Instance.AttackButton.onClick.RemoveListener(OnAttackButtonClicked);
            
            selectedEnemy = null;
        }

        private void OnAttackButtonClicked() => Attack().Forget();

        private async UniTask Attack()
        {
            Game.Instance.AttackButton.interactable = false;
            
            var game = Game.Instance;
            var attackAmount = 0;
            var calculatedDices = new List<DiceState>();
            for (var i = 0; i < game.attackDiceHolder.Dices.Count; i++)
            {
                var dice = game.attackDiceHolder.Dices[i];
                gameFlow.GameState.Hand.Remove(dice.DiceState);
                calculatedDices.Add(dice.DiceState);
                await dice.DiceState.CalculateValue();

                attackAmount = calculatedDices.Sum(d => d.Value);
                game.damageText.text = attackAmount.ToString();
            }
            var tasks = new List<UniTask>();
            var countFX = attackAmount/4f;
            for (var i = 0; i < countFX; i++)
            {
                var fx = Object.Instantiate(game.attackFx, selectedEnemy.View.transform);
                fx.transform.localPosition = game.attackPoint.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
                var task = fx.transform.DOMove(selectedEnemy.View.transform.position, 0.25f).SetEase(Ease.InSine).SetDelay(Random.Range(0, 0.2f)).ToUniTask()
                    .ContinueWith(() =>
                    {
                        Object.Destroy(fx);
                    });
                tasks.Add(task);
            }
            await UniTask.WhenAll(tasks);
            selectedEnemy.TakeDamage(attackAmount);
            if (selectedEnemy.IsDead)
            {
                enemies.Remove(selectedEnemy);
                selectedEnemy.View.OnEnemyClicked -= OnEnemyClicked;
                await selectedEnemy.Die();
                selectedEnemy = null;
            }

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
                await diceState.DestroyDice();
            }

            gameFlow.GameState.Hand.Clear();

            if (enemies.Count == 0)
            {
                gameFlow.GameState.ChallengeIndex++;
                await gameFlow.WinState();
                return;
            }
            
            await EnemyTurn();
            await UniTask.Delay(1000);
            
            await gameFlow.DrawHand();
        }
        
        private async UniTask EnemyTurn()
        {
            foreach (var enemy in enemies)
            {
                var damage = enemy.EnemyEntry.DamageCount;
                await gameFlow.GameState.Buddy.TakeDamage(damage);
            }
        }
    }
}