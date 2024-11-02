using System;
using RG.ContentSystem.Core;
using RG.ContentSystem.Core.Constants;
using UnityEngine;

namespace Code.Dices
{
    [Serializable]
    [WithConstants]
    public class DiceEntry : ContentEntry
    {
        [field:SerializeField] public int MaxDiceValue { get; private set; }
        [field:SerializeField] public Dice DicePrefab { get; private set; }
        [field:SerializeField] public Sprite DiceSprite { get; private set; }
        [field:SerializeField] public float FontSize { get; private set; }
    }
}