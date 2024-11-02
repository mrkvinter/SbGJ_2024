using System;
using RG.ContentSystem.Core;
using UnityEngine;

namespace Code.Buddies
{
    public class Buddy
    {
        public BuddyEntry BuddyEntry { get; }
        public BuddyView BuddyView { get; }

        public Buddy(BuddyEntry buddyEntry, BuddyView buddyView)
        {
            BuddyEntry = buddyEntry;
            BuddyView = buddyView;
            
            BuddyView.Initialize(this);
        }
    }
    
    [Serializable]
    public class BuddyEntry : ContentEntry
    {
        [field: SerializeField] public int HealthDiceCount { get; private set; }
        [field: SerializeField] public int ShieldDiceCount { get; private set; }
        [field: SerializeField] public BuddyView Prefab { get; private set; }
    }
}