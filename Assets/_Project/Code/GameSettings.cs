using System;
using Code.Dices;
using Code.Enemies;
using RG.ContentSystem.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code
{
    [Serializable]
    public class GameSettings : SettingsEntry
    {
        public ContentRef<ChallengeEntry>[] Challenges;
        
        [Space]
        public ContentRef<DiceSetEntry> StartingDiceSet;

        [Title("===TEST===")] 
        public bool AutoSelectHpAndShield;
    }
}