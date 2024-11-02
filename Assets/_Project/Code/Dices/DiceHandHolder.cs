using System;
using System.Collections.Generic;
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
        public int maxDiceCount = 3;
        
        private List<Dice> dices = new();
        private Dice previewDice;
        public IReadOnlyList<Dice> Dices => dices;
        
        public bool IsLocked { get; private set; }

        public bool IsFull() => maxDiceCount >= 0 && dices.Count >= maxDiceCount;

        public void DeOccupy(Dice dice)
        {
            dices.Remove(dice);
            ArrangeСentre();
        }

        public bool Occupy(Dice dice)
        {
            if (dices.Contains(dice) || (maxDiceCount >= 0 && dices.Count >= maxDiceCount))
            {
                return false;
            }

            previewDice = null;
            var index = CalculateIndex(dice);
            dices.Insert(index, dice);
            dice.transform.SetParent(transform);
            ArrangeСentre();

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
                var x = i * offset - fullWidth / 2;
                if (previewIndex.HasValue)
                {
                    if (i - previewIndex.Value >= 0)
                    {
                        x += diceWidth * 0.5f;
                    }
                    else if (i - previewIndex.Value <= -1)
                    {
                        x -= diceWidth * 0.5f;
                    }
                }

                dice.transform.DOLocalMove(new Vector3(x, 0, -0.1f), 0.1f);
            }
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
    }
}