﻿using System.Collections.Generic;
using System.Linq;
using Code.Dices;
using Code.Enemies;
using Code.Utilities;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Core.FinalStateMachine;
using GameAnalyticsSDK;
using KvinterGames;
using RG.ContentSystem.Core;
using UnityEngine;

namespace Code.States
{
    public class FightState : BaseState
    {
        private GameFlow gameFlow;
        private GameRunState gameRunState;
        private Enemy selectedEnemy;

        private List<Enemy> enemies = new();

        private ChallengeEntry challengeEntry;
        private bool isBusy;
        private bool isWin;
        
        public IReadOnlyCollection<Enemy> Enemies => enemies;

        public FightState(GameFlow gameFlow, GameRunState gameRunState)
        {
            this.gameFlow = gameFlow;
            this.gameRunState = gameRunState;
        }

        protected override async UniTask OnEnter()
        {
            Debug.Log("FightState OnEnter");
            
            
            isWin = false;
            Game.Instance.SetTextToButton(Texts.EndRoundButton);
            var challenge = gameFlow.GameState.ChallengeIndex;
            challengeEntry = ContentManager.GetContent(gameFlow.GameState.Buddy.BuddyEntry.Challenges[challenge]);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, gameRunState.Buddy.BuddyEntry.Id, challengeEntry.Id);

            if (!challengeEntry.FrontEnemy.IsEmpty)
                SpawnEnemy(challengeEntry.FrontEnemy, Game.Instance.FrontEnemyPoint);

            if (!challengeEntry.LeftEnemy.IsEmpty)
                SpawnEnemy(challengeEntry.LeftEnemy, Game.Instance.LeftEnemyPoint);

            if (!challengeEntry.RightEnemy.IsEmpty)
                SpawnEnemy(challengeEntry.RightEnemy, Game.Instance.RightEnemyPoint);

            if (gameFlow.GameState.Buddy.BuddyEntry.IsTutorialBuddy)
                await Game.Instance.DialoguePanel.ShowDialogueAsync(GameTexts.tutor_third);

            await StartTurn();
            
            Game.Instance.AttackButton.gameObject.SetActive(true);
            Game.Instance.AttackButton.onClick.AddListener(OnAttackButtonClicked);
            Game.Instance.AttackHolder.gameObject.SetActive(true);
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
            GameAnalytics.NewProgressionEvent(isWin ? GAProgressionStatus.Complete : GAProgressionStatus.Fail,
                gameRunState.Buddy.BuddyEntry.Id, challengeEntry.Id);

            for (var i = 0; i < enemies.Count; i++)
            {
                Object.Destroy(enemies[i].View.gameObject);
            }
            
            enemies.Clear();
            Game.Instance.AttackButton.gameObject.SetActive(false);
            Game.Instance.AttackButton.onClick.RemoveListener(OnAttackButtonClicked);
            Game.Instance.AttackHolder.gameObject.SetActive(false);

            selectedEnemy = null;
        }

        private void OnAttackButtonClicked() => Attack().Forget();

        private async UniTask Attack()
        { 
            if (isBusy)
                return;

            isBusy = true;
            Game.Instance.AttackButton.gameObject.SetActive(false);
            Game.Instance.AttackButton.interactable = false;
            if (Game.Instance.attackDiceHolder.Dices.Count == 0)
            {
                await EndTurn();
                isBusy = false;
                Game.Instance.AttackButton.gameObject.SetActive(true);
                Game.Instance.AttackButton.interactable = true;

                return;
            }
            

            Game.Instance.DialoguePanel.Clear();
            Game.Instance.AttackButton.gameObject.SetActive(false);
            Game.Instance.AttackButton.interactable = false;
            isBusy = true;

            var game = Game.Instance;
            var attackAmount = 0;
            var calculatedDices = new List<DiceState>();
            game.damageText.gameObject.SetActive(true);
            for (var i = 0; i < game.attackDiceHolder.Dices.Count; i++)
            {
                var dice = game.attackDiceHolder.Dices[i];
                gameFlow.GameState.Hand.Remove(dice.DiceState);
                calculatedDices.Add(dice.DiceState);
                await dice.DiceState.CalculateValue(() =>
                {
                    attackAmount = calculatedDices.Sum(d => d.Value);
                });

                // attackAmount = calculatedDices.Sum(d => d.Value);
                // game.damageText.text = attackAmount.ToString();
            }

            var tasks = new List<UniTask>();
            var countFX = attackAmount / 4f;
            for (var i = 0; i < countFX; i++)
            {
                var fx = Object.Instantiate(game.attackFx);
                fx.transform.position = game.attackPoint.position +
                                        new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
                fx.transform.position = fx.transform.position.WithZ(-0.1f);
                var task = fx.transform.DOMove(selectedEnemy.View.transform.position.WithZ(-0.1f), 0.2f)
                    .SetEase(Ease.InSine).SetDelay(Random.Range(0, 1f)).ToUniTask()
                    .ContinueWith(() =>
                    {
                        selectedEnemy.View.OnDamage();
                        SoundController.Instance.PlaySound("hit", 0.1f);
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
                if (enemies.Count > 0)
                    OnEnemyClicked(enemies[0]);
            }

            game.damageText.text = string.Empty;

            for (var index = game.attackDiceHolder.Dices.Count - 1; index >= 0; index--)
            {
                var dice = game.attackDiceHolder.Dices[index];
                dice.DiceState.DestroyDice().Forget();
            }

            // if (gameFlow.GameState.Hand.Count == 0 || enemies.Count == 0)
                await EndTurn();

            Game.Instance.AttackButton.interactable = true;
            
            game.damageText.gameObject.SetActive(false);
            isBusy = false;
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

            await StartNextTurn();
        }

        private async UniTask StartNextTurn()
        {
            if (selectedEnemy == null)
                OnEnemyClicked(enemies[0]);

            if (gameFlow.GameState.Buddy.Health <= 0)
            {
                isWin = false;
                await Game.Instance.DialoguePanel.ShowDialogueAsync(GameTexts.death);
                await gameRunState.LoseFightState();
                return;
            }

            await gameFlow.DrawHand();
            
            foreach (var enemy in enemies)
            {
                enemy.OnRoundStart();
            }
            
            Game.Instance.AttackButton.gameObject.SetActive(true);
            Game.Instance.AttackHolder.gameObject.SetActive(true);
        }

        private async UniTask EndTurn()
        {
            foreach (var diceState in gameFlow.GameState.Hand)
            {
                await diceState.DestroyDice();
            }

            foreach (var dice in gameFlow.GameState.Dices)
            {
                dice.OnEndTurn();
            }

            gameFlow.GameState.Hand.Clear();

            if (enemies.Count == 0)
            {
                isWin = true;
                gameFlow.GameState.Buddy.OnFightEnd();
                foreach (var dice in gameFlow.GameState.Dices)
                {
                    dice.OnEndBattle();
                }

                await gameRunState.WinFightState();
                return;
            }

            await EnemyTurn();
            await UniTask.Delay(500);

            await StartNextTurn();
        }

        private async UniTask EnemyTurn()
        {
            foreach (var enemy in enemies)
            {
                var damage = enemy.EnemyEntry.DamageCount;
                var countFX = damage / 4f;
                var tasks = new List<UniTask>();
                for (var i = 0; i < countFX; i++)
                {
                    var fx = Object.Instantiate(Game.Instance.attackFx);
                    fx.transform.position = Game.Instance.attackPoint.position +
                                            new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
                    fx.transform.position = fx.transform.position.WithZ(-0.1f);
                    var nextDice = gameFlow.GameState.Buddy.GetNextDice();
                    if (nextDice == null)
                        break;

                    var task = fx.transform.DOMove(nextDice.DiceView.transform.position.WithZ(-0.1f), 0.2f)
                        .SetEase(Ease.InSine).SetDelay(Random.Range(0, 1)).ToUniTask()
                        .ContinueWith(() =>
                        {
                            SoundController.Instance.PlaySound("hit", 0.1f);
                            Object.Destroy(fx);
                        });
                    tasks.Add(task);
                }

                await UniTask.WhenAll(tasks);
                await gameFlow.GameState.Buddy.TakeDamage(damage);
            }
        }
    }
}