using Code.Buddies;
using Code.Utilities;
using Cysharp.Threading.Tasks;
using Game.Core.FinalStateMachine;
using UnityEngine;

namespace Code.States
{
    public class SelectBuddyState : BaseState
    {
        private GameFlow gameFlow;
        private GameSettings gameSettings;

        public SelectBuddyState(GameFlow gameFlow)
        {
            this.gameFlow = gameFlow;
            gameSettings = ContentManager.GetSettings<GameSettings>();
        }

        protected override async UniTask OnEnter()
        {
            var buddyCompleted = PlayerPrefs.GetInt("BuddyCompleted", 0);

            Game.Instance.BuddySelectorRoot.gameObject.SetActive(true);

            Game.Instance.TutorialBuddySelector.NotOpenedContent.gameObject.SetActive(false);
            Game.Instance.TutorialBuddySelector.SelectButton.onClick.AddListener(OnTutorialBuddyClicked);

            Game.Instance.FirstBuddySelector.NotOpenedContent.gameObject.SetActive(!(buddyCompleted >= 1));
            Game.Instance.FirstBuddySelector.SelectButton.interactable = buddyCompleted >= 1;
            Game.Instance.FirstBuddySelector.SelectButton.onClick.AddListener(OnFirstBuddyClicked);
            
            Game.Instance.SecondBuddySelector.NotOpenedContent.gameObject.SetActive(!(buddyCompleted >= 2));
            Game.Instance.SecondBuddySelector.SelectButton.interactable = buddyCompleted >= 2;
            Game.Instance.SecondBuddySelector.SelectButton.onClick.AddListener(OnSecondBuddyClicked);
            
            Game.Instance.LastBuddySelector.NotOpenedContent.gameObject.SetActive(!(buddyCompleted >= 3));
            Game.Instance.LastBuddySelector.SelectButton.interactable = buddyCompleted >= 3;
            Game.Instance.LastBuddySelector.SelectButton.onClick.AddListener(OnLastBuddyClicked);
        }

        protected override async UniTask OnExit()
        {
            Game.Instance.BuddySelectorRoot.gameObject.SetActive(false);
            Game.Instance.TutorialBuddySelector.SelectButton.onClick.RemoveListener(OnTutorialBuddyClicked);
            Game.Instance.FirstBuddySelector.SelectButton.onClick.RemoveListener(OnFirstBuddyClicked);
            Game.Instance.SecondBuddySelector.SelectButton.onClick.RemoveListener(OnSecondBuddyClicked);
            Game.Instance.LastBuddySelector.SelectButton.onClick.RemoveListener(OnLastBuddyClicked);
        }

        private void OnTutorialBuddyClicked()
        {
            gameFlow.StartWithBuddy(gameSettings.TutorialBuddy);
        }

        private void OnFirstBuddyClicked()
        {
            gameFlow.StartWithBuddy(gameSettings.FirstBuddy);
        }

        private void OnSecondBuddyClicked()
        {
            gameFlow.StartWithBuddy(gameSettings.SecondBuddy);
        }

        private void OnLastBuddyClicked()
        {
            gameFlow.StartWithBuddy(gameSettings.LastBuddy);
        }
    }
}