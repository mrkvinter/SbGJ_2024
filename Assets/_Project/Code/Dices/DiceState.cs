using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Code.Dices
{
    public class DiceState
    {
        
        public DiceEntry DiceEntry { get; }
        public Dice DiceView { get; private set; }
        public int Value { get; private set; }
        public bool HaveToShowLeftIndicator => DiceEntry.Duplicator || DiceEntry.Reroller;

        public DiceState(DiceEntry diceEntry)
        {
            DiceEntry = diceEntry;
        }
        
        public void SetValue(int value)
        {
            Value = value;
            DiceView?.SetValue(value);
        }
        
        public void SetView(Dice dice)
        {
            DiceView = dice;
            DiceView.Init(this);
            DiceView.SetValue(Value);
        }
        
        public void ClearView()
        {
            DiceView = null;
        }
        
        public void OnDiceHolderUpdated()
        {
            if (DiceEntry.Duplicator)
            {
                var index = DiceView?.DiceHolderParent?.Dices.IndexOf(DiceView) ?? -1;
                if (index >= 1)
                {
                    var leftDice = DiceView.DiceHolderParent.Dices[index - 1];
                    SetValue(leftDice.DiceState.Value);
                }
                else
                {
                    SetValue(0);
                }
            }
        }
        
        private void Reroll()
        {
            var value = Random.Range(1, DiceEntry.MaxDiceValue + 1);
            SetValue(value);
        }


        public async UniTask DestroyDice()
        {
            var diceView = DiceView;
            ClearView();

            var diceHolder = diceView.DiceHolderParent;
            diceHolder.DeOccupy(diceView);
            diceView.transform.DOKill();
            await diceView.transform.DOScale(Vector3.zero, 0.15f).ToUniTask(); 
            Object.Destroy(diceView.gameObject);
        }
        
        public void DeOccupy()
        {
            DiceView.DiceHolderParent.DeOccupy(DiceView);
            DiceView.SetDiceHolderParent(null);
        }
        
        public void Occupy(DiceHandHolder diceHandHolder)
        {
            DiceView.SetDiceHolderParent(diceHandHolder);
            diceHandHolder.Occupy(DiceView);
        }
        
        public async UniTask CalculateValue(Action updater)
        {
            if (DiceView == null || DiceView.DiceHolderParent == null)
            {
                return;
            }

            await DiceView.transform.DOLocalMoveY(.25f, 0.1f).ToUniTask();

            if (DiceEntry.LoneWolf)
            {
                if (DiceView.DiceHolderParent.Dices.Count == 1)
                {
                    var tween = DiceView.transform.DOShakeRotation(0.4f, 90*Vector3.forward, 10, 90f, false);
                    await UniTask.Delay(200);
                    SetValue(DiceEntry.MaxDiceValue);
                    await tween.AsyncWaitForCompletion();
                }
            }

            updater?.Invoke();
            await UniTask.Delay(500);

            if (DiceEntry.Reroller)
            {
                var index = DiceView.DiceHolderParent.Dices.IndexOf(DiceView);
                if (index >= 1)
                {
                    var leftDice = DiceView.DiceHolderParent.Dices[index - 1];
                    var tween = DiceView.transform.DOShakeRotation(0.4f, 90*Vector3.forward, 10, 90f, false);
                    await UniTask.Delay(200);
                    leftDice.DiceState.Reroll();
                    await tween.AsyncWaitForCompletion();
                    await leftDice.DiceState.CalculateValue(updater);
                }
            }
            
            await DiceView.transform.DOLocalMoveY(0, 0.1f).ToUniTask();
            // await UniTask.Delay(350);
        }
    }
}