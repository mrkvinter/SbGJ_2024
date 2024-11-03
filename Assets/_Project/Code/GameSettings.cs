using System;
using Code.Buddies;
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
        public ContentRef<BuddyEntry> TutorialBuddy;
        public ContentRef<BuddyEntry> FirstBuddy;
        public ContentRef<BuddyEntry> SecondBuddy;
        public ContentRef<BuddyEntry> LastBuddy;
        
        [Space]
        public ContentRef<DiceSetEntry> StartingDiceSet;

        [Title("===TEST===")] 
        public bool AutoSelectHpAndShield;
    }
}