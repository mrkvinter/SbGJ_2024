using System;
using System.Linq;
using Code.Dices;
using Cysharp.Threading.Tasks;
using RG.ContentSystem.Core;
using UnityEngine;

namespace Code.Buddies
{
    public class Buddy
    {
        public BuddyEntry BuddyEntry { get; }
        public BuddyView BuddyView { get; }

        private DiceHandHolder healthDiceHandHolder;
        private DiceHandHolder shieldDiceHandHolder;
        public int Health => healthDiceHandHolder.Dices.Sum(dice => dice.DiceState.Value);

        public Buddy(BuddyEntry buddyEntry, BuddyView buddyView)
        {
            BuddyEntry = buddyEntry;
            BuddyView = buddyView;
            
            healthDiceHandHolder = buddyView.HpDiceHandHolder;
            shieldDiceHandHolder = buddyView.ShieldDiceHandHolder;

            BuddyView.Initialize(this);
        }

        public async UniTask TakeDamage(int damage)
        {
            while (damage > 0 && Health > 0)
            {
                if (shieldDiceHandHolder.Dices.Count > 0)
                {
                    var shieldDice = shieldDiceHandHolder.Dices[^1];
                    var shieldDiceValue = shieldDice.DiceState.Value;
                    var damageToTake = Mathf.Min(shieldDiceValue, damage);
                    damage -= damageToTake;
                    shieldDice.DiceState.SetValue(shieldDiceValue - damageToTake);
                    if (shieldDice.DiceState.Value == 0)
                    {
                        await shieldDice.DiceState.DestroyDice();
                    }
                }
                else if (healthDiceHandHolder.Dices.Count > 0)
                {
                    var healthDice = healthDiceHandHolder.Dices[^1];
                    var healthDiceValue = healthDice.DiceState.Value;
                    var damageToTake = Mathf.Min(healthDiceValue, damage);
                    damage -= damageToTake;
                    healthDice.DiceState.SetValue(healthDiceValue - damageToTake);
                    if (healthDice.DiceState.Value == 0)
                    {
                        await healthDice.DiceState.DestroyDice();
                    }
                }
                
                await UniTask.Yield();
            }
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