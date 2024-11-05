using System.Linq;
using Code.Buddies;
using Code.Utilities;
using Cysharp.Threading.Tasks;
using Game.Core.FinalStateMachine;
using RG.ContentSystem.Core;
using UnityEngine;

namespace Code.States
{
    public class AttributesSelectionState : BaseState
    {
        private GameFlow gameFlow;
        private GameRunState gameRunState;

        private bool isBusy;

        public AttributesSelectionState(GameFlow gameFlow, GameRunState gameRunState)
        {
            this.gameFlow = gameFlow;
            this.gameRunState = gameRunState;
        }

        protected override async UniTask OnEnter()
        {

            gameFlow.GameState.Bag.Clear();
            gameFlow.GameState.Bag.AddRange(gameFlow.GameState.Dices);

            UniTask task = UniTask.CompletedTask;
            if (gameFlow.GameState.Buddy.BuddyEntry.IsTutorialBuddy)
            {
                task = Game.Instance.DialoguePanel.ShowDialogueAsync(GameTexts.tutor_second);
            }

            while (gameFlow.GameState.Bag.Count > 0)
            {
                await gameFlow.RollDice(true);
            }

            if (ContentManager.GetSettings<GameSettings>().AutoSelectHpAndShield)
            {
                var buddy = gameRunState.Buddy;
                while (buddy.BuddyView.HpDiceHandHolder.Dices.Count < buddy.BuddyEntry.HealthDiceCount)
                {
                    var dice = gameFlow.GameState.Hand[^1];
                    gameFlow.GameState.Hand.Remove(dice);
                    dice.DeOccupy();
                    dice.Occupy(buddy.BuddyView.HpDiceHandHolder);
                }

                while (buddy.BuddyView.ShieldDiceHandHolder.Dices.Count < buddy.BuddyEntry.ShieldDiceCount)
                {
                    var dice = gameFlow.GameState.Hand[^1];
                    gameFlow.GameState.Hand.Remove(dice);
                    dice.DeOccupy();
                    dice.Occupy(buddy.BuddyView.ShieldDiceHandHolder);
                }
            }
            
            Game.Instance.AttackButton.gameObject.SetActive(true);
            Game.Instance.AttackButton.onClick.AddListener(OnAttackButtonClicked);
        }

        protected override async UniTask OnExit()
        {
            Game.Instance.AttackButton.gameObject.SetActive(false);
            Game.Instance.AttackButton.onClick.RemoveListener(OnAttackButtonClicked);
            
            Game.Instance.DialoguePanel.Clear();
        }

        private void OnAttackButtonClicked() => UniTask.Create(async () =>
        {
            if (isBusy)
                return;
            

            if (!gameRunState.Buddy.BuddyView.HpDiceHandHolder.IsFull())
            {
                Debug.Log("HP dice hand is not full");
                Game.Instance.DialoguePanel.ShowDialogueAsync(GameTexts.not_enough_hp_dice);
                //TODO: Show error message
                return;
            }
            
            if (!gameRunState.Buddy.BuddyView.ShieldDiceHandHolder.IsFull())
            {
                Debug.Log("Shield dice hand is not full");
                Game.Instance.DialoguePanel.ShowDialogueAsync(GameTexts.not_enough_shield_dice);
                //TODO: Show error message
                return;
            }
            
            isBusy = true;
            gameRunState.Buddy.BuddyView.HpDiceHandHolder.Lock();
            gameRunState.Buddy.BuddyView.ShieldDiceHandHolder.Lock();
            
            gameFlow.GameState.Dices.Clear();
            gameFlow.GameState.Dices.AddRange(Game.Instance.handDiceHolder.Dices.Select(dice => dice.DiceState));
            
            for (var i = Game.Instance.handDiceHolder.Dices.Count - 1; i >= 0; i--)
            {
                await Game.Instance.handDiceHolder.Dices[i].DiceState.DestroyDice();
            }
            
            gameFlow.GameState.Hand.Clear();
            gameRunState.EnterFightState();
            isBusy = false;
        });
    }
}