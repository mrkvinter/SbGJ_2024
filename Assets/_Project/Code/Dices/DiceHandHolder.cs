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
        private List<Dice> dices = new();
        private Dice previewDice;
        public IReadOnlyList<Dice> Dices => dices;

        private void Update()
        {
            // ArrangeСentre();
        }

        public void DeOccupy(Dice dice)
        {
            // dice.transform.SetParent(null);
            dices.Remove(dice);
            ArrangeСentre();
        }

        public bool Occupy(Dice dice)
        {
            if (dices.Contains(dice))
            {
                return false;
            }

            // var index = CalculateIndex(dice);
            dices.Add(dice);
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
            for (int i = 0; i < count; i++)
            {
                var dice = dices[i];
                var x = i * offset - fullWidth / 2;
                // if (previewIndex.HasValue)
                // {
                //     if (i >= previewIndex.Value)
                //     {
                //         x += diceWidth * 0.5f;
                //     }
                //     else
                //     {
                //         x -= diceWidth * 0.5f;
                //     }
                //
                //     x += diceWidth;
                // }

                dice.transform.DOLocalMove(new Vector3(x, 0, 0), 0.1f);
            }
        }

        public void OccupyPreview(Dice dice)
        {
            previewDice = dice;
        }
    }
}