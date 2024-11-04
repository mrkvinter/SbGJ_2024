using System;
using Code.Dices;
using RG.ContentSystem.Core;
using UnityEngine;

namespace Code.DiceSets
{
    [Serializable]
    public class DiceSetShopEntry : ContentEntry
    {
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public int Price { get; private set; }
        [field: SerializeField] public int DiceCount { get; private set; }
        [field: SerializeField] public int DiceToSelect { get; private set; }
        [field: SerializeField] public DiceSetProbability[] Probabilities { get; private set; }
    }
    
    [Serializable] 
    public class DiceSetProbability
    {
        [field: SerializeField] public DiceRank Rank { get; private set; }
        [field: SerializeField] public float Probability { get; private set; }
    }
}