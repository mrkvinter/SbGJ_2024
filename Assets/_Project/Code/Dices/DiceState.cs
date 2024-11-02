using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Code.Dices
{
    public class DiceState
    {
        
        public DiceEntry DiceEntry { get; }
        public Dice DiceView { get; private set; }
        public int Value { get; private set; }
        public bool HaveToShowLeftIndicator => DiceEntry.Duplicator;

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
        
        public async UniTask DestroyDice()
        {
            var diceView = DiceView;
            ClearView();

            var diceHolder = diceView.DiceHolderParent;
            diceHolder.DeOccupy(diceView);
            diceView.transform.DOKill();
            await diceView.transform.DOScale(Vector3.zero, 0.2f).ToUniTask(); 
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
    }
}