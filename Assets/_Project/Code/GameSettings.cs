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
        public int StartCoins;

        // public ContentRef<ChallengeEntry>[] Challenges;
        
        [Space]
        public ContentRef<BuddyEntry> TutorialBuddy;
        public ContentRef<BuddyEntry> FirstBuddy;
        public ContentRef<BuddyEntry> SecondBuddy;
        public ContentRef<BuddyEntry> LastBuddy;
        
        [Space]
        public ContentRef<DiceSetEntry> StartingDiceSet;

        [Space] 
        public Sprite D4HotModifierSprite;
        public Sprite D6HotModifierSprite;
        public Sprite D8HotModifierSprite;
        public Sprite D12HotModifierSprite;
        public Sprite D20HotModifierSprite;

        [Title("===TEST===")] 
        public bool AutoSelectHpAndShield;

    }
}