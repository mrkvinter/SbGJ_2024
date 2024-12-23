﻿using System.Collections.Generic;
using Code.Buddies;
using Code.Dices;
using Code.States;
using Code.Utilities;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Core.FinalStateMachine;
using KvinterGames;
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
        public int DrawnDicesCount = 4;
        public int CurrentDrawnDicesCount = 0;
        public int ChallengeIndex = 0;
        public int Coins = 0;
        public bool EmptyCupTipShown = false;

        public void ShuffleBag()
        {
            Bag.Sort((_, _) => Random.Range(-1, 1));
        }
    }

    public class GameFlow
    {
        private Game game;
        private GameState gameState;
        public IFsm fsm;
        private GameSettings gameSettings;
        
        public GameState GameState => gameState;
        
        private SoundController.LoopSound ambientNoise;

        public GameFlow(Game game)
        {
            this.game = game;
        }

        public void Tick()
        {
            var damage = 0;
            foreach (var dice in game.attackDiceHolder.Dices)
            {
                damage += dice.DiceState.Value;
            }
            game.damageText.text = damage.ToString();
        }

        public void StartGame()
        {
            ambientNoise = SoundController.Instance.PlayLoop("amb_noise");
            ambientNoise.AudioSource.volume = 1f;
            ChangePitch();

            ShowBlackScreen(true).Forget();

            gameSettings = ContentManager.GetSettings<GameSettings>();
            gameState = new GameState();
            gameState.EmptyCupTipShown = PlayerPrefs.GetInt("EmptyCupTipShown", 0) == 1;
            fsm = new Fsm();
            // fsm.RegistryState(new FightState(this));
            // fsm.RegistryState(new AttributesSelectionState(this));
            fsm.RegistryState(new SelectBuddyState(this));
            fsm.RegistryState(new GameRunState(this));
            
            foreach (var dice in ContentManager.GetSettings<GameSettings>().StartingDiceSet.Unwrap().DiceEntries)
            {
                gameState.Dices.Add(new DiceState(dice.Unwrap()));
            }
            
            fsm.ToState<SelectBuddyState>().Forget();
            HideBlackScreen().Forget();
            // fsm.ToStateWithParams<AttributesSelectionState>(new ContentRef<BuddyEntry>("BuddyHeart")).Forget();
        }

        public void ClearState()
        {
            gameState = new GameState();
            foreach (var dice in ContentManager.GetSettings<GameSettings>().StartingDiceSet.Unwrap().DiceEntries)
            {
                gameState.Dices.Add(new DiceState(dice.Unwrap()));
            }
            
            gameState.EmptyCupTipShown = PlayerPrefs.GetInt("EmptyCupTipShown", 0) == 1;
        }

        private void ChangePitch()
        {
            ambientNoise.AudioSource.DOPitch(Random.Range(0.8f, 1.2f), 2).SetEase(Ease.Linear).OnComplete(ChangePitch);
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
        
        public async UniTask DrawHand(int count, bool rollMaxValue = false)
        {
            for (var i = 0; i < count; i++)
            {
                await RollDice(rollMaxValue);
            }
        }
        
        private async UniTask ReturnDicesToBag(List<DiceState> playedDices)
        {
            Debug.Log("ReturnDicesToBag");

            //показать впервый раз текст: ты опустошил мешок, я перемешаю его, но теперь ты будешь тянуть на 1 кубик меньше
            if (!gameState.EmptyCupTipShown)
            {
                gameState.EmptyCupTipShown = true;
                PlayerPrefs.SetInt("EmptyCupTipShown", 1);
                await Game.Instance.DialoguePanel.ShowDialogueAsync(GameTexts.no_dices);
            }

            foreach (var diceState in gameState.Dices)
            {
                if (!playedDices.Contains(diceState) && diceState.CanBePlayed)
                {
                    gameState.Bag.Add(diceState);
                }
            }
            
            ShuffleBag();
            gameState.CurrentDrawnDicesCount--;
        }

        public async UniTask RollDice(bool rollMaxValue = false)
        {
            Debug.Log("RollDice");
            if (gameState.Bag.Count == 0)
            {
                await ReturnDicesToBag(gameState.Hand);
            }
            
            if (gameState.Bag.Count == 0)
            {
                Debug.Log("Not enough dices in bag");
                return;
            }

            SoundController.Instance.PlaySound("dice_deal", 0.1f, 0.7f, pitch: 1.25f);
            var diceState = gameState.Bag[^1];
            gameState.Bag.RemoveAt(gameState.Bag.Count - 1);
            gameState.Hand.Add(diceState);
                
            diceState.SetView(Object.Instantiate(diceState.DiceEntry.DicePrefab));
            
            var diceValue = rollMaxValue ? diceState.MaxDiceValue : Random.Range(1, diceState.MaxDiceValue + 1);
            if (diceState.DiceEntry.Duplicator)
            {
                diceValue = 0;
            }

            diceState.SetValue(diceValue);
                
            diceState.DiceView.SetDiceHolderParent(game.handDiceHolder);
            game.handDiceHolder.Occupy(diceState.DiceView);
            game.DiceInBagText.text = gameState.Bag.Count.ToString();

            await UniTask.Delay(50);
        }
        

        private void ShuffleBag()
        {
            gameState.Bag.Sort((_, _) => Random.Range(-1, 1));
        }

        public void StartWithBuddy(ContentRef<BuddyEntry> buddy)
        {
            fsm.ToStateWithParams<GameRunState>(buddy).Forget();
        }
        
        public async UniTask ShowBlackScreen(bool instant = false)
        {
            if (instant)
            {
                game.BlackScreen.gameObject.SetActive(true);
                game.BlackScreen.color = new Color(0, 0, 0, 1);
                return;
            }
            game.BlackScreen.gameObject.SetActive(true);
            await game.BlackScreen.DOFade(1, .5f).SetEase(Ease.Linear).ToUniTask();
        }
        
        public async UniTask HideBlackScreen()
        {
            await game.BlackScreen.DOFade(0, .5f).SetEase(Ease.Linear).ToUniTask();
            game.BlackScreen.gameObject.SetActive(false);
        }
    }
}