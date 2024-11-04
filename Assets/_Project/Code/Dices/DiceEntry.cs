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
        [field: SerializeField] public int MaxDiceValue { get; private set; }
        [field: SerializeField] public DiceRank DiceRank { get; private set; }
        [field: SerializeField] public Dice DicePrefab { get; private set; }
        [field: SerializeField] public Sprite DiceSprite { get; private set; }
        [field: SerializeField] public Color DiceColor { get; private set; }
        [field: SerializeField] public Color DiceTextColor { get; private set; }
        [field: SerializeField] public float FontSize { get; private set; }
        [field: Space, SerializeField] public bool GhostDice { get; private set; }
        [field:SerializeField] public bool Duplicator { get; private set; }
        [field:SerializeField] public bool Reroller { get; private set; }
        [field:SerializeField] public bool LoneWolf { get; private set; }
        [field:SerializeField] public bool IsEpicCube { get; private set; }
        [field:SerializeField] public bool IsStrongCube { get; private set; }
        [field:SerializeField] public bool IsGlassCube { get; private set; }
    }
    
    [Serializable]
    public class DiceSetEntry : ContentEntry
    {
        [field: SerializeField] public ContentRef<DiceEntry>[] DiceEntries { get; private set; }
    }
}