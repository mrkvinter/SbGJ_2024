using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Code.Dices
{
    public interface IDiceDropHandler
    {
        bool Occupy(Dice dice);
    }

    public class DiceHandHolder : MonoBehaviour, IDiceDropHandler
    {
        [SerializeField] private float offset = 0.09f;
        [SerializeField] private Transform[] slots;
        
        public int maxDiceCount = 3;

        private List<Dice> dices = new();
        private Dice previewDice;
        public IList<Dice> Dices => dices;

        public bool IsLocked { get; private set; }
        
        public bool IsFull()
        {
            if (maxDiceCount < 0)
            {
                return false;
            }

            var count = dices.Count(e => !e.DiceState.DiceEntry.GhostDice);
            return count >= maxDiceCount;
        }

        public int RealMaxDiceCount
        {
            get
            {
                if (maxDiceCount < 0)
                {
                    return -1;
                }

                var count = dices.Count(e => e.DiceState.DiceEntry.GhostDice);
                return maxDiceCount + count;
            }
        }

        public void DeOccupy(Dice dice)
        {
            dices.Remove(dice);
            ArrangeСentre();
            
            foreach (var d in dices)
            {
                d.DiceState.OnDiceHolderUpdated();
            }
            
            dice.DiceState.OnDiceHolderUpdated();
        }

        public bool Occupy(Dice dice)
        {
            if (dices.Contains(dice) || (!dice.DiceState.DiceEntry.GhostDice && IsFull()) || IsLocked)
            {
                return false;
            }

            dice.SetDiceHolderParent(this);
            previewDice = null;
            var index = CalculateIndex(dice);
            dices.Insert(index, dice);
            dice.transform.SetParent(transform);
            ArrangeСentre();
            foreach (var d in dices)
            {
                d.DiceState.OnDiceHolderUpdated();
            }

            return true;
        }

        private int CalculateIndex(Dice dice)
        {
            var count = dices.Count;
            for (int i = 0; i < count; i++)
            {
                if (dice.transform.position.x < dices[i].transform.position.x)
                {
                    return i;
                }
            }

            return count;
        }

        private void ArrangeСentre()
        {
            var count = dices.Count;
            var diceWidth = 0.25f;
            var fullWidth = count * offset - offset;
            int? previewIndex = null;
            if (previewDice != null)
            {
                previewIndex = CalculateIndex(previewDice);
            }

            for (int i = 0; i < count; i++)
            {
                var dice = dices[i];
                var shift = 0f;
                var x = i * offset - fullWidth / 2;
                if (previewIndex.HasValue)
                {
                    if (i - previewIndex.Value >= 0)
                    {
                        shift = diceWidth * 0.5f;
                    }
                    else if (i - previewIndex.Value <= -1)
                    {
                        shift = -diceWidth * 0.5f;
                    }
                }

                if (slots.Length != 0)
                {
                    if (previewIndex.HasValue && i - previewIndex.Value >= 0)
                    {
                        shift = diceWidth * 2f;
                    }
                    else if (previewIndex.HasValue && i - previewIndex.Value <= -1)
                    {
                        shift = 0;
                    }
                    if (slots.Length > i)
                    {
                        var endValue = new Vector3(slots[i].position.x + shift, slots[i].position.y, -0.1f);
                        dice.transform.DOMove(endValue, 0.1f);
                    }
                    else
                    {
                        var distance = slots[1].localPosition - slots[0].localPosition;
                        var endValue = new Vector3(slots[0].position.x + distance.x * i + shift, slots[0].position.y, -0.1f);
                        dice.transform.DOMove(endValue, 0.1f);
                    }
                }
                else
                {
                    dice.transform.DOLocalMove(new Vector3(x + shift, 0, -0.1f), 0.1f);
                }
            }
            
            UpdateSlotsVisibility();
        }

        public void OccupyPreview(Dice dice)
        {
            previewDice = dice;
            ArrangeСentre();
        }

        public void Lock()
        {
            IsLocked = true;
        }
        
        private void UpdateSlotsVisibility()
        {
            for (var i = 0; i < slots.Length; i++)
            {
                slots[i].gameObject.SetActive(i < RealMaxDiceCount);
            }
        }
    }
}