using System.Linq;
using Code.Buddies;
using Code.Utilities;
using Cysharp.Threading.Tasks;
using Game.Core.FinalStateMachine;
using RG.ContentSystem.Core;
using UnityEngine;

namespace Code.States
{
    public class AttributesSelectionState : BaseState<ContentRef<BuddyEntry>>
    {
        private GameFlow gameFlow;
        private GameRunState gameRunState;

        public Buddy Buddy { get; private set; }

        public AttributesSelectionState(GameFlow gameFlow, GameRunState parentFsm)
        {
            this.gameFlow = gameFlow;
            gameRunState = parentFsm;
        }

        protected override async UniTask OnEnter()
        {
            var buddyEntry = ContentManager.GetContent(Arguments);
            var view = Object.Instantiate(buddyEntry.Prefab, Game.Instance.BuddyPoint);
            view.transform.localPosition = Vector3.zero;
            Buddy = new Buddy(buddyEntry, view);
            gameFlow.GameState.Buddy = Buddy;
            
            Game.Instance.AttackButton.gameObject.SetActive(true);
            Game.Instance.AttackButton.onClick.AddListener(OnAttackButtonClicked);

            gameFlow.GameState.Bag.Clear();
            gameFlow.GameState.Bag.AddRange(gameFlow.GameState.Dices);

            while (gameFlow.GameState.Bag.Count > 0)
            {
                await gameFlow.RollDice(true);
            }

            if (ContentManager.GetSettings<GameSettings>().AutoSelectHpAndShield)
            {
                while (Buddy.BuddyView.HpDiceHandHolder.Dices.Count < Buddy.BuddyEntry.HealthDiceCount)
                {
                    var dice = gameFlow.GameState.Hand[^1];
                    gameFlow.GameState.Hand.Remove(dice);
                    dice.DeOccupy();
                    dice.Occupy(Buddy.BuddyView.HpDiceHandHolder);
                }

                while (Buddy.BuddyView.ShieldDiceHandHolder.Dices.Count < Buddy.BuddyEntry.ShieldDiceCount)
                {
                    var dice = gameFlow.GameState.Hand[^1];
                    gameFlow.GameState.Hand.Remove(dice);
                    dice.DeOccupy();
                    dice.Occupy(Buddy.BuddyView.ShieldDiceHandHolder);
                }
            }
        }

        protected override async UniTask OnExit()
        {
            Game.Instance.AttackButton.gameObject.SetActive(false);
            Game.Instance.AttackButton.onClick.RemoveListener(OnAttackButtonClicked);
        }

        private void OnAttackButtonClicked() => UniTask.Create(async () =>
        {
            if (!Buddy.BuddyView.HpDiceHandHolder.IsFull())
            {
                Debug.Log("HP dice hand is not full");
                //TODO: Show error message
                return;
            }
            
            if (!Buddy.BuddyView.ShieldDiceHandHolder.IsFull())
            {
                Debug.Log("Shield dice hand is not full");
                //TODO: Show error message
                return;
            }
            
            Buddy.BuddyView.HpDiceHandHolder.Lock();
            Buddy.BuddyView.ShieldDiceHandHolder.Lock();
            
            gameFlow.GameState.Dices.Clear();
            gameFlow.GameState.Dices.AddRange(Game.Instance.handDiceHolder.Dices.Select(dice => dice.DiceState));
            
            for (var i = Game.Instance.handDiceHolder.Dices.Count - 1; i >= 0; i--)
            {
                await Game.Instance.handDiceHolder.Dices[i].DiceState.DestroyDice();
            }
            
            gameFlow.GameState.Hand.Clear();
            gameRunState.EnterFightState();
        });
    }
}